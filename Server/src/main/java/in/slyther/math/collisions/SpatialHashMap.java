package in.slyther.math.collisions;

import com.google.common.collect.HashMultimap;
import com.google.common.collect.Multimap;
import in.slyther.math.Rect;
import in.slyther.math.Vector2;

import java.util.HashSet;
import java.util.Set;

public class SpatialHashMap<T> implements SpatialMap<T> {
    private final Multimap<T,Set<T>> bucketsMap = HashMultimap.create();
    private final Set<T>[][] buckets;

    private final Vector2 min;
    private final Vector2 max;
    private final float cellSize;


    @SuppressWarnings("unchecked")
    public SpatialHashMap(Vector2 min, Vector2 max, float cellSize) {
        this.min = min;
        this.max = max;
        this.cellSize = cellSize;

        int nBucketsX = (int) Math.ceil((max.getX() - min.getX()) / cellSize);
        int nBucketsY = (int) Math.ceil((max.getY() - min.getY()) / cellSize);

        buckets = (Set<T>[][]) new Set[nBucketsX][nBucketsY];

        for (int i = 0; i < nBucketsX; i++) {
            for (int j = 0; j < nBucketsY; j++) {
                buckets[i][j] = new HashSet<T>();
            }
        }
    }

    
    private void insert(T obj, float x, float y) {
        int bucketX = (int) ((x - min.getX()) / cellSize);
        int bucketY = (int) ((y - min.getY()) / cellSize);
        buckets[bucketX][bucketY].add(obj);
        bucketsMap.put(obj, buckets[bucketX][bucketY]);
    }


    @Override
    public void put(T obj, Vector2 pos) {
        remove(obj);
        insert(obj, pos.getX(), pos.getY());
    }


    @Override
    public void put(T obj, Rect bound) {
        remove(obj);
        insert(obj, bound.getMin().getX(), bound.getMin().getY());
        insert(obj, bound.getMin().getX(), bound.getMax().getY());
        insert(obj, bound.getMax().getX(), bound.getMin().getY());
        insert(obj, bound.getMax().getX(), bound.getMax().getY());
    }


    @Override
    public void remove(T obj) {
        if (!contains(obj)) {
            return;
        }

        for (Set<T> bucket : bucketsMap.get(obj)) {
            bucket.remove(obj);
        }

        bucketsMap.removeAll(obj);
    }


    @Override
    public void update(T obj, Vector2 pos) {
        put(obj, pos);
    }


    @Override
    public void update(T obj, Rect bound) {
        put(obj, bound);
    }


    private Set<T> getNear(float x, float y) {
        int bucketX = (int) ((x - min.getX()) / cellSize);
        int bucketY = (int) ((y - min.getY()) / cellSize);
        return buckets[bucketX][bucketY];
    }


    @Override
    public Iterable<T> getNear(Vector2 pos) {
        return getNear(pos.getX(), pos.getY());
    }


    @Override
    public Iterable<T> getNear(Rect bound) {
        Set<T> result = new HashSet<T>();
        result.addAll(getNear(bound.getMin().getX(), bound.getMin().getY()));
        result.addAll(getNear(bound.getMin().getX(), bound.getMax().getY()));
        result.addAll(getNear(bound.getMax().getX(), bound.getMin().getY()));
        result.addAll(getNear(bound.getMax().getX(), bound.getMax().getY()));
        return result;
    }


    public boolean contains(T obj) {
        return bucketsMap.containsKey(obj);
    }
}
