using UnityEngine;
using System.Collections;
using slyther.flatbuffers;

public class FoodState : MonoBehaviour 
{
	public SpriteRenderer spriteRenderer;
    public int weight;
    public bool collected = false;

	void Start()
	{
		spriteRenderer.material.SetColor("_TintColor", Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f, 0.6f, 0.6f));
	}

	void Update()
	{
		this.transform.localScale = Vector3.one * 0.1f * weight;
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
