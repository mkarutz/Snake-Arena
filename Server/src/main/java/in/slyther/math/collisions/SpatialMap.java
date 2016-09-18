package in.slyther.math.collisions;

import in.slyther.math.Rect;
import in.slyther.math.Vector2;

public interface SpatialMap<T> {
    void put(T obj, Vector2 pos);
    void put(T obj, Rect bound);
    void remove(T obj);
    void update(T obj, Vector2 pos);
    void update(T obj, Rect bound);
    Iterable<T> getNear(Vector2 pos);
    Iterable<T> getNear(Rect bound);
    boolean contains(T obj);
}
