using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using slyther.flatbuffers;

public class GameState : MonoBehaviour 
{
	public int MAX_SNAKES = 100;
	public int MAX_FOODS = 100;
	public int WORLD_RADIUS = 500;

	public SnakeState snakePrefab;
	public FoodState foodPrefab;

    private SnakeState[] snakePool;
    private FoodState[] foodPool;

    private int maxSnakes;
    private int maxFoods;
    private float worldRadius;


    void Awake() 
	{
		InitState(MAX_SNAKES, MAX_FOODS, WORLD_RADIUS);
	}


    public void InitState(int maxSnakes, int maxFoods, float worldRadius)
    {
		this.maxSnakes = maxSnakes;
		this.maxFoods = maxFoods;
		this.worldRadius = worldRadius;

		InitSnakes();
		InitFood();
    }


	private void InitSnakes()
	{
		this.snakePool = new SnakeState[this.maxSnakes];

		for (int i = 0; i < this.maxSnakes; i++)
		{
			snakePool[i] = GameObject.Instantiate(snakePrefab);
		}
	}


	private void InitFood()
	{
		this.foodPool = new FoodState[this.maxFoods];

		for (int i = 0; i < this.maxFoods; i++)
		{
			foodPool[i] = GameObject.Instantiate(foodPrefab);
		}
	}


	public SnakeState GetSnake(int snakeId)
	{
		return snakePool[snakeId];
	}


    public FoodState GetFood(int foodID)
    {
        return foodPool[foodID];
    }

    public bool IsFoodActive(int foodID)
    {
        return foodPool[foodID].enabled;
    }

    public bool IsSnakeActive(int snakeID)
    {
        return snakePool[snakeID].enabled;
    }

    public void DeactivateFood(int foodID)
    {
        foodPool[foodID].enabled = false;
        foodPool[foodID].GetComponent<FoodView>().enabled = false;

        Destroy(foodPool[foodID].GetComponent<IController>().getControllerComponent());
    }
}
