package in.slyther.gameobjects;

import com.google.flatbuffers.FlatBufferBuilder;
import in.slyther.math.Rect;
import in.slyther.math.Vector2;
import slyther.flatbuffers.SnakeState;


/**
 *
 */
public class Snake {
    public static final int MAX_PARTS = 100;

    private final int pid;
    private String name;
    private int skin;
    private int score;
    private boolean isTurbo;
    private boolean isDead;
    private final SnakePart[] parts = new SnakePart[MAX_PARTS];
    private int head;
    private int tail;

    private Rect boundingBox = Rect.unit();


    /**
     *
     * @param pid
     * @param name
     * @param score
     */
    public Snake(int pid, String name, int score) {
        this.pid = pid;
        this.name = name;
        this.score = score;

        initSnakeParts();
        updateBoundingBox();
    }


    /**
     *
     */
    private void initSnakeParts() {
        for (int i = 0; i < MAX_PARTS; i++) {
            parts[i] = new SnakePart(Vector2.zero());
        }
    }


    public void updateBoundingBox() {
        float x0 = Float.MAX_VALUE;
        float x1 = Float.MIN_VALUE;
        float y0 = Float.MAX_VALUE;
        float y1 = Float.MIN_NORMAL;

        for (int i = 0; i < MAX_PARTS; i++) {
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
     *
     * @param position Starting position.
     * @param startingScore Starting starting score.
     */
    public void respawn(Vector2 position, int startingScore) {
        score = startingScore;
        tail = 0;
        head = 1;
        isDead = false;
        isTurbo = false;

        parts[tail].getPosition().setX(0);
        parts[tail].getPosition().setY(0);

        parts[head].getPosition().setX(position.getX());
        parts[head].getPosition().setY(position.getY());
    }


    public int serialize(FlatBufferBuilder builder) {
        int[] partsOffsets = new int[MAX_PARTS];

        for (int i = 0; i < MAX_PARTS; i++) {
            partsOffsets[i] = parts[i].serialize(builder, i);
        }

        int vectorOffset = SnakeState.createPartsVector(builder, partsOffsets);
        int nameOffset = builder.createString(name);

        SnakeState.startSnakeState(builder);
        SnakeState.addPlayerId(builder, pid);
        SnakeState.addParts(builder, vectorOffset);
        SnakeState.addHead(builder, head);
        SnakeState.addIsDead(builder, isDead);
        SnakeState.addIsTurbo(builder, isTurbo);
        SnakeState.addName(builder, nameOffset);
        SnakeState.addScore(builder, score);
        SnakeState.addTail(builder, tail);
        SnakeState.addSkin(builder, skin);

        return SnakeState.endSnakeState(builder);
    }


    /**
     *
     * @param desiredMove
     */
    public void move(Vector2 desiredMove) {

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

    public int getHead() {
        return head;
    }

    public void setHead(int head) {
        this.head = head;
    }

    public int getTail() {
        return tail;
    }

    public void setTail(int tail) {
        this.tail = tail;
    }
}
