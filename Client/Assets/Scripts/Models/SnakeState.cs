using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SnakeState : MonoBehaviour {

    public GameObject head;

    public float speedFactor;

    public int snakeSkinID;
    public int score;
    public string name;

    private IList<Vector3> backbonePoints = new List<Vector3>();

    // Use this for initialization
    void Start() {
        // Default backbone
        this.backbonePoints.Add(this.head.transform.position);
    }

    public Vector3 CalcBackboneParametizedPosition(float distance)
    {
        if (this.backbonePoints.Count == 0)
            return Vector3.zero;

        float accDistance = 0.0f;
        Vector3 prev = this.backbonePoints.Last();
        foreach (Vector3 curr in this.backbonePoints.Reverse())
        {
            Vector3 segmentVector = curr - prev;
            float segmentMagnitude = segmentVector.magnitude;
            if (accDistance + segmentMagnitude >= distance)
            {
                float t = distance - accDistance;
                return prev + segmentVector.normalized * t;
            }
            else
            {
                accDistance += segmentMagnitude;
            }
            prev = curr;
        }
        return Vector3.zero;
    }

    public IList<Vector3> getBackbone()
    {
        return this.backbonePoints;
    }

    public float GetSnakeLength()
    {
        return 1 + Mathf.Min(40000, this.score) / 100.0f;
    }

    public float GetSnakeThickness()
    {
        return 0.2f + Mathf.Sqrt(Mathf.Min(40000, this.score)) / 100.0f;
    }
}
