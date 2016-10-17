using UnityEngine;
using System.Collections;

public class SnakeSineBackbone : MonoBehaviour {

    public SnakeState snake;
    public int numBackbonePoints = 50;
    public float pointSpacing;
    public float amplitude;
    public float frequency;
    public float offsetFactor;
    public int lengthFactor;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < numBackbonePoints; i++)
            snake.AddBackboneHeadPoint(Vector2.zero);
        snake.score = lengthFactor;
	}
	
	// Update is called once per frame
	void Update () {
        UpdateBackbone();
        //this.transform.position = new Vector3(this.transform.parent.position.x, this.transform.parent.position.y, this.transform.position.z);
	}

    private void UpdateBackbone()
    {
        for (int i = 0; i < numBackbonePoints; i++)
        {
            snake.SetBackbonePoint(i, 
                new Vector2(-i * pointSpacing, Mathf.Sin((i * offsetFactor) + Time.time * frequency) * amplitude));
        }
    }
}
