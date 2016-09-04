using UnityEngine;
using System.Collections;

public class FoodState : MonoBehaviour {

    public Vector2 position;
    public Color color;
    public int weight;

	// Use this for initialization
	void Start () {
        this.color = Color.HSVToRGB(Random.Range(0.0f, 1.0f), 1.0f, 1.0f);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
