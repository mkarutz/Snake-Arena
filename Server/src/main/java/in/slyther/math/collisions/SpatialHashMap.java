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


    /**
     * Inserts an object at the given spatial coordinates.
     * @param obj The object.
     * @param x X coordinate.
     * @param y Y coordinate.
     */
    private void insert(T obj, float x, float y) {
        int bucketX = toBucketX(x);
        int bucketY = toBucketY(y);
        addToBucket(obj, bucketX, bucketY);
    }


    /**
     * Adds an object to a bucket.
     * @param obj The object.
     * @param bucketX The horizontal index of the bucket.
     * @param bucketY The vertical index of the bucket.
     */
    private void addToBucket(T obj, int bucketX, int bucketY) {
        buckets[bucketX][bucketY].add(obj);
        bucketsMap.put(obj, buckets[bucketX][bucketY]);
    }


    /**
     * Converts a spatial coordinate to a bucket index.
     * @param x Horizontal component of the spatial coordinate.
     * @return The bucket index.
     */
    private int toBucketX(float x) {
        return (int) ((x - min.getX()) / cellSize);
    }


    /**
     * Converts a spatial coordinate to a bucket index.
     * @param y Vertical component of the spatial coordinate.
     * @return The bucket index.
     */
    private int toBucketY(float y) {
        return (int) ((y - min.getY()) / cellSize);
    }


    /**
     * Adds an object with at the given position.
     * @param obj The object.
     * @param pos The spatial coordinates of the object.
     */
    @Override
    public void put(T obj, Vector2 pos) {
        remove(obj);
        insert(obj, pos.getX(), pos.getY());
    }


    /**
     * Adds an object bounded by the given rectangle.
     * @param obj The object.
     * @param bound The bounding rectangle.
     */
    @Override
    public void put(T obj, Rect bound) {
        remove(obj);

        int minBucketX = toBucketX(bound.getMin().getX());
        int minBucketY = toBucketY(bound.getMin().getY());
        int maxBucketX = toBucketX(bound.getMax().getX());
        int maxBucketY = toBucketY(bound.getMax().getY());

        for (int i = minBucketX; i <= maxBucketX; i++) {
            for (int j = minBucketY; j <= maxBucketY; j++) {
                addToBucket(obj, i, j);
            }
        }
    }


    /**
     * Removes an object the from the map.
     * @param obj The object.
     */
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


    /**
     * Updates the position of an object.
     * @param obj The object.
     * @param pos The new position.
     */
    @Override
    public void update(T obj, Vector2 pos) {
        put(obj, pos);
    }


    /**
     * Updates the bounding rectangle of an Object.
     * @param obj The object.
     * @param bound The new bounding rectangle.
     */
    @Override
    public void update(T obj, Rect bound) {
        put(obj, bound);
    }


    /**
     * Gets all the objects in the bucket of the given coordinates.
     * @param x The x coordinate.
     * @param y The y coordinate.
     * @return Set of objects in the bucket.
     */
    private Set<T> getNear(float x, float y) {
        int bucketX = toBucketX(x);
        int bucketY = toBucketY(y);
        return buckets[bucketX][bucketY];
    }


    /**
     * Query the objects near a position.
     * @param pos Spatial position.
     * @return Iterable of all the objects near the position.
     */
    @Override
    public Iterable<T> getNear(Vector2 pos) {
        return getNear(pos.getX(), pos.getY());
    }


    /**
     * Query the objects near a bounding rectangle.
     * @param bound The bounding rectangle.
     * @return Iterable of all the objects near the rectangle.
     */
    @Override
    public Iterable<T> getNear(Rect bound) {
        Set<T> result = new HashSet<T>();

        int minBucketX = toBucketX(bound.getMin().getX());
        int minBucketY = toBucketY(bound.getMin().getY());
        int maxBucketX = toBucketX(bound.getMax().getX());
        int maxBucketY = toBucketY(bound.getMax().getY());

        for (int i = minBucketX; i <= maxBucketX; i++) {
            for (int j = minBucketY; j <= maxBucketY; j++) {
                result.addAll(buckets[i][j]);
            }
        }

        return result;
    }


    /**
     * Returns true if the map contains the object.
     * @param obj The object.
     * @return True if the object is present, else false.
     */
    @Override
    public boolean contains(T obj) {
        return bucketsMap.containsKey(obj);
    }
}
