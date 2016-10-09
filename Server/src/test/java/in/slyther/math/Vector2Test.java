package in.slyther.math;

import org.junit.Test;

import static org.junit.Assert.*;

public class Vector2Test {
    @Test
    public void testAngleBetween() throws Exception {
        Vector2 from = new Vector2(1, 1);
        Vector2 to = new Vector2(1, -1);
        float delta = Vector2.angleBetween(from, to);
        assertEquals((float) -Math.PI / 2.0f, delta, 0.01f);
    }

    @Test
    public void testAngleBetween2() throws Exception {
        Vector2 from = new Vector2(1, -1);
        Vector2 to = new Vector2(1, -1);
        float delta = Vector2.angleBetween(from, to);
        assertEquals(0, delta, 0.01f);
    }

    @Test
    public void testAngleBetween3() throws Exception {
        Vector2 from = new Vector2(1, 0);
        Vector2 to = new Vector2(-1, 0);
        float delta = Vector2.angleBetween(from, to);
        assertEquals(-Math.PI, delta, 0.01f);
    }

    @Test
    public void testRotateTowards() throws Exception {
        Vector2 from = new Vector2(1, 1);
        Vector2 to = new Vector2(1, -1);

        from.rotateTowards(to, 10.0f);

        assertEquals(0.0f, Vector2.angleBetween(from, to), 0.01f);
    }

    @Test
    public void testRotateTowards2() throws Exception {
        Vector2 from = new Vector2(1, 1);
        Vector2 to = new Vector2(1, -1);

        from.rotateTowards(to, (float) Math.PI / 4);

        assertEquals(-Math.PI / 4, Vector2.angleBetween(from, to), 0.01f);
    }

    @Test
    public void testIsPerpendicularTo() {
        Vector2 A = new Vector2(0, 0);
        Vector2 B = new Vector2 (1, 1);
        Vector2 C = new Vector2(0.5f, 1);

        assertTrue(Vector2.isPerpendicularToSegment(C, A, B));

        C.set(1, 0);
        assertTrue(Vector2.isPerpendicularToSegment(C, A, B));

        C.set(-1, -1);
        assertTrue(!Vector2.isPerpendicularToSegment(C, A, B));

        C.set(0, -1);
        assertTrue(!Vector2.isPerpendicularToSegment(C, A, B));
    }
}
