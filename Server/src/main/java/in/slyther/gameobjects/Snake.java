package in.slyther.gameobjects;

import in.slyther.math.Vector2;

public class Snake {
    private static final int MAX_PARTS = 100;

    private int pid;
    private String name;
    private int score;
    private final SnakePart[] parts = new SnakePart[MAX_PARTS];
    private int head;
    private int tail;

    public Snake(int pid, String name, int score) {
        this.pid = pid;
        this.name = name;
        this.score = score;
    }

    public boolean collides(Vector2 pos) {
        for (int i = tail; i < head; i++) {
            if (distanceToLine(pos, parts[i].getPosition(), parts[(i + 1) % MAX_PARTS].getPosition()) < getRadius()) {
                return true;
            }
        }
        return false;
    }

    private float a(Vector2 p1, Vector2 p2) {
        // ax + by + c = 0
        // a = -(by_0 + c) / x_0
        // b = -(ax + c) / y
        // a = ax - c / x
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

    public SnakePart[] getParts() {
        return parts;
    }
}
