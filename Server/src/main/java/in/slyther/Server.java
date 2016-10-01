package in.slyther;

import in.slyther.network.NetworkManager;

import java.io.IOException;


/**
 *
 */
public class Server extends Thread {
    private static final int DEFAULT_PORT = 3000;
    private static final int DEFAULT_TICK_RATE = 20;

    private final int timeStep;
    private final NetworkManager networkManager;
    private final World world;


    /**
     * Server constructor.
     * @param builder Builder Pattern for configuring server.
     */
    private Server(Builder builder) {
        this.timeStep = 1000 / builder.tickRate;
        this.world = new World(this);
        this.networkManager = new NetworkManager(this, world, builder.udpPort);
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
     * Get the time-step of each tick.
     * @return the time-step
     */
    public int getTimeStep() {
        return timeStep;
    }


    private int tick;

    public int getTick() {
        return tick;
    }


    /**
     * Main Server Thread.
     */
    @Override
    public void run() {
        System.out.println("Starting server.");

        world.initWorld();

        try {
            networkManager.bind();
        } catch (IOException e) {
            e.printStackTrace();
            System.exit(-1);
        }

        tick = 0;
        long lastTickTime = 0;
        while (true) {
            long startTime = System.currentTimeMillis();

            networkManager.handleIncomingPackets(tick);
            networkManager.checkTimeouts(tick);

            world.simulate(tick);

            networkManager.sendOutgoingPackets(tick);

            long deltaTime = System.currentTimeMillis() - startTime;
            try {
                if (deltaTime < timeStep) {
                    Thread.sleep(timeStep - deltaTime);
                } else {
                    System.out.println("Time step exceeded.");
                }
            } catch (InterruptedException e) {
                e.printStackTrace();
                System.exit(-1);
            }

            tick++;
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
