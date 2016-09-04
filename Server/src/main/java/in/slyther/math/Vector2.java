package in.slyther.math;

import java.util.Random;

public class Vector2 {
    private static final Random random = new Random();
    private float x;
    private float y;

    public Vector2(float x, float y) {
        this.x = x;
        this.y = y;
    }

    public static Vector2 zero() {
        return new Vector2(0, 0);
    }


    /**
     * x^2 + y^2 < bound^2
     * y < sqrt(bound^2 - x^2)
     * @param bound Upper bound (exclusive) for magnitude of Vector.
     * @return Vector2 with magnitude less than bound.
     */
    public static Vector2 randomUniform(float bound) {
        final float x = random.nextFloat() * bound;
        final float y = random.nextFloat() * (float) Math.sqrt(bound * bound - x * x);
        return new Vector2(x, y);
    }


    public float getX() {
        return x;
    }

    public void setX(float x) {
        this.x = x;
    }

    public float getY() {
        return y;
    }

    public void setY(float y) {
        this.y = y;
    }
}
