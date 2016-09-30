using UnityEngine;
using System.Collections;
using slyther.flatbuffers;

public class FoodNetworkGameObject : INetworkGameObject {
	public FoodState food;


	public override void Replicate(NetworkObjectState state)
	{
		NetworkFoodState foodState = new NetworkFoodState();
		state.GetState(foodState);
		food.ReplicateState(foodState);
	}


	public override void Destroy()
	{
		GameObject.Destroy(gameObject);
	}
}
