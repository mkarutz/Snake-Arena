package in.slyther.network;

import in.slyther.gameobjects.Snake;
import in.slyther.math.Rect;

import java.net.SocketAddress;

public class ClientProxy {
    private SocketAddress socketAddress;
    private Snake snake;


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
        // TODO
        return null;
    }
}
