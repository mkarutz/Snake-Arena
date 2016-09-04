package in.slyther.network;

import in.slyther.flatbuffers.ClientMessage;

import java.io.IOException;
import java.net.InetSocketAddress;
import java.net.SocketAddress;
import java.nio.ByteBuffer;
import java.nio.channels.DatagramChannel;
import java.util.ArrayDeque;
import java.util.Deque;


/**
 *
 */
public class NetworkManager {
    private static final int MAX_PACKET = 2400;
    private static final int MAX_PACKETS_PER_TICK = 200;

    private final int udpPort;
    private DatagramChannel channel;
    private final ByteBuffer buf = ByteBuffer.allocate(MAX_PACKET);

    private final Deque<ClientMessage> messageQueue = new ArrayDeque<>(MAX_PACKETS_PER_TICK);


    /**
     *
     * @param udpPort
     */
    public NetworkManager(int udpPort) {
        this.udpPort = udpPort;
    }


    /**
     *
     * @throws IOException
     */
    public void bind() throws IOException {
        channel = DatagramChannel.open();
        channel.socket().bind(new InetSocketAddress(udpPort));
        channel.configureBlocking(false);
    }


    /**
     *
     */
    public void handleIncomingPackets() {
        readMessagesToQueue();
        processQueuedMessages();
    }


    /**
     *
     */
    private void readMessagesToQueue() {
        int packetsRead = 0;
        while (packetsRead < MAX_PACKETS_PER_TICK) {
            try {
                final SocketAddress socketAddress = channel.receive(buf);
                if (socketAddress == null) {
                    // No more packets to receive
                    break;
                }

                // Deserialize message and add to queue
                ClientMessage msg = ClientMessage.getRootAsClientMessage(buf);


                packetsRead++;
            } catch (IOException e) {
                e.printStackTrace();
                System.exit(-1);
            }
        }
    }


    /**
     *
     */
    private void processQueuedMessages() {

    }


    /**
     *
     */
    public void sendOutgoingPackets() {

    }
}
