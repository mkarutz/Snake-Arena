package in.slyther.network;

import in.slyther.gameobjects.Snake;
import in.slyther.math.Rect;

import java.net.SocketAddress;

public class ClientProxy {
    private static final float VIEWPORT_SCALE = 30.0f;

    private SocketAddress socketAddress;
    private int clientId;
    private Snake snake;
    private Rect viewPortZone = new Rect();
    private int lastInputTick = -1;

    /**
     *
     * @param socketAddress
     * @param snake
     */
    public ClientProxy(SocketAddress socketAddress, int clientId, Snake snake) {
        this.socketAddress = socketAddress;
        this.clientId = clientId;
        this.snake = snake;
    }


    /**
     * Get the client ID.
     * @return The client ID.
     */
    public int getClientId() {
        return clientId;
    }


    public String getPlayerName() {
        return snake.getName();
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

        return viewPortZone;
    }


    private float viewPortSize() {
        return snake.getThickness() * VIEWPORT_SCALE * 1.2f;
    }


    public int getLastInputTick() {
        return lastInputTick;
    }

    public void setLastInputTick(int lastInputTick) {
        this.lastInputTick = lastInputTick;
    }
}
