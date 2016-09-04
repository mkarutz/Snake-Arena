package in.slyther;

import in.slyther.math.Vector2;

public class UserCommand {
    private int playerId;
    private Vector2 desiredMove;
    private boolean isTurbo;

    public UserCommand(int playerId, Vector2 desiredMove, boolean isTurbo) {
        this.playerId = playerId;
        this.desiredMove = desiredMove;
        this.isTurbo = isTurbo;
    }

    public int getPlayerId() {
        return playerId;
    }

    public void setPlayerId(int playerId) {
        this.playerId = playerId;
    }

    public Vector2 getDesiredMove() {
        return desiredMove;
    }

    public void setDesiredMove(Vector2 desiredMove) {
        this.desiredMove = desiredMove;
    }

    public boolean isTurbo() {
        return isTurbo;
    }

    public void setTurbo(boolean turbo) {
        isTurbo = turbo;
    }
}
