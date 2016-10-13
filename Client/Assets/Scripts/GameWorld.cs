using UnityEngine;
using System.Collections;

public class GameWorld : MonoBehaviour {

    public float worldRadius = GameConfig.WORLD_RADIUS;
    private Plane worldPlane;

    void Start()
    {
        this.worldPlane = new Plane(Vector3.back, Vector3.zero);
    }

    void Update()
    {
        this.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * worldRadius * 2.0f;
    }

	public Vector2 GenerateRandomWorldPoint()
	{
		return this.GenerateRandomWorldPoint (0.0f, this.worldRadius);
	}

	public Vector2 GenerateRandomWorldPoint(float minRadius, float maxRadius)
	{
		float angle = Random.Range(0.0f, 360.0f);
		float dist = Random.Range(minRadius, maxRadius);
		Vector2 pos = Quaternion.AngleAxis(angle, Vector3.back) * (Vector3.right * dist);
		return pos;
	}

    public Plane GetWorldPlane()
    {
        return this.worldPlane;
    }
}
