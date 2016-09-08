package in.slyther.network;
import java.net.SocketAddress;
import java.nio.ByteBuffer;

/**
 *
 */
public class ReceivedPacket {
    private SocketAddress fromAddress;
    private ByteBuffer byteBuffer;

    public ReceivedPacket(SocketAddress fromAddress, ByteBuffer byteBuffer) {
        this.fromAddress = fromAddress;
        this.byteBuffer = byteBuffer;
    }

    public SocketAddress getFromAddress() {
        return fromAddress;
    }

    public void setFromAddress(SocketAddress fromAddress) {
        this.fromAddress = fromAddress;
    }

    public ByteBuffer getByteBuffer() {
        return byteBuffer;
    }

    public void setByteBuffer(ByteBuffer byteBuffer) {
        this.byteBuffer = byteBuffer;
    }
}
