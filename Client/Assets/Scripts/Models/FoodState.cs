using UnityEngine;
using System.Collections;
using slyther.flatbuffers;

public class FoodState : MonoBehaviour 
{
	public SpriteRenderer spriteRenderer;
    public int weight;
    public bool collected = false;
    public float scaleFactor = 0.0f;
    public float hue = 0.0f;

    public SnakeState collectingSnake;

    void Start()
	{
        //spriteRenderer.material.SetColor("_TintColor", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f, 0.6f, 0.6f));
        //hue = Random.Range(0.0f, 1.0f);
        spriteRenderer.color = Random.ColorHSV(0f, 1f, 1f, 1f, 1f, 1f, 1.0f, 1.0f);
        UpdateScale();
    }

	void Update()
	{
        UpdateScale();
	}

    public void UpdateScale()
    {
        this.transform.localScale = (new Vector3(1.0f, 1.0f, 1.0f) * 0.1f * weight * scaleFactor) + Vector3.right * 0.001f;
        //this.transform.localScale += Vector3.forward * Random.Range(0.0f, 1.0f);
        //this.transform.position += Vector3.forward * hue;
    }

    public int CollectFood(SnakeState collectingSnake)
    {
        this.collectingSnake = collectingSnake;
        this.collected = true;
        return this.weight;
    }

    public void ReplicateState(NetworkFoodState state)
	{
		this.weight = state.Weight;
		this.transform.position = new Vector2(state.Position.X, state.Position.Y);
        //this.transform.position += Vector3.forward * hue;

    }
}
