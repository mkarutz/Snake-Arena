using UnityEngine;
using System.Collections;

public class FoodView : MonoBehaviour {
    
    private FoodState foodState;
    private SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Awake () {
        this.foodState = this.GetComponent<FoodState>();
        this.spriteRenderer = this.gameObject.AddComponent<SpriteRenderer>();
        this.spriteRenderer.sprite = Resources.Load<Sprite>("FoodSprite");
        this.spriteRenderer.material.shader = Shader.Find("Particles/Additive");
        this.spriteRenderer.enabled = false;
    }

    void OnEnable()
    {
        this.spriteRenderer.enabled = true;
    }

    void OnDisable()
    {
        this.spriteRenderer.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        this.transform.localScale = Vector3.one * 0.05f * this.foodState.weight;

        foodState.color.a = 0.6f;
        spriteRenderer.material.SetColor("_TintColor", foodState.color);
    }
}
