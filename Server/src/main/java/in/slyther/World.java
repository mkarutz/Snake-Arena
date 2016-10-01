package in.slyther;

import com.google.flatbuffers.FlatBufferBuilder;
import in.slyther.gameobjects.Food;
import in.slyther.gameobjects.Snake;
import in.slyther.math.Vector2;
import in.slyther.math.collisions.SpatialHashMap;
import in.slyther.math.collisions.SpatialMap;
import in.slyther.network.ClientProxy;
import slyther.flatbuffers.*;

import java.util.ArrayDeque;
import java.util.Deque;
import java.util.Random;


/**
 *
 */
public class World {
    private static final float WORLD_RADIUS = 20;
    private static final int STARTING_SCORE = 100;
    public static final int MAX_PLAYERS = 100;
    public static final int MAX_FOOD = 1000;
    private static final int FOOD_MAX_WEIGHT = 5;

    private final Random random = new Random();
    private final Server server;
    private final Snake[] snakes = new Snake[MAX_PLAYERS];
    private final Food[] foods = new Food[MAX_FOOD];

    private final Deque<Integer> freeSnakeIdsPool = new ArrayDeque<>(MAX_PLAYERS);
    private final Deque<Integer> freeFoodIdsPool = new ArrayDeque<>(MAX_FOOD);

    private final SpatialMap<Snake> snakeSpatialMap = new SpatialHashMap<>(
            new Vector2(-3 * WORLD_RADIUS, -3 * WORLD_RADIUS),
            new Vector2(3 * WORLD_RADIUS, 3 * WORLD_RADIUS),
            10);

    private final SpatialMap<Food> foodSpatialMap = new SpatialHashMap<>(
            new Vector2(-3 * WORLD_RADIUS, -3 * WORLD_RADIUS),
            new Vector2(3 * WORLD_RADIUS, 3 * WORLD_RADIUS),
            10);


    /**
     *
     * @param server
     */
    public World(Server server) {
        this.server = server;
    }


    /**
     *
     */
    public void initWorld() {
        initFood();
        initSnakes();

        spawnFoods(MAX_FOOD / 2);
    }


    public void simulate(int tick) {
    }


    public void handleInput(int snakeId, boolean isTurbo, Vector2 desiredMove, float dt) {
        final Snake snake = snakes[snakeId];

        if (snake.isDead()) {
            return;
        }

        snake.setTurbo(isTurbo);
        snake.move(desiredMove, dt);
        snakeSpatialMap.update(snake, snake.getBoundingBox());

        checkCollisions(snake);
    }


    private void checkCollisions(Snake snake) {
        checkFoodCollisions(snake);
        checkWorldEdgeCollision(snake);
        checkSnakeCollisions(snake);
    }


    private final Vector2 origin = Vector2.zero();
    private void checkWorldEdgeCollision(Snake snake) {
        if (Vector2.distance(snake.getHeadPosition(), origin) > WORLD_RADIUS) {
            killSnake(snake);
        }
    }


    private boolean shouldRespawnNextFood = false;
    private void checkFoodCollisions(Snake snake) {
        for (Food food : foodSpatialMap.getNear(snake.getBoundingBox())) {
            if (!food.isActive()) {
                continue;
            }

            if (snake.isCollidedWith(food.getPosition())) {
                snake.addScore(food.getWeight());
                food.setActive(false);
                freeFoodIdsPool.add(food.getFoodId());

                if (shouldRespawnNextFood) {
                    respawnFood();
                }

                shouldRespawnNextFood = !shouldRespawnNextFood;
            }
        }
    }


    private void checkSnakeCollisions(Snake snake) {
        for (Snake other : snakeSpatialMap.getNear(snake.getBoundingBox())) {
            if (other.isDead()) {
                continue;
            }

            if (other == snake) {
                continue;
            }

            if (snake.isRunInto(other)) {
                killSnake(snake);
                return;
            }
        }
    }


    public void killSnake(Snake snake) {
        dropFood(snake);
        snake.setDead(true);
        freeSnakeIdsPool.add(snake.getPid());
    }


    private void dropFood(Snake snake) {
        int totalFoodToDrop = snake.getScore() / 2;
        for ( ; totalFoodToDrop > 0; totalFoodToDrop -= 10) {
            float randomLength = (float) Math.random() * snake.getLength();
            Vector2 pos = snake.getPositionAtLength(randomLength);

            if (pos == null) {
                return;
            }

            pos.translate((float) Math.random() * snake.getThickness() / 2,
                    (float) Math.random() * snake.getThickness() / 2);

            spawnFood(pos, 10);
        }
    }


    public int serializeObjectStates(FlatBufferBuilder builder, int tick, ClientProxy clientProxy) {
        int[] objectOffsets = new int[MAX_PLAYERS + MAX_FOOD];
        int n = 0;

        // Get snakes near the viewport
        for (Snake snake : snakeSpatialMap.getNear(clientProxy.getViewportZone())) {
            if (snake.isDead()) {
                continue;
            }

            int snakeStateOffset = snake.serialize(builder);
            NetworkObjectState.startNetworkObjectState(builder);
            NetworkObjectState.addNetworkId(builder, snake.getPid());
            NetworkObjectState.addStateType(builder, NetworkObjectStateType.NetworkSnakeState);
            NetworkObjectState.addState(builder, snakeStateOffset);
            objectOffsets[n++] = NetworkObjectState.endNetworkObjectState(builder);
        }

        // Get the foods near the viewport
        for (Food food : foodSpatialMap.getNear(clientProxy.getViewportZone())) {
            if (!food.isActive()) {
                continue;
            }

            int foodStateOffset = food.serialize(builder);
            NetworkObjectState.startNetworkObjectState(builder);
            NetworkObjectState.addNetworkId(builder, food.getFoodId() + 1000);
            NetworkObjectState.addStateType(builder, NetworkObjectStateType.NetworkFoodState);
            NetworkObjectState.addState(builder, foodStateOffset);
            objectOffsets[n++] = NetworkObjectState.endNetworkObjectState(builder);
        }

        int objectsVectorOffset = NetworkWorldState.createObjectStatesVector(builder, objectOffsets, n);
        NetworkWorldState.startServerWorldState(builder);
        NetworkWorldState.addObjectStates(builder, objectsVectorOffset);
        NetworkWorldState.addTick(builder, tick);

        return NetworkWorldState.endServerWorldState(builder);
    }


    /**
     * Gets a free snake from the pool and respawns it.
     */
    public Snake spawnSnake() {
        final int id = freeSnakeIdsPool.remove();
        respawnSnake(snakes[id]);
        return snakes[id];
    }


    /**
     * Respawns the given {@link Snake}.
     * @param snake The snake to respawn.
     */
    private void respawnSnake(Snake snake) {
        snake.setScore(STARTING_SCORE);
        snake.respawn(Vector2.randomUniform(WORLD_RADIUS), STARTING_SCORE);
        updateSpatialSnakeMap(snake);
    }


    /**
     * Respawns a foods at a random location.
     * @param food The foods to respawn.
     */
    private void respawnFood(Food food) {
        spawnFood(food, Vector2.randomUniform(WORLD_RADIUS), food.getWeight());
    }


    private void respawnFood() {
        Food food = getFreeFood();
        if (food == null) {
            return;
        }

        respawnFood(food);
    }


    private void spawnFood(Vector2 pos, int weight) {
        Food food = getFreeFood();

        assert(food != null);
        if (food == null) {
            return;
        }

        spawnFood(food, pos, weight);
    }


    private Food getFreeFood() {
        if (freeFoodIdsPool.isEmpty()) {
            return null;
        }

        int foodId = freeFoodIdsPool.remove();
        return foods[foodId];
    }


    private void spawnFood(Food food, Vector2 pos, int weight) {
        food.getPosition().set(pos);
        food.setWeight(weight);
        food.setActive(true);
        updateSpatialFoodMap(food);
    }


    private void updateSpatialFoodMap(Food food) {
        foodSpatialMap.update(food, food.getPosition());
    }

    private void updateSpatialSnakeMap(Snake snake) {
        snakeSpatialMap.update(snake, snake.getBoundingBox());
    }

    /**
     *
     */
    private void initSnakes() {
        for (int i = 0; i < MAX_PLAYERS; i++) {
            snakes[i] = new Snake(i, "", STARTING_SCORE);
            freeSnakeIdsPool.add(i);
        }
    }


    /**
     *
     */
    private void initFood() {
        for (int i = 0; i < MAX_FOOD; i++) {
            foods[i] = new Food(i, Vector2.randomUniform(WORLD_RADIUS), 1 + random.nextInt(FOOD_MAX_WEIGHT));
            foodSpatialMap.put(foods[i], foods[i].getPosition());

            freeFoodIdsPool.add(i);
            foods[i].setActive(false);
        }
    }


    private void spawnFoods(int n) {
        for (int i = 0; i < n; i++) {
            Food food = getFreeFood();
            respawnFood(food);
        }
    }
}
