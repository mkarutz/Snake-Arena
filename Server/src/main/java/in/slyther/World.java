package in.slyther;

import com.google.flatbuffers.FlatBufferBuilder;
import in.slyther.gameobjects.Food;
import in.slyther.gameobjects.GameObject;
import in.slyther.gameobjects.ScoreBoard;
import in.slyther.gameobjects.Snake;
import in.slyther.math.Rect;
import in.slyther.math.Vector2;
import in.slyther.math.collisions.SpatialHashMap;
import in.slyther.math.collisions.SpatialMap;
import in.slyther.network.ClientProxy;
import in.slyther.network.LinkingContext;
import slyther.flatbuffers.*;

import java.util.ArrayDeque;
import java.util.Deque;
import java.util.Random;


public class World {
    private static final float WORLD_RADIUS = 50;
    private static final int STARTING_SCORE = 100;
    public static final int MAX_PLAYERS = 100;
    public static final int MAX_FOOD = 10000;
    private static final int FOOD_MAX_WEIGHT = 5;

    private static final int FOOD_DROP_WEIGHT = FOOD_MAX_WEIGHT;

    // Server context
    private final Server server;
    private final LinkingContext linkingContext;

    // Game objects
    private final Snake[] snakes = new Snake[MAX_PLAYERS];
    private final Food[] foods = new Food[MAX_FOOD];
    private final ScoreBoard scoreBoard = new ScoreBoard();

    // Utilities and helper data structures
    private final Random random = new Random();

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
     * World constructor.
     * @param server Server context.
     */
    public World(Server server, LinkingContext linkingContext) {
        this.server = server;
        this.linkingContext = linkingContext;
    }


    /**
     * Initialize game objects.
     */
    public void initWorld() {
        initFood();
        initSnakes();

        spawnFoods(MAX_FOOD / 2);
    }


    public void simulate(int tick) {
    }


    public void handleInput(int snakeId, boolean isTurbo, Vector2 desiredMove, float dt) {
        final Snake snake = (Snake) linkingContext.getGameObject(snakeId);

        if (snake.isDead()) {
            return;
        }

        snake.setTurbo(isTurbo);
        snake.move(desiredMove, dt);
        snakeSpatialMap.update(snake, snake.getBoundingBox());

        if (snake.isTurbo()) {
            dropTurboFood(snake, dt);
        }

        scoreBoard.updateScore(snake);
        checkCollisions(snake);
    }


    private void dropTurboFood(Snake snake, float dt) {
        float snakeLength = snake.getLength();

        Vector2 foodPos = snake.getPositionAtLength(snakeLength);
        spawnFood(foodPos, 1);
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

            if (snake.canReachFoodAt(food.getPosition())) {
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
        if (snake.isDead()) {
            return;
        }

        dropFood(snake);
        snake.setDead(true);
        freeSnakeIdsPool.add(snake.getPid());
        snakeSpatialMap.remove(snake);
        scoreBoard.remove(snake);
    }


    private void dropFood(Snake snake) {
        int totalFoodToDrop = snake.getScore() / 2;
        for ( ; totalFoodToDrop > 0; totalFoodToDrop -= FOOD_DROP_WEIGHT) {
            float randomLength = (float) Math.random() * snake.getLength();
            Vector2 pos = snake.getPositionAtLength(randomLength);

            if (pos == null) {
                return;
            }

            pos.translate((float) Math.random() * snake.getThickness() / 2,
                    (float) Math.random() * snake.getThickness() / 2);

            spawnFood(pos, FOOD_DROP_WEIGHT);
        }
    }


    public int serializeObjectStates(FlatBufferBuilder builder, int tick, ClientProxy clientProxy) {
        int[] objectOffsets = new int[MAX_PLAYERS + MAX_FOOD];
        int n = 0;

        Rect viewport = clientProxy.getViewportZone();

        // Get snakes near the viewport
        for (Snake snake : snakeSpatialMap.getNear(viewport)) {
            if (!viewport.intersects(snake.getBoundingBox())) {
                continue;
            }

            if (!snake.isDead()) {
                objectOffsets[n++] = serializeObject(builder, snake);
            }
        }

        // Get the foods near the viewport
        for (Food food : foodSpatialMap.getNear(viewport)) {
            if (!viewport.collidesWith(food.getPosition())) {
                continue;
            }

            if (food.isActive()) {
                objectOffsets[n++] = serializeObject(builder, food);
            }
        }

        // Serialize scoreboard
        objectOffsets[n++] = serializeObject(builder, scoreBoard);

        int objectsVectorOffset = ServerWorldState.createObjectStatesVector(builder, objectOffsets, n);
        ServerWorldState.startServerWorldState(builder);
        ServerWorldState.addObjectStates(builder, objectsVectorOffset);
        ServerWorldState.addTick(builder, tick);

        return ServerWorldState.endServerWorldState(builder);
    }


    private int serializeObject(FlatBufferBuilder builder, GameObject go) {
        int offset = go.serialize(builder);
        NetworkObjectState.startNetworkObjectState(builder);
        NetworkObjectState.addNetworkId(builder, linkingContext.getNetworkId(go, true));
        NetworkObjectState.addStateType(builder, go.classId());
        NetworkObjectState.addState(builder, offset);
        return NetworkObjectState.endNetworkObjectState(builder);
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
        snake.respawn(Vector2.randomUniform(0.75f * WORLD_RADIUS), STARTING_SCORE);

        updateSpatialSnakeMap(snake);
        scoreBoard.updateScore(snake);
    }


    /**
     * Respawns a foods at a random location.
     * @param food The foods to respawn.
     */
    private void respawnFood(Food food) {
        spawnFood(food, Vector2.randomUniform(WORLD_RADIUS), 1 + random.nextInt(FOOD_MAX_WEIGHT));
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


    private void initSnakes() {
        for (int i = 0; i < MAX_PLAYERS; i++) {
            snakes[i] = new Snake(i, "", STARTING_SCORE);
            freeSnakeIdsPool.add(i);
        }
    }


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
