// automatically generated by the FlatBuffers compiler, do not modify

package in.slyther.flatbuffers;

import java.nio.*;
import java.lang.*;
import java.util.*;
import com.google.flatbuffers.*;

@SuppressWarnings("unused")
public final class ClientMessage extends Table {
  public static ClientMessage getRootAsClientMessage(ByteBuffer _bb) { return getRootAsClientMessage(_bb, new ClientMessage()); }
  public static ClientMessage getRootAsClientMessage(ByteBuffer _bb, ClientMessage obj) { _bb.order(ByteOrder.LITTLE_ENDIAN); return (obj.__init(_bb.getInt(_bb.position()) + _bb.position(), _bb)); }
  public ClientMessage __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }

  public int clientId() { int o = __offset(4); return o != 0 ? bb.get(o + bb_pos) & 0xFF : 0; }
  public byte msgType() { int o = __offset(6); return o != 0 ? bb.get(o + bb_pos) : 0; }
  public Table msg(Table obj) { int o = __offset(8); return o != 0 ? __union(obj, o) : null; }

  public static int createClientMessage(FlatBufferBuilder builder,
      int clientId,
      byte msg_type,
      int msgOffset) {
    builder.startObject(3);
    ClientMessage.addMsg(builder, msgOffset);
    ClientMessage.addMsgType(builder, msg_type);
    ClientMessage.addClientId(builder, clientId);
    return ClientMessage.endClientMessage(builder);
  }

  public static void startClientMessage(FlatBufferBuilder builder) { builder.startObject(3); }
  public static void addClientId(FlatBufferBuilder builder, int clientId) { builder.addByte(0, (byte)clientId, 0); }
  public static void addMsgType(FlatBufferBuilder builder, byte msgType) { builder.addByte(1, msgType, 0); }
  public static void addMsg(FlatBufferBuilder builder, int msgOffset) { builder.addOffset(2, msgOffset, 0); }
  public static int endClientMessage(FlatBufferBuilder builder) {
    int o = builder.endObject();
    return o;
  }
  public static void finishClientMessageBuffer(FlatBufferBuilder builder, int offset) { builder.finish(offset); }
}

