package in.slyther.gameobjects;

import com.google.flatbuffers.FlatBufferBuilder;
import in.slyther.math.Vector2;
import slyther.flatbuffers.FoodState;
import slyther.flatbuffers.Vec2;

public class Food {
    private Vector2 position;
    private int weight;
    private boolean isActive;

    public Food(Vector2 position, int weight) {
        this.position = position;
        this.weight = weight;
    }

    public int serialize(FlatBufferBuilder builder, int index) {
        FoodState.startFoodState(builder);
        FoodState.addFoodId(builder, index);
        FoodState.addIsActive(builder, isActive);
        FoodState.addWeight(builder, weight);
        FoodState.addPosition(builder, Vec2.createVec2(builder, position.getX(), position.getY()));

        return FoodState.endFoodState(builder);
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
