// automatically generated by the FlatBuffers compiler, do not modify

package in.slyther.flatbuffers;

import java.nio.*;
import java.lang.*;
import java.util.*;
import com.google.flatbuffers.*;

@SuppressWarnings("unused")
public final class ClientInputState extends Table {
  public static ClientInputState getRootAsClientInputState(ByteBuffer _bb) { return getRootAsClientInputState(_bb, new ClientInputState()); }
  public static ClientInputState getRootAsClientInputState(ByteBuffer _bb, ClientInputState obj) { _bb.order(ByteOrder.LITTLE_ENDIAN); return (obj.__init(_bb.getInt(_bb.position()) + _bb.position(), _bb)); }
  public ClientInputState __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public long tick() { int o = __offset(4); return o != 0 ? (long)bb.getInt(o + bb_pos) & 0xFFFFFFFFL : 0; }
  public in.slyther.flatbuffers.Vector2 desiredMove() { return desiredMove(new in.slyther.flatbuffers.Vector2()); }
  public in.slyther.flatbuffers.Vector2 desiredMove(in.slyther.flatbuffers.Vector2 obj) { int o = __offset(6); return o != 0 ? obj.__init(o + bb_pos, bb) : null; }
  public boolean isTurbo() { int o = __offset(8); return o != 0 ? 0!=bb.get(o + bb_pos) : false; }

  public static void startClientInputState(FlatBufferBuilder builder) { builder.startObject(3); }
  public static void addTick(FlatBufferBuilder builder, long tick) { builder.addInt(0, (int)tick, 0); }
  public static void addDesiredMove(FlatBufferBuilder builder, int desiredMoveOffset) { builder.addStruct(1, desiredMoveOffset, 0); }
  public static void addIsTurbo(FlatBufferBuilder builder, boolean isTurbo) { builder.addBoolean(2, isTurbo, false); }
  public static int endClientInputState(FlatBufferBuilder builder) {
    int o = builder.endObject();
    return o;
  }
}

