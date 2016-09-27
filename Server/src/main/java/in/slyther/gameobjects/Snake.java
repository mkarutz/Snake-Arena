package in.slyther.gameobjects;

import com.google.flatbuffers.FlatBufferBuilder;
import in.slyther.math.Rect;
import in.slyther.math.Vector2;
import slyther.flatbuffers.NetworkSnakeState;


/**
 *
 */
public class Snake {
    public static final int GROWTH_CAP = 40000;
    public static final float MIN_LENGTH = 1.0f;
    public static final float MIN_THICKNESS = 0.2f;
    public static final float GROWTH_RATE = 1.0f / 100.0f;
    public static final float MOVE_SPEED = 2.0f;
    public static final float TURN_RADIUS_FACTOR = 4.0f;
    public static final int MAX_PARTS = 100;

    public static final float MAX_SEGMENT_ANGLE_DEG = 5.0f;
    public static final float MAX_SEGMENT_ANGLE = MAX_SEGMENT_ANGLE_DEG * (float) Math.PI / 180.0f;

    private final int pid;
    private String name;
    private int skin;
    private int score;
    private boolean isTurbo;
    private boolean isDead;

    private final SnakePart[] parts = new SnakePart[MAX_PARTS];
    private int headPointer;
    private int tailPointer;

    private Rect boundingBox = Rect.unit();


    /**
     * Snake constructor.
     * @param pid Player ID.
     * @param name Player name.
     * @param score Player score.
     */
    public Snake(int pid, String name, int score) {
        this.pid = pid;
        this.name = name;
        this.score = score;

        initSnakeParts();
        respawn(Vector2.zero(), score);

        setDead(true);
    }


    /**
     * Initializes the parts of a new snake.
     */
    private void initSnakeParts() {
        for (int i = 0; i < MAX_PARTS; i++) {
            parts[i] = new SnakePart(i, Vector2.zero());
        }
    }


    /**
     * Updates the bounding box of this snake.
     */
    public void updateBoundingBox() {
        float x0 = Float.MAX_VALUE;
        float x1 = Float.MIN_VALUE;
        float y0 = Float.MAX_VALUE;
        float y1 = Float.MIN_NORMAL;

        for (int i = headPointer; i != tailPointer; i = nextPointer(i)) {
            x0 = Math.min(x0, parts[i].getPosition().getX());
            x1 = Math.max(x1, parts[i].getPosition().getX());
            y0 = Math.min(y0, parts[i].getPosition().getY());
            y1 = Math.max(y1, parts[i].getPosition().getY());
        }

        boundingBox.setMinX(x0);
        boundingBox.setMinY(y0);
        boundingBox.setMaxX(x1);
        boundingBox.setMaxY(y1);
    }


    /**
     * Respawns the snake.
     * @param position Starting position.
     * @param startingScore Starting starting score.
     */
    public void respawn(Vector2 position, int startingScore) {
        score = startingScore;
        headPointer = 0;
        tailPointer = 3;
        isDead = false;
        isTurbo = false;

        getPart(0).getPosition().set(position);
        getPart(1).getPosition().set(position).translate(1, 1);
        getPart(2).getPosition().set(position).translate(2, 2);

        updateBoundingBox();
    }


    /**
     * Serialize this snake for replication over the network.
     * @param builder FlatBufferBuilder.
     * @return The offset of the flatbuffer.
     */
    public int serialize(FlatBufferBuilder builder) {
        int[] partsOffsets = new int[MAX_PARTS];
        int n = 0;

        for (int i = headPointer; i != tailPointer; i = nextPointer(i)) {
            partsOffsets[n++] = parts[i].serialize(builder);
        }

        int vectorOffset = NetworkSnakeState.createPartsVector(builder, partsOffsets, n);
        int nameOffset = builder.createString(name);

        NetworkSnakeState.startNetworkSnakeState(builder);
        NetworkSnakeState.addPlayerId(builder, pid);
        NetworkSnakeState.addParts(builder, vectorOffset);
        NetworkSnakeState.addHead(builder, headPointer);
        NetworkSnakeState.addIsDead(builder, isDead);
        NetworkSnakeState.addIsTurbo(builder, isTurbo);
        NetworkSnakeState.addName(builder, nameOffset);
        NetworkSnakeState.addScore(builder, score);
        NetworkSnakeState.addTail(builder, tailPointer);
        NetworkSnakeState.addSkin(builder, skin);

        return NetworkSnakeState.endNetworkSnakeState(builder);
    }


    /**
     * Returns the players score capped at the growth limit.
     * @return The capped score.
     */
    public float cappedScore() {
        return Math.min(GROWTH_CAP, score);
    }


    /**
     * Gets the length of the snake.
     * @return The length.
     */
    public float getLength() {
        return MIN_LENGTH + GROWTH_RATE * cappedScore();
    }


    /**
     * Get the thickness of the snake.
     * @return The thickness.
     */
    public float getThickness() {
        return MIN_THICKNESS + GROWTH_RATE * (float) Math.sqrt(cappedScore());
    }


    /**
     * Get the first {@link SnakePart}
     * @return The head.
     */
    public SnakePart getHead() {
        return parts[getHeadPointer()];
    }


    /**
     * Get the spatial position of the snake's head.
     * @return Vector2 position.
     */
    public Vector2 getHeadPosition() {
        return getHead().getPosition();
    }


    /**
     * Move the snake.
     * @param desiredMove The direction in which the player wishes to move.
     */
    public void move(Vector2 desiredMove, float dt) {
//        Vector2 direction = getDirection();
//        if (desiredMove.magnitude() > 0.0f) {
//            direction.rotateTowards(desiredMove, turnSpeed() * dt);
//        }
//
//        getHeadPosition().add(direction.multiply(moveSpeed() * dt));
//
//        if (Math.abs(segmentAngle()) > MAX_SEGMENT_ANGLE) {
//            addNewSnakePart();
//        }
//
//        updateTailPointer();
//        updateBoundingBox();
//        System.out.println("desired move = " + desiredMove);
//        System.out.println("dt = " + dt);
//        System.out.println("movespeed = " + moveSpeed());
//
        Vector2 moveVec = desiredMove.normalize().multiply(0.05f);
        System.out.println("Move vec = " + moveVec);
        getHeadPosition().add(new Vector2(-0.04109339f, 0.028483912f));
    }


    private void updateTailPointer() {
        final float length = getLength();
        float partDistance = 0;

        int i = 0;
        int ptr = headPointer;

        while (ptr != tailPointer) {
            if (i > 2 && partDistance > length) {
                tailPointer = i;
                return;
            }

            partDistance += Vector2.distance(getHeadPosition(), parts[i].getPosition());

            i++;
            ptr = nextPointer(ptr);
        }
    }


    private float segmentAngle() {
        final Vector2 headPosition = getHeadPosition();
        final Vector2 neckPosition = getNeckPosition();
        final Vector2 prevPosition = getPart(2).getPosition();

        final Vector2 headVec = Vector2.minus(headPosition, neckPosition);
        final Vector2 neckVec = Vector2.minus(neckPosition, prevPosition);

        return (float) Vector2.angleBetween(neckVec, headVec);
    }


    private SnakePart getPart(int i) {
        return parts[(headPointer + i) % MAX_PARTS];
    }


    private void addNewSnakePart() {
        if (prevPointer(headPointer) == tailPointer) {
            return;
        }

        headPointer = prevPointer(headPointer);

        getHeadPosition().setX(getNeckPosition().getX());
        getHeadPosition().setY(getNeckPosition().getY());
    }


    private float moveSpeed() {
        return isTurbo ? turboMoveSpeed() : normalMoveSpeed();
    }


    private float turboMoveSpeed() {
        return 2.0f * MOVE_SPEED;
    }


    private float normalMoveSpeed() {
        return MOVE_SPEED;
    }


    private float turnSpeed() {
        return (360 * MOVE_SPEED) / (TURN_RADIUS_FACTOR * ((float) Math.PI) * getThickness());
    }


    /**
     * Move the snake forwards.
     * @param dt Change in game time.
     */
    private void moveHeadForward(float dt) {
        getHead().getPosition().add(getDirection().multiply(dt * MOVE_SPEED));
    }


    private final Vector2 direction = Vector2.up();

    /**
     * Gets the direction the snake is currently facing.
     * @return Vector2 direction.
     */
    public Vector2 getDirection() {
        Vector2 headPos = getHeadPosition();
        Vector2 neckPos = getNeckPosition();

        direction.setX(headPos.getX() - neckPos.getX());
        direction.setY(headPos.getY() - neckPos.getY());

        return direction.normalize();
    }


    /**
     * Gets the position of the second snake part.
     * @return Vector2 position.
     */
    private Vector2 getNeckPosition() {
        return getNeck().getPosition();
    }


    /**
     * Get the second snake part.
     * @return
     */
    private SnakePart getNeck() {
        return parts[getNeckPointer()];
    }

    private int getNeckPointer() {
        return nextPointer(headPointer);
    }

    private int nextPointer(int pointer) {
        return (pointer + 1) % MAX_PARTS;
    }

    private int prevPointer(int pointer) {
        return (pointer - 1) % MAX_PARTS;
    }

    public Rect getBoundingBox() {
        return boundingBox;
    }

    public void setBoundingBox(Rect boundingBox) {
        this.boundingBox = boundingBox;
    }

    public int getPid() {
        return pid;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public int getScore() {
        return score;
    }

    public void setScore(int score) {
        this.score = score;
    }

    public boolean isTurbo() {
        return isTurbo;
    }

    public void setTurbo(boolean turbo) {
        isTurbo = turbo;
    }

    public boolean isDead() {
        return isDead;
    }

    public void setDead(boolean dead) {
        isDead = dead;
    }

    public SnakePart[] getParts() {
        return parts;
    }

    public int getHeadPointer() {
        return headPointer;
    }

    public void setHeadPointer(int headPointer) {
        this.headPointer = headPointer;
    }

    public int getTailPointer() {
        return tailPointer;
    }

    public void setTailPointer(int tailPointer) {
        this.tailPointer = tailPointer;
    }
}
