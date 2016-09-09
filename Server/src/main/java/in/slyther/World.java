package in.slyther;

import com.google.flatbuffers.FlatBufferBuilder;
import in.slyther.gameobjects.Food;
import in.slyther.gameobjects.Snake;
import in.slyther.math.Vector2;
import slyther.flatbuffers.ServerWorldState;

import java.util.ArrayDeque;
import java.util.Deque;
import java.util.Random;


/**
 *
 */
public class World {
    private static final float WORLD_RADIUS = 500;
    private static final int STARTING_SCORE = 200;
    private static final int MAX_PLAYERS = 100;
    private static final int MAX_FOOD = 10000;
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
            objectOffsets[n++] = snakes[i].serialize(builder);
        }

        for (int i = 0; i < MAX_FOOD; i++) {
            objectOffsets[n++] = food[i].serialize(builder, i);
        }

        assert(n == MAX_PLAYERS + MAX_FOOD);

        ServerWorldState.startServerWorldState(builder);
        ServerWorldState.createObjectStatesVector(builder, objectOffsets);
        ServerWorldState.addTick(builder, tick);

        return ServerWorldState.endServerWorldState(builder);
    }


    /**
     *
     */
    public Snake spawnSnake() {
        final int id = freeSnakeIdsPool.remove();
        respawnSnake(snakes[id]);
        return snakes[id];
    }


    /**
     * Respawn Snake.
     * @param snake The snake to respawn.
     */
    private void respawnSnake(Snake snake) {
        snake.setScore(STARTING_SCORE);
        snake.respawn(Vector2.randomUniform(WORLD_RADIUS), STARTING_SCORE);
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
            food[i] = new Food(Vector2.randomUniform(WORLD_RADIUS), random.nextInt(FOOD_MAX_WEIGHT));
        }
    }
}
