using UnityEngine;
using System.Collections;
using slyther.flatbuffers;

public class FoodState : MonoBehaviour 
{
	public SpriteRenderer spriteRenderer;
    public int weight;
    public bool collected = false;
    public float scaleFactor = 0.0f;

    public SnakeState collectingSnake;

    void Start()
	{
		spriteRenderer.material.SetColor("_TintColor", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f, 0.6f, 0.6f));
    }

	void Update()
	{
		this.transform.localScale = Vector3.one * 0.1f * weight * scaleFactor;
	}

	public void Despawn()
	{
		gameObject.SetActive(false);
	}

	public void Respawn()
	{
		transform.localScale = Vector3.one * ((float) weight / 10.0f);
		gameObject.SetActive(true);
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

		if (state.IsActive) {
			Respawn();
		} else {
			Despawn();
		}
	}
}
