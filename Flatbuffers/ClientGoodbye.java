// automatically generated by the FlatBuffers compiler, do not modify

package in.slyther.flatbuffers;

import java.nio.*;
import java.lang.*;
import java.util.*;
import com.google.flatbuffers.*;

@SuppressWarnings("unused")
public final class ClientGoodbye extends Table {
  public static ClientGoodbye getRootAsClientGoodbye(ByteBuffer _bb) { return getRootAsClientGoodbye(_bb, new ClientGoodbye()); }
  public static ClientGoodbye getRootAsClientGoodbye(ByteBuffer _bb, ClientGoodbye obj) { _bb.order(ByteOrder.LITTLE_ENDIAN); return (obj.__init(_bb.getInt(_bb.position()) + _bb.position(), _bb)); }
  public ClientGoodbye __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; return this; }


  public static void startClientGoodbye(FlatBufferBuilder builder) { builder.startObject(0); }
  public static int endClientGoodbye(FlatBufferBuilder builder) {
    int o = builder.endObject();
    return o;
  }
}

