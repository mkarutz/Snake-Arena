// automatically generated by the FlatBuffers compiler, do not modify

package in.slyther.flatbuffers;

public final class ServerMessageType {
  private ServerMessageType() { }
  public static final byte NONE = 0;
  public static final byte ServerHello = 1;
  public static final byte ServerWorldState = 2;
  public static final byte ServerGoodbye = 3;
  public static final byte TickAck = 4;

  public static final String[] names = { "NONE", "ServerHello", "ServerWorldState", "ServerGoodbye", "TickAck", };

  public static String name(int e) { return names[e]; }
}

