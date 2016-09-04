package in.slyther;

import java.io.IOException;
import java.net.DatagramSocket;
import java.net.InetSocketAddress;
import java.net.SocketException;
import java.nio.channels.DatagramChannel;

/**
 *
 */
public class Server extends Thread {
    private static final int DEFAULT_PORT = 3000;
    private static final int DEFAULT_TICK_RATE = 20;

    private final int udpPort;
    private final int timeStep;


    /**
     * Server constructor.
     * @param builder Builder Pattern for configuring server.
     */
    private Server(Builder builder) {
        this.udpPort = builder.udpPort;
        this.timeStep = 1000 / builder.tickRate;
    }


    /**
     * Server Builder.
     */
    public static class Builder {
        private int udpPort = DEFAULT_PORT;
        private int tickRate = DEFAULT_TICK_RATE;

        public Server build() {
            return new Server(this);
        }

        public Builder setUdpPort(int udpPort) {
            this.udpPort = udpPort;
            return this;
        }

        public Builder setTickRate(int tickRate) {
            this.tickRate = tickRate;
            return this;
        }
    }


    /**
     * Main Server Thread.
     */
    @Override
    public void run() {
        // Main tick loop
        long lastTickTime = 0;
        while (true) {
            long startTime = System.currentTimeMillis();

            // Read client messages
            networkManager.readPackets();

            // Simulate Tick
            world.tick();

            // Send snapshots
            networkManager.sendUpdates();

            // Sleep tick time
            long deltaTime = System.currentTimeMillis() - startTime;
            try {
                Thread.sleep(timeStep - deltaTime);
            } catch (InterruptedException e) {
                e.printStackTrace();
                System.exit(-1);
            }
        }
    }


    /**
     * Main Slyther.in server program.
     * @param args
     */
    public static void main(String[] args) {
        final Server server = new Builder().build();
        server.start();
    }
}
