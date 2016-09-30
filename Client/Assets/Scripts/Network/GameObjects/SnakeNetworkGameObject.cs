using UnityEngine;
using System.Collections;
using slyther.flatbuffers;

public class SnakeNetworkGameObject : INetworkGameObject {
	public SnakeState snake;


	public override void Replicate(NetworkObjectState state)
	{
		NetworkSnakeState snakeState = new NetworkSnakeState();
		state.GetState(snakeState);
		snake.ReplicateState(snakeState);
	}


	public override void Destroy()
	{
		GameObject.Destroy(gameObject);
	}
}
