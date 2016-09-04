package in.slyther.gameobjects;

import in.slyther.math.Vector2;

public class SnakePart {
    private Vector2 position;
    private Vector2 direction;

    public SnakePart(Vector2 position, Vector2 direction) {
        this.position = position;
        this.direction = direction;
    }

    public Vector2 getPosition() {
        return position;
    }

    public void setPosition(Vector2 position) {
        this.position = position;
    }

    public Vector2 getDirection() {
        return direction;
    }

    public void setDirection(Vector2 direction) {
        this.direction = direction;
    }
}
