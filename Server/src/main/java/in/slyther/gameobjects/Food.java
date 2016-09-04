package in.slyther.gameobjects;

import in.slyther.math.Vector2;

public class Food {
    private Vector2 position;
    private int weight;

    public Food(Vector2 position, int weight) {
        this.position = position;
        this.weight = weight;
    }

    public Vector2 getPosition() {
        return position;
    }

    public void setPosition(Vector2 position) {
        this.position = position;
    }

    public int getWeight() {
        return weight;
    }

    public void setWeight(int weight) {
        this.weight = weight;
    }
}
