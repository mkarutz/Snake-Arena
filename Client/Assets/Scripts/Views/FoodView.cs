using UnityEngine;
using System.Collections;

public class FoodView : MonoBehaviour 
{
    public float scaleFactor = 1;

    public FoodState foodState;
    public SpriteRenderer spriteRenderer;

	void Update () 
	{
		//this.transform.localScale = Vector3.one * 0.05f * this.foodState.weight * this.scaleFactor;
		//foodState.color.a = 0.6f;
		//spriteRenderer.material.SetColor("_TintColor", foodState.color);
	}

    void OnEnable()
    {
        this.spriteRenderer.enabled = true;
    }

    void OnDisable()
    {
        this.spriteRenderer.enabled = false;
    }
}
