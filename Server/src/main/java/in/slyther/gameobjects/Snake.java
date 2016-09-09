package in.slyther.gameobjects;

import in.slyther.math.Vector2;

import java.util.Vector;


/**
 *
 */
public class Snake {
    private static final int MAX_PARTS = 100;

    private final int pid;
    private String name;
    private int score;
    private boolean isTurbo;
    private boolean isDead;
    private final SnakePart[] parts = new SnakePart[MAX_PARTS];
    private int head;
    private int tail;


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
    }


    /**
     *
     */
    private void initSnakeParts() {
        for (int i = 0; i < MAX_PARTS; i++) {
            parts[i] = new SnakePart(Vector2.zero(), Vector2.zero());
        }
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
