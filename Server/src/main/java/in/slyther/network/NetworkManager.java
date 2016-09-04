package in.slyther.network;

import in.slyther.flatbuffers.*;

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

        System.out.println("Binding to port " + udpPort);
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
                messageQueue.add(msg);

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
        while (!messageQueue.isEmpty()) {
            final ClientMessage msg = messageQueue.remove();
            final int msgType = msg.msgType();

            switch (msgType) {
                case ClientMessageType.ClientHello:
                    ClientHello hello = (ClientHello) msg.msg(new ClientHello());
                    processHelloMessage(hello);
                    break;
                case ClientMessageType.ClientInputState:
                    ClientInputState inputState = (ClientInputState) msg.msg(new ClientInputState());
                    processInputStateMessage(msg.clientId(), inputState);
                    break;
                case ClientMessageType.ClientGoodbye:
                    ClientGoodbye goodbye = (ClientGoodbye) msg.msg(new ClientGoodbye());
                    processGoodbyeMessage(msg.clientId(), goodbye);
                    break;
            }
        }
    }


    /**
     *
     * @param hello
     */
    private void processHelloMessage(ClientHello hello) {
        System.out.println("Received hello from " + hello.playerName());
    }


    /**
     *
     * @param inputState
     */
    private void processInputStateMessage(int clientId, ClientInputState inputState) {
        System.out.println("Received input from " + clientId);
    }


    /**
     *
     * @param goodbye
     */
    private void processGoodbyeMessage(int clientId, ClientGoodbye goodbye) {
        System.out.println("Received goodbye from " + clientId);
    }


    /**
     *
     */
    public void sendOutgoingPackets() {

    }
}
