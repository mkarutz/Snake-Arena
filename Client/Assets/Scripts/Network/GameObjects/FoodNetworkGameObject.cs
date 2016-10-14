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
        // Find closest snake
        // This is a hack
        GameObject[] snakes = GameObject.FindGameObjectsWithTag("Snake");
        GameObject closestSnake = GameObject.FindGameObjectWithTag("Player");
        float minDistance = Vector2.Distance(this.food.transform.position, closestSnake.transform.position);
        foreach (GameObject snake in snakes)
        {
            if (snake.activeInHierarchy)
            {
                float dist = Vector2.Distance(this.food.transform.position, snake.transform.position);
                if (dist < minDistance)
                {
                    dist = minDistance;
                    closestSnake = snake;
                }
            }
        }
        SnakeState closestSnakeState = closestSnake.GetComponent<SnakeState>();
        if (minDistance < closestSnakeState.GetSnakeThickness() * GameConfig.EAT_DISTANCE_RATIO * 2.0f)
            food.CollectFood(closestSnakeState);
        else
            Destroy(this.gameObject);
	}
}
