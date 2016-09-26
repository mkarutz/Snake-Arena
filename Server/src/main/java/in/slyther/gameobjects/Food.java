package in.slyther.gameobjects;

import com.google.flatbuffers.FlatBufferBuilder;
import in.slyther.math.Vector2;
import slyther.flatbuffers.NetworkFoodState;
import slyther.flatbuffers.Vec2;

public class Food {
    private int foodId;
    private Vector2 position;
    private int weight;
    private boolean isActive;


    public Food(int foodId, Vector2 position, int weight) {
        this.foodId = foodId;
        this.position = position;
        this.weight = weight;
        isActive = true;
    }

    public int serialize(FlatBufferBuilder builder) {
        NetworkFoodState.startNetworkFoodState(builder);
        NetworkFoodState.addFoodId(builder, foodId);
        NetworkFoodState.addIsActive(builder, isActive);
        NetworkFoodState.addWeight(builder, weight);
        NetworkFoodState.addPosition(builder, Vec2.createVec2(builder, position.getX(), position.getY()));

        return NetworkFoodState.endNetworkFoodState(builder);
    }

    public int getFoodId() {
        return foodId;
    }

    public void setFoodId(int foodId) {
        this.foodId = foodId;
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

    public boolean isActive() {
        return isActive;
    }

    public void setActive(boolean active) {
        isActive = active;
    }
}
