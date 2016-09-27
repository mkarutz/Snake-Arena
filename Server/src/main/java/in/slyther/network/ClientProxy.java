package in.slyther.network;

import in.slyther.gameobjects.Snake;
import in.slyther.math.Rect;

import java.net.SocketAddress;

public class ClientProxy {
    private static final float VIEWPORT_SCALE = 10.0f;

    private SocketAddress socketAddress;
    private Snake snake;
    private Rect viewPortZone = new Rect();
    private int lastInputTick = -1;

    /**
     *
     * @param socketAddress
     * @param snake
     */
    public ClientProxy(SocketAddress socketAddress, Snake snake) {
        this.socketAddress = socketAddress;
        this.snake = snake;
    }


    /**
     * Get the client ID.
     * @return The client ID.
     */
    public int getClientId() {
        return snake.getPid();
    }


    /**
     *
     * @return
     */
    public SocketAddress getSocketAddress() {
        return socketAddress;
    }


    /**
     *
     * @param socketAddress
     */
    public void setSocketAddress(SocketAddress socketAddress) {
        this.socketAddress = socketAddress;
    }


    /**
     *
     * @return
     */
    public Snake getSnake() {
        return snake;
    }


    /**
     *
     * @param snake
     */
    public void setSnake(Snake snake) {
        this.snake = snake;
    }


    public Rect getViewportZone() {
        viewPortZone.setWidth(viewPortSize());
        viewPortZone.setHeight(viewPortSize());
        viewPortZone.center(snake.getHeadPosition().getX(), snake.getHeadPosition().getY());

        System.out.println("Player position: " + snake.getHeadPosition());
        System.out.println("Client viewport: { " + viewPortZone.getMin() + ", " + viewPortZone.getMax() + " }");

        return viewPortZone;
    }


    private float viewPortSize() {
        return snake.getThickness() * VIEWPORT_SCALE;
    }


    public int getLastInputTick() {
        return lastInputTick;
    }

    public void setLastInputTick(int lastInputTick) {
        this.lastInputTick = lastInputTick;
    }
}
