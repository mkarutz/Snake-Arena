using UnityEngine;
using System.Collections;

public class FoodView : MonoBehaviour {

    private FoodState foodState;
    //private SpriteRenderer spriteRenderer;
    private MeshRenderer meshRenderer;

    // Use this for initialization
    void Start () {
        this.foodState = this.GetComponent<FoodState>();
        //this.spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.meshRenderer = this.GetComponent<MeshRenderer>();
    }
	
	// Update is called once per frame
	void Update () {
        foodState.color.a = 0.6f;
        meshRenderer.material.SetColor("_TintColor", foodState.color);
    }
}
