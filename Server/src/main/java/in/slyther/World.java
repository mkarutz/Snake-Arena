package in.slyther;

import com.google.flatbuffers.FlatBufferBuilder;
import in.slyther.gameobjects.Food;
import in.slyther.gameobjects.Snake;
import in.slyther.math.Vector2;
import slyther.flatbuffers.*;

import java.nio.ByteBuffer;
import java.util.ArrayDeque;
import java.util.Arrays;
import java.util.Deque;
import java.util.Random;
import java.awt.Color;


/**
 *
 */
public class World {
    private static final float WORLD_RADIUS = 500;
    private static final int STARTING_SCORE = 200;
    public static final int MAX_PLAYERS = 10;
    public static final int MAX_FOOD = 10;
    private static final int FOOD_MAX_WEIGHT = 50;

    private final Random random = new Random();
    private final Server server;
    private final Snake[] snakes = new Snake[MAX_PLAYERS];
    private final Food[] food = new Food[MAX_FOOD];

    private final Deque<Integer> freeSnakeIdsPool = new ArrayDeque<>(MAX_PLAYERS);
    private final Deque<Integer> freeFoodIdsPool = new ArrayDeque<>(MAX_FOOD);


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


    public int serializeObjectStates(FlatBufferBuilder builder, int tick) {
        int[] objectOffsets = new int[MAX_PLAYERS + MAX_FOOD];
        int n = 0;

        for (int i = 0; i < MAX_PLAYERS; i++) {
            int snakeStateOffset = snakes[i].serialize(builder);
            NetworkObjectState.startNetworkObjectState(builder);
            NetworkObjectState.addStateType(builder, NetworkObjectStateType.SnakeState);
            NetworkObjectState.addState(builder, snakeStateOffset);
            objectOffsets[n++] = NetworkObjectState.endNetworkObjectState(builder);
        }

        for (int i = 0; i < MAX_FOOD; i++) {
            int foodStateOffset = food[i].serialize(builder, i);
            NetworkObjectState.startNetworkObjectState(builder);
            NetworkObjectState.addStateType(builder, NetworkObjectStateType.FoodState);
            NetworkObjectState.addState(builder, foodStateOffset);
            objectOffsets[n++] = NetworkObjectState.endNetworkObjectState(builder);
        }

        assert(n == MAX_PLAYERS + MAX_FOOD);

        int objectsVectorOffset = ServerWorldState.createObjectStatesVector(builder, objectOffsets);
        ServerWorldState.startServerWorldState(builder);
        ServerWorldState.addObjectStates(builder, objectsVectorOffset);
        ServerWorldState.addTick(builder, tick);

        return ServerWorldState.endServerWorldState(builder);
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
    }


    /**
     * Respawns a food at a random location.
     * @param food The food to respawn.
     */
    private void respawnFood(Food food) {
        food.getPosition().setRandomUniform(WORLD_RADIUS);
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
            int r = random.nextInt(2)*255;
            int g = ((r==0)?random.nextInt(2):0)*255;
            int b = ((r==0 && g == 0)?1:random.nextInt(2))*255;
            food[i] = new Food(Vector2.randomUniform(WORLD_RADIUS), random.nextInt(FOOD_MAX_WEIGHT), new Color(r,g,b));
        }
    }
}
