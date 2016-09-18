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

    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (o == null || getClass() != o.getClass()) return false;

        Vector2 vector2 = (Vector2) o;

        return Float.compare(vector2.x, x) == 0 && Float.compare(vector2.y, y) == 0;
    }

    @Override
    public int hashCode() {
        int result = (x != +0.0f ? Float.floatToIntBits(x) : 0);
        result = 31 * result + (y != +0.0f ? Float.floatToIntBits(y) : 0);
        return result;
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
        final Vector2 result = Vector2.zero();
        result.setRandomUniform(bound);
        return result;
    }


    /**
     * Sets this Vector2 to a uniformly random position in a circle of
     * radius bound.
     * x^2 + y^2 < bound^2
     * y < sqrt(bound^2 - x^2)
     * @param bound Upper bound (exclusive) for magnitude of Vector.
     */
    public void setRandomUniform(float bound) {
        x = random.nextFloat() * 2 * bound - bound;
        final float yBound = (float) Math.sqrt(bound * bound - x * x);
        y = random.nextFloat() * 2 * yBound - yBound;
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
