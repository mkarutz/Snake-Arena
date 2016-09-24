package in.slyther.gameobjects;

import com.google.flatbuffers.FlatBufferBuilder;
import in.slyther.math.Vector2;
import slyther.flatbuffers.SnakePartState;
import slyther.flatbuffers.Vec2;

public class SnakePart {
    private final int index;
    private Vector2 position;

    public SnakePart(int index, Vector2 position) {
        this.index = index;
        this.position = position;
    }


    public int serialize(FlatBufferBuilder builder) {
        SnakePartState.startSnakePartState(builder);
        SnakePartState.addPosition(builder, Vec2.createVec2(builder, position.getX(), position.getY()));
        SnakePartState.addIndex(builder, index);

        return SnakePartState.endSnakePartState(builder);
    }


    public Vector2 getPosition() {
        return position;
    }

    public void setPosition(Vector2 position) {
        this.position = position;
    }
}
