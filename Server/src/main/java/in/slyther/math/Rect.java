package in.slyther.math;

public class Rect {
    private Vector2 min;
    private Vector2 max;


    /**
     * Returns a rectangle with unit edge length centered
     * at the origin.
     * @return A new {@link Rect}
     */
    public static Rect unit() {
        return new Rect(new Vector2(-0.5f, -0.5f), new Vector2(0.5f, 0.5f));
    }


    public Rect(Vector2 min, Vector2 max) {
        this.min = min;
        this.max = max;
    }


    public void setMaxX(float x) {
        max.setX(x);

        if (min.getX() > x) {
            min.setX(x);
        }
    }


    public void setMaxY(float y) {
        max.setY(y);

        if (min.getY() > y) {
            min.setY(y);
        }
    }


    public void setMinY(float y) {
        min.setY(y);

        if (max.getY() < y) {
            max.setY(y);
        }
    }


    public void setMinX(float x) {
        min.setX(x);

        if (max.getX() < x) {
            max.setX(x);
        }
    }

    public Vector2 getMin() {
        return min;
    }


    public void setMin(Vector2 min) {
        this.min = min;
    }

    public Vector2 getMax() {
        return max;
    }

    public void setMax(Vector2 max) {
        this.max = max;
    }
}
