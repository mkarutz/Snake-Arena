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
    private static final int STARTING_SCORE = 200;
    public static final int MAX_PLAYERS = 100;
    public static final int MAX_FOOD = 1000;
    private static final int FOOD_MAX_WEIGHT = 50;

    private final Random random = new Random();
    private final Server server;
    private final Snake[] snakes = new Snake[MAX_PLAYERS];
    private final Food[] food = new Food[MAX_FOOD];

    private final Deque<Integer> freeSnakeIdsPool = new ArrayDeque<>(MAX_PLAYERS);
    private final Deque<Integer> freeFoodIdsPool = new ArrayDeque<>(MAX_FOOD);

    private final SpatialMap<Snake> snakeSpatialMap = new SpatialHashMap<>(
            new Vector2(-2 * WORLD_RADIUS, -2 * WORLD_RADIUS),
            new Vector2(2 * WORLD_RADIUS, 2 * WORLD_RADIUS),
            10);

    private final SpatialMap<Food> foodSpatialMap = new SpatialHashMap<>(
            new Vector2(-2 * WORLD_RADIUS, -2 * WORLD_RADIUS),
            new Vector2(2 * WORLD_RADIUS, 2 * WORLD_RADIUS),
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
    }


    public void simulate(int tick) {
    }


    public void handleInput(int snakeId, boolean isTurbo, Vector2 desiredMove) {
        final Snake snake = snakes[snakeId];

        snake.setTurbo(isTurbo);
        snake.move(desiredMove, server.getTimeStep() / 1000.0f);

        snakeSpatialMap.update(snake, snake.getBoundingBox());
    }


    public int serializeObjectStates(FlatBufferBuilder builder, int tick, ClientProxy clientProxy) {
        int[] objectOffsets = new int[MAX_PLAYERS + MAX_FOOD];
        int n = 0;

        // Get snakes near the viewport
        for (Snake snake : snakeSpatialMap.getNear(clientProxy.getViewportZone())) {
            int snakeStateOffset = snake.serialize(builder);
            NetworkObjectState.startNetworkObjectState(builder);
            NetworkObjectState.addStateType(builder, NetworkObjectStateType.NetworkSnakeState);
            NetworkObjectState.addState(builder, snakeStateOffset);
            objectOffsets[n++] = NetworkObjectState.endNetworkObjectState(builder);
        }

        // Get the food near the viewport
        for (Food food : foodSpatialMap.getNear(clientProxy.getViewportZone())) {
            int foodStateOffset = food.serialize(builder);
            NetworkObjectState.startNetworkObjectState(builder);
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
     * Respawns a food at a random location.
     * @param food The food to respawn.
     */
    private void respawnFood(Food food) {
        food.getPosition().setRandomUniform(WORLD_RADIUS);
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
            food[i] = new Food(i, Vector2.randomUniform(WORLD_RADIUS), random.nextInt(FOOD_MAX_WEIGHT));
            foodSpatialMap.put(food[i], food[i].getPosition());
        }
    }
}
