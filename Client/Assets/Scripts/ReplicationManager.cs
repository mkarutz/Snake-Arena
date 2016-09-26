using UnityEngine;
using System.Collections;
using slyther.flatbuffers;

public class ReplicationManager : MonoBehaviour {
	public const int MAX_SNAKES = 100;
	public const int MAX_FOOD = 100;

	private Snake[] snakes = new Snake[MAX_SNAKES];


	private NetworkObjectState networkObjectState = new NetworkObjectState();
	private SnakeStateFB snakeStateFB = new SnakeStateFB();

	public void replicateState(ServerWorldState serverWorldState) {
		for (int i = 0; i < serverWorldState.ObjectStatesLength; i++) {
			networkObjectState = serverWorldState.GetObjectStates(networkObjectState, i);

			switch (networkObjectState.StateType) {
			case NetworkObjectStateType.SnakeStateFB:
				snakeStateFB = networkObjectState.GetState<SnakeStateFB> (snakeStateFB);
				int snakeID = snakeStateFB.PlayerId;
				snakes[snakeID].replicateState(snakeStateFB);
				break;
			}
		}
	}
		
	void Start () {
	
	}

	void Update () {
	
	}
}
