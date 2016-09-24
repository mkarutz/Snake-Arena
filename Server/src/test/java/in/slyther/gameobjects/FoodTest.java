package in.slyther.gameobjects;

import com.google.flatbuffers.FlatBufferBuilder;
import in.slyther.math.Vector2;
import org.junit.Before;
import org.junit.Test;
import slyther.flatbuffers.FoodState;
import slyther.flatbuffers.NetworkObjectState;
import slyther.flatbuffers.NetworkObjectStateType;
import slyther.flatbuffers.SnakeState;

import java.nio.ByteBuffer;
import java.awt.Color;

import static org.junit.Assert.*;

public class FoodTest {
    private static final Vector2 pos = new Vector2(0, 1);
    private static final int weight = 255;
    private static final int foodId = 123;

    private Food food;

    @Before
    public void buildSnake() {
        food = new Food(foodId, pos, weight);
    }

    @Test
    public void testSerialize() throws Exception {
        FlatBufferBuilder builder = new FlatBufferBuilder(0);

        int foodOffset = food.serialize(builder);

        NetworkObjectState.startNetworkObjectState(builder);
        NetworkObjectState.addStateType(builder, NetworkObjectStateType.FoodState);
        NetworkObjectState.addState(builder, foodOffset);
        int objectOffset = NetworkObjectState.endNetworkObjectState(builder);

        NetworkObjectState.finishNetworkObjectStateBuffer(builder, objectOffset);

        ByteBuffer buf = builder.dataBuffer();

        NetworkObjectState state = NetworkObjectState.getRootAsNetworkObjectState(buf);

        assertEquals(state.stateType(), NetworkObjectStateType.FoodState);

        FoodState foodState = (FoodState) state.state(new FoodState());

        assertTrue(Math.abs(foodState.position().x() - pos.getX()) < 1e-4);
        assertTrue(Math.abs(foodState.position().y() - pos.getY()) < 1e-4);

        System.out.println(foodState.weight());

        assertEquals(foodState.weight(), weight);
        assertEquals(foodState.foodId(), foodId);
    }
}
