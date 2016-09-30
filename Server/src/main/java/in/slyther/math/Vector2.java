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

    public Vector2(Vector2 other) {
        this(other.x, other.y);
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

    @Override
    public String toString() {
        return "(" + x + ", " + y + ")";
    }

    public static Vector2 zero() {
        return new Vector2(0, 0);
    }

    public static Vector2 up() {
        return new Vector2(0, 1);
    }


    public Vector2 set(Vector2 other) {
        x = other.x;
        y = other.y;
        return this;
    }


    public void lerpTo(Vector2 other, float lerp) {
        x += lerp * (other.x - x);
        y += lerp * (other.y - y);
    }


    public static float distance(Vector2 a, Vector2 b) {
        final float dx = a.x - b.x;
        final float dy = a.y - b.y;
        return (float) Math.sqrt(dx * dx + dy * dy);
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


    public void translate(float x, float y) {
        this.x += x;
        this.y += y;
    }

    public static Vector2 minus(Vector2 a, Vector2 b) {
        Vector2 result = new Vector2(a.x, a.y);
        return result.subtract(b);
    }


    public Vector2 add(Vector2 v) {
        translate(v.x, v.y);
        return this;
    }

    public Vector2 subtract(Vector2 v) {
        return add(v.multiply(-1));
    }

    public Vector2 multiply(float k) {
        this.x *= k;
        this.y *= k;
        return this;
    }

    public Vector2 divide(float k) {
        return multiply(1.0f / k);
    }

    public Vector2 normalize() {
        return divide(magnitude());
    }

    public float magnitude() {
        return (float) Math.sqrt(x*x + y*y);
    }

    public Vector2 rotateTowards(Vector2 other, float maxDegrees) {
        float rotateAngle = angleBetween(this, other);
        int sign = rotateAngle < 0 ? -1 : 1;
        rotateAngle = sign * Math.min(sign * rotateAngle, Math.abs(maxDegrees));

        float newX = (float) (Math.cos(rotateAngle) * x - Math.sin(rotateAngle) * y);
        float newY = (float) (Math.sin(rotateAngle) * x + Math.cos(rotateAngle) * y);
        x = newX;
        y = newY;
        return this;
    }

    public static float angleBetween(Vector2 from, Vector2 to) {
        final float toTheta = (float) Math.atan2(to.getY(), to.getX());
        final float fromTheta = (float) Math.atan2(from.getY(), from.getX());

        float delta = toTheta - fromTheta;
        int sign = delta < 0 ? -1 : 1;

        delta *= sign;

        if (delta > Math.PI) {
            delta = (float) (2 * Math.PI - delta);
            sign *= -1;
        }

        return sign * delta;
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
