using UnityEngine;
using System.Collections;

public class FoodView : MonoBehaviour {

    private FoodState foodState;
    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
        this.foodState = this.GetComponent<FoodState>();
        this.spriteRenderer = this.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        foodState.color.a = 0.3f;
        spriteRenderer.material.SetColor("_TintColor", foodState.color);
    }
}
