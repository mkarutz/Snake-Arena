using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface SpatialMap<T> {
    void put(T obj, Vector2 pos);
    void put(T obj, Rect bound);
    void remove(T obj);
    void update(T obj, Vector2 pos);
    void update(T obj, Rect bound);
    IEnumerable<T> getNear(Vector2 pos);
    IEnumerable<T> getNear(Rect bound);
    bool contains(T obj);
}
