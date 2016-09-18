package slyther.flatbuffers;

import com.google.flatbuffers.FlatBufferBuilder;
import in.slyther.World;
import in.slyther.gameobjects.Food;
import in.slyther.gameobjects.Snake;
import in.slyther.math.Vector2;
import org.junit.Before;
import org.junit.Test;

import java.nio.ByteBuffer;
import java.util.ArrayList;
import java.util.List;

import static in.slyther.World.MAX_FOOD;
import static in.slyther.World.MAX_PLAYERS;
import static org.junit.Assert.*;


public class WorldTest {
    private World world;
    private final int tick = 123;

    @Before
    public void before() {
        world = new World(null);
        world.initWorld();
    }

    @Test
    public void testSerialize() {
        FlatBufferBuilder builder = new FlatBufferBuilder(0);

        int worldStateOffset = world.serializeObjectStates(builder, tick);

        ServerMessage.startServerMessage(builder);
        ServerMessage.addMsgType(builder, ServerMessageType.ServerWorldState);
        ServerMessage.addMsg(builder, worldStateOffset);
        int messageOffset = ServerMessage.endServerMessage(builder);

        ServerMessage.finishServerMessageBuffer(builder, messageOffset);

        ByteBuffer buf = builder.dataBuffer();

        ServerMessage msg = ServerMessage.getRootAsServerMessage(buf);

        assertEquals(msg.msgType(), ServerMessageType.ServerWorldState);

        ServerWorldState state = (ServerWorldState) msg.msg(new ServerWorldState());

        assertEquals(state.tick(), tick);
        assertEquals(state.objectStatesLength(), MAX_PLAYERS + MAX_FOOD);
    }
}
