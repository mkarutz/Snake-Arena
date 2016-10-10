package in.slyther.network;

import com.google.flatbuffers.FlatBufferBuilder;
import in.slyther.Server;
import in.slyther.World;
import in.slyther.gameobjects.Snake;
import in.slyther.math.Vector2;
import slyther.flatbuffers.*;

import java.io.IOException;
import java.net.InetSocketAddress;
import java.net.SocketAddress;
import java.nio.ByteBuffer;
import java.nio.channels.DatagramChannel;
import java.util.*;


/**
 *
 */
public class NetworkManager {
    private static final int TIMEOUT = 5000;
    private static final int MAX_PACKET = 2400;
    private static final int MAX_PACKETS_PER_TICK = 200;

    private final LinkingContext linkingContext;
    private final Server server;
    private final World world;

    private final int udpPort;
    private DatagramChannel channel;

    private final Deque<ReceivedPacket> packetQueue = new ArrayDeque<>(MAX_PACKETS_PER_TICK);
    private final Map<SocketAddress, ClientProxy> socketAddressClientProxyMap = new HashMap<>();

    // Pre-allocated Flatbuffer objects for (de)serialization
    private final ClientMessage clientMessage = new ClientMessage();
    private final ClientHello clientHello = new ClientHello();
    private final ClientGoodbye clientGoodbye = new ClientGoodbye();
    private final ClientInputState clientInputState = new ClientInputState();
    private final ServerMessage serverMessage = new ServerMessage();
    private final ServerHello serverHello = new ServerHello();
    private final ServerConfig serverConfig = new ServerConfig();


    /**
     * NetworkManager constructor.
     * @param world The game world.
     * @param udpPort The port for UDP communication with game clients.
     */
    public NetworkManager(Server server, LinkingContext linkingContext, World world, int udpPort) {
        this.server = server;
        this.linkingContext = linkingContext;
        this.world = world;
        this.udpPort = udpPort;
    }


    /**
     * Bind to the network socket and setup buffers from reading.
     * @throws IOException
     */
    public void bind() throws IOException {
        channel = DatagramChannel.open();
        channel.socket().bind(new InetSocketAddress(udpPort));
        channel.configureBlocking(false);

        System.out.println("Binding to port " + udpPort);
    }


    /**
     * Handle all the incoming packets for this tick.
     * @param tick The current tick.
     */
    public void handleIncomingPackets(int tick) {
        readPacketsToQueue();
        processQueuedPackets(tick);
    }


    /**
     * Receive bytes from the network and queue {@link ReceivedPacket}s
     * for processing.
     */
    private void readPacketsToQueue() {
        int packetsRead = 0;
        while (packetsRead < MAX_PACKETS_PER_TICK) {
            ByteBuffer buf = ByteBuffer.allocate(MAX_PACKET);
            buf.clear();

            try {
                final SocketAddress socketAddress = channel.receive(buf);
                if (socketAddress == null) {
                    break;
                }

                buf.flip();

                final ReceivedPacket packet = new ReceivedPacket(socketAddress, buf);
                packetQueue.add(packet);

                packetsRead++;
            } catch (IOException e) {
                e.printStackTrace();
                System.exit(-1);
            }
        }
    }


    /**
     * Process all the packets read this tick.
     * @param tick The current tick.
     */
    private void processQueuedPackets(int tick) {
        while (!packetQueue.isEmpty()) {
            final ReceivedPacket packet = packetQueue.remove();
            final SocketAddress fromAddress = packet.getFromAddress();

            if (!socketAddressClientProxyMap.containsKey(fromAddress)) {
                processPacketFromNewClient(packet);
            } else {
                final ClientProxy clientProxy = socketAddressClientProxyMap.get(fromAddress);
                processPacketFromPlayer(clientProxy, packet);
            }
        }
    }


    /**
     * Process a packet from an unknown address.
     * @param packet The received packet.
     */
    private void processPacketFromNewClient(ReceivedPacket packet) {
        final ClientMessage msg = ClientMessage.getRootAsClientMessage(packet.getByteBuffer(), clientMessage);
        final int msgType = msg.msgType();

        if (msgType != ClientMessageType.ClientHello) {
            return;
        }

        msg.msg(clientHello);
        String playerName = clientHello.playerName();
        connectNewPlayer(packet.getFromAddress(), playerName);
    }


    /**
     * Handle a new client connection.
     * @param socketAddress The socket address of the new client.
     * @param playerName The client's player name.
     */
    private void connectNewPlayer(SocketAddress socketAddress, String playerName) {
        final ClientProxy clientProxy = getClientProxy(socketAddress, playerName);
        clientProxy.setLastInputTick(server.getTick());

        System.out.println("New player: " + playerName + " (" + socketAddress + ")");

        socketAddressClientProxyMap.put(socketAddress, clientProxy);
        sendServerHello(clientProxy);
    }


    /**
     * Get a ClientProxy for a new client.
     * @param socketAddress The address of the new client.
     * @param playerName The name of the new client.
     * @return The ClientProxy.
     */
    private ClientProxy getClientProxy(SocketAddress socketAddress, String playerName) {
        final Snake playerSnake = world.spawnSnake();

        int playerNetworkId = linkingContext.getNetworkId(playerSnake, true);
        System.out.println("Spawned new snake " + playerNetworkId);

        playerSnake.setName(playerName);

        return new ClientProxy(socketAddress, playerNetworkId, playerSnake);
    }


    /**
     * Sends a welcome message to a new client.
     * @param clientProxy The {@link ClientProxy} for the new client.
     */
    private void sendServerHello(ClientProxy clientProxy) {
        FlatBufferBuilder builder = new FlatBufferBuilder(1);

        ServerHello.startServerHello(builder);
        ServerHello.addClientId(builder, clientProxy.getClientId());
        int offsetServerHello = ServerHello.endServerHello(builder);

        ServerMessage.startServerMessage(builder);
        ServerMessage.addMsgType(builder, ServerMessageType.ServerHello);
        ServerMessage.addMsg(builder, offsetServerHello);
        int offsetServerMessage = ServerMessage.endServerMessage(builder);

        ServerMessage.finishServerMessageBuffer(builder, offsetServerMessage);
        ByteBuffer buf = builder.dataBuffer();

        try {
            channel.send(buf, clientProxy.getSocketAddress());
        } catch (IOException e) {
            System.out.println("ERROR: Failed to send Server Hello.");
            System.exit(-1);
        }
    }


    /**
     * Process a packet from a connected player client.
     * @param clientProxy The proxy for the client.
     * @param packet The received packet.
     */
    private void processPacketFromPlayer(ClientProxy clientProxy, ReceivedPacket packet) {
        final ClientMessage msg = ClientMessage.getRootAsClientMessage(packet.getByteBuffer(), clientMessage);
        final int msgType = msg.msgType();

        if (msg.clientId() != clientProxy.getClientId()) {
            return;
        }

        switch (msgType) {
            case ClientMessageType.ClientInputState:
                ClientInputState inputState = (ClientInputState) msg.msg(clientInputState);
                processInputStateMessage(clientProxy, inputState);
                break;
            case ClientMessageType.ClientGoodbye:
                ClientGoodbye goodbye = (ClientGoodbye) msg.msg(clientGoodbye);
                processGoodbyeMessage(clientProxy, goodbye);
                break;
        }
    }


    private Vec2 input = new Vec2();
    private Vector2 desiredMove = Vector2.zero();

    /**
     * Process new input from from a client.
     * @param clientProxy Client proxy.
     * @param inputState The input state received.
     */
    private void processInputStateMessage(ClientProxy clientProxy, ClientInputState inputState) {
        final int clientId = linkingContext.getNetworkId(clientProxy.getSnake(), true);

        int ticksSinceLastInput = clientProxy.getLastInputTick() != -1
                ? server.getTick() - clientProxy.getLastInputTick()
                : 1;
        float dt = ticksSinceLastInput * server.getTimeStep() / 1000.0f;

        input = inputState.desiredMove(input);
        desiredMove.set(input.x(), input.y());
        world.handleInput(clientId, inputState.isTurbo(), desiredMove, dt);

        clientProxy.setLastInputTick(server.getTick());
    }


    /**
     * Process Goodbye message form client.
     * @param goodbye The Goodbye message.
     */
    private void processGoodbyeMessage(ClientProxy clientProxy, ClientGoodbye goodbye) {
        System.out.println("Client disconnected (" + clientProxy.getPlayerName() + ")");
        disconnectClient(clientProxy);
    }


    /**
     * Send world replication packets out to clients.
     */
    public void sendOutgoingPackets(int tick) {
        for (ClientProxy clientProxy : socketAddressClientProxyMap.values()) {
            sendWorldState(tick, clientProxy);
        }
    }

    private Set<ClientProxy> clientsToDisconnect = new HashSet<>();
    public void checkTimeouts(int tick) {
        clientsToDisconnect.clear();

        for (ClientProxy clientProxy : socketAddressClientProxyMap.values()) {
            final int ticksSinceLastInputPacket = tick - clientProxy.getLastInputTick();
            if (ticksSinceLastInputPacket > TIMEOUT / server.getTimeStep()) {
                System.out.println("Player (" + clientProxy.getPlayerName() + ") timed out");
                clientsToDisconnect.add(clientProxy);
            }
        }

        for (ClientProxy clientProxy : clientsToDisconnect) {
            disconnectClient(clientProxy);
        }
    }
    
    
    private void disconnectClient(ClientProxy clientProxy) {
        world.killSnake(clientProxy.getSnake());
        socketAddressClientProxyMap.remove(clientProxy.getSocketAddress());
    }


    /**
     * Send the current world snapshot to a client.
     * @param clientProxy The client proxy.
     */
    private void sendWorldState(int tick, ClientProxy clientProxy) {
        FlatBufferBuilder builder = new FlatBufferBuilder(1);

        int offsetServerWorldState = world.serializeObjectStates(builder, tick, clientProxy);

        ServerMessage.startServerMessage(builder);
        ServerMessage.addMsgType(builder, ServerMessageType.ServerWorldState);
        ServerMessage.addMsg(builder, offsetServerWorldState);
        int offsetServerMessage = ServerMessage.endServerMessage(builder);

        ServerMessage.finishServerMessageBuffer(builder, offsetServerMessage);
        ByteBuffer buf = builder.dataBuffer();

        try {
            channel.send(buf, clientProxy.getSocketAddress());
        } catch (IOException e) {
            System.out.println("ERROR: Failed to send World State.");
            e.printStackTrace();
            System.exit(-1);
        }
    }
}
