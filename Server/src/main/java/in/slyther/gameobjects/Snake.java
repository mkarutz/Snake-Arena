package in.slyther.gameobjects;

import in.slyther.math.Vector2;


/**
 *
 */
public class Snake {
    private static final int MAX_PARTS = 100;

    private int pid;
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
     * @param desiredMove
     */
    public void move(Vector2 desiredMove) {

    }


    public int getPid() {
        return pid;
    }

    public void setPid(int pid) {
        this.pid = pid;
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
