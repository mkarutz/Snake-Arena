using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SpatialHashMap<T> : SpatialMap<T>
{
    private Dictionary<T, HashSet<int>> bucketsMap = new Dictionary<T, HashSet<int>>();
    private HashSet<T>[] buckets;

    private Vector2 min;
    private Vector2 max;
    public float cellSize;

    public SpatialHashMap(Vector2 min,Vector2 max, float cellSize)
    {
        this.min = min;
        this.max = max;
        this.cellSize = cellSize;

        int nBucketX = (int)Math.Ceiling((max.x - min.x)/cellSize);
        int nBucketY = (int)Math.Ceiling((max.y - min.y) / cellSize);

        buckets = new HashSet<T>[nBucketX * nBucketY];
        for(int i = 0; i < (nBucketX * nBucketY); i++)
        {
            buckets[i] = new HashSet<T>();
        }
    }

    private void insert(T obj,float x,float y)
    {
        int bucketX = (int) ((x - min.x) / cellSize);
        int bucketY = (int) ((y - min.y) / cellSize);
        int bucketIndex = bucketX + (bucketY * ((int)Math.Ceiling((max.x - min.x) / cellSize)));
        buckets[bucketIndex].Add(obj);

        HashSet<int> indexSet = new HashSet<int>();
        if (!bucketsMap.ContainsKey(obj))
        {
            indexSet.Add(bucketIndex);
            bucketsMap.Add(obj, indexSet);
        }
        else
        {
            bucketsMap[obj].Add(bucketIndex);
        }
            
    }

    public bool contains(T obj)
    {
        return bucketsMap.ContainsKey(obj);
    }

    private HashSet<T> getNear(float x, float y)
    {
        int bucketX = (int)((x - min.x) / cellSize);
        int bucketY = (int)((y - min.y) / cellSize);
        int bucketIndex = bucketX + (bucketY * ((int)Math.Ceiling((max.x - min.x) / cellSize)));
        return buckets[bucketIndex];
    }

    public IEnumerable<T> getNear(Rect bound)
    {
        HashSet<T> result = new HashSet<T>();
        result.UnionWith(getNear(bound.xMax, bound.yMax));
        result.UnionWith(getNear(bound.xMax, bound.yMin));
        result.UnionWith(getNear(bound.xMin, bound.yMax));
        result.UnionWith(getNear(bound.xMin, bound.yMin));
        return result;
    }

    public IEnumerable<T> getNear(Vector2 pos)
    {
        return getNear(pos.x, pos.y);
    }

    public void put(T obj, Rect bound)
    {
        remove(obj);
        insert(obj, bound.xMax, bound.yMax);
        insert(obj, bound.xMax, bound.yMin);
        insert(obj, bound.xMin, bound.yMax);
        insert(obj, bound.xMin, bound.yMin);

    }

    public void put(T obj, Vector2 pos)
    {
        remove(obj);
        insert(obj, pos.x, pos.y);
    }

    public void remove(T obj)
    {
        if (!contains(obj))
        {
            return;
        }

        foreach(int i in bucketsMap[obj])
        {
            buckets[i].Remove(obj);
        }

        bucketsMap.Remove(obj);        
    }

    public void update(T obj, Rect bound)
    {
        put(obj, bound);
    }

    public void update(T obj, Vector2 pos)
    {
        put(obj, pos);
    }
}
