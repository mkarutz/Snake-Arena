package in.slyther.gameobjects;

import com.google.flatbuffers.FlatBufferBuilder;
import org.junit.Before;
import org.junit.Test;
import slyther.flatbuffers.NetworkObjectState;
import slyther.flatbuffers.NetworkObjectStateType;
import slyther.flatbuffers.SnakeState;

import java.nio.ByteBuffer;

import static org.junit.Assert.*;

public class SnakeTest {
    private static final String name = "foo";
    private static final int id = 123;
    private static final int score = 1337;

    private Snake snake;

    @Before
    public void buildSnake() {
        snake = new Snake(id, name, score);
    }

    @Test
    public void testSerialize() throws Exception {
        FlatBufferBuilder builder = new FlatBufferBuilder(0);

        int snakeOffset = snake.serialize(builder);

        NetworkObjectState.startNetworkObjectState(builder);
        NetworkObjectState.addStateType(builder, NetworkObjectStateType.SnakeState);
        NetworkObjectState.addState(builder, snakeOffset);
        int objectOffset = NetworkObjectState.endNetworkObjectState(builder);

        NetworkObjectState.finishNetworkObjectStateBuffer(builder, objectOffset);

        ByteBuffer buf = builder.dataBuffer();

        NetworkObjectState state = NetworkObjectState.getRootAsNetworkObjectState(buf);

        assertEquals(state.stateType(), NetworkObjectStateType.SnakeState);

        SnakeState snakeState = (SnakeState) state.state(new SnakeState());

        assertEquals(snakeState.name(), name);
        assertEquals(snakeState.score(), score);
        assertEquals(snakeState.playerId(), id);
        assertEquals(snakeState.partsLength(), Snake.MAX_PARTS);
    }
}
