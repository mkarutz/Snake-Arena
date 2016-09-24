package in.slyther.math;

import org.junit.Test;

import static org.junit.Assert.*;

/**
 * Created by mkarutz on 24/09/16.
 */
public class RectTest {
    @Test
    public void center() throws Exception {
        Rect rect = new Rect();
        rect.getMin().setX(0);
        rect.getMin().setY(0);
        rect.getMax().setX(10);
        rect.getMax().setY(10);

        assertEquals(0, rect.getMin().getX(), 0.01f);
        assertEquals(0, rect.getMin().getY(), 0.01f);
        assertEquals(10, rect.getMax().getX(), 0.01f);
        assertEquals(10, rect.getMax().getY(), 0.01f);

        rect.center(0, 0);

        assertEquals(-5.0f, rect.getMin().getX(), 0.01f);
        assertEquals(-5.0f, rect.getMin().getY(), 0.01f);
        assertEquals(5.0f, rect.getMax().getX(), 0.01f);
        assertEquals(5.0f, rect.getMax().getY(), 0.01f);
    }

    @Test
    public void width() throws Exception {
        Rect rect = new Rect();
        rect.getMin().setX(0);
        rect.getMin().setY(0);
        rect.getMax().setX(10);
        rect.getMax().setY(10);

        assertEquals(10, rect.getWidth(), 0.01f);
    }

    @Test
    public void height() throws Exception {
        Rect rect = new Rect();
        rect.getMin().setX(0);
        rect.getMin().setY(0);
        rect.getMax().setX(10);
        rect.getMax().setY(10);

        assertEquals(10, rect.getHeight(), 0.01f);
    }
}
