using UnityEngine;
using System.Collections;
using slyther.flatbuffers;

public class ReplicationManager : MonoBehaviour {
	public GameState gameState;

	public void ReplicateSnake(NetworkSnakeState snakeState)
	{
		SnakeState snake = gameState.GetSnake(snakeState.PlayerId);
		snake.ReplicateState(snakeState);
	}

	public void ReplicateFood(NetworkFoodState foodState)
	{
		FoodState food = gameState.GetFood(foodState.FoodId);
		food.ReplicateState(foodState);
	}
}
