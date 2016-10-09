package in.slyther.gameobjects;

import com.google.flatbuffers.FlatBufferBuilder;
import in.slyther.math.Vector2;
import slyther.flatbuffers.NetworkSnakePartState;
import slyther.flatbuffers.Vec2;

public class SnakePart {
    private final int index;
    private Vector2 position;

    public SnakePart(int index, Vector2 position) {
        this.index = index;
        this.position = position;
    }


    public int serialize(FlatBufferBuilder builder) {
        NetworkSnakePartState.startNetworkSnakePartState(builder);
        NetworkSnakePartState.addPosition(builder, Vec2.createVec2(builder, position.getX(), position.getY()));
        NetworkSnakePartState.addIndex(builder, index);

        return NetworkSnakePartState.endNetworkSnakePartState(builder);
    }


    public Vector2 getPosition() {
        return position;
    }

    public void setPosition(Vector2 position) {
        this.position = position;
    }
}
