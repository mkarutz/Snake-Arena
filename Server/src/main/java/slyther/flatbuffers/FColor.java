// automatically generated by the FlatBuffers compiler, do not modify

package slyther.flatbuffers;

import java.nio.*;
import java.lang.*;
import java.util.*;
import com.google.flatbuffers.*;

@SuppressWarnings("unused")
public final class FColor extends Struct {
  public void __init(int _i, ByteBuffer _bb) { bb_pos = _i; bb = _bb; }
  public FColor __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public long r() { return (long)bb.getInt(bb_pos + 0) & 0xFFFFFFFFL; }
  public long g() { return (long)bb.getInt(bb_pos + 4) & 0xFFFFFFFFL; }
  public long b() { return (long)bb.getInt(bb_pos + 8) & 0xFFFFFFFFL; }

  public static int createFColor(FlatBufferBuilder builder, long r, long g, long b) {
    builder.prep(4, 12);
    builder.putInt((int)b);
    builder.putInt((int)g);
    builder.putInt((int)r);
    return builder.offset();
  }
}
