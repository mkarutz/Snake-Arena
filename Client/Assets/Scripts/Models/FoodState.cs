using UnityEngine;
using System.Collections;
using slyther.flatbuffers;

public class FoodState : MonoBehaviour 
{
    public Color color;
    public int weight;
    public bool collected = false;

	void Awake()
	{
		Despawn();
	}

	public void Despawn()
	{
		gameObject.SetActive(false);
	}

	public void Respawn()
	{
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
