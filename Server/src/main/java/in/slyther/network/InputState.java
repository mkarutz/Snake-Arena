package in.slyther.network;

import in.slyther.math.Vector2;

public class InputState {
    private Vector2 desiredMove;
    private boolean isTurbo;

    public InputState(Vector2 desiredMove, boolean isTurbo) {
        this.desiredMove = desiredMove;
        this.isTurbo = isTurbo;
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

