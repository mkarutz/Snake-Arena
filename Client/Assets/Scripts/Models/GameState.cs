using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour {

    // Public/exposed attributes
    public GameObject snakeTemplate;
    public GameObject foodTemplate;

    // Private attributes
    private SnakeState[] snakePool;
    private FoodState[] foodPool;

    private int maxSnakes;
    private int maxFoods;
    private float worldRadius;

    // Use this for initialization
    void Start () {
	    
	}

    void InitState(int maxSnakes, int maxFoods, float worldRadius)
    {
        this.maxSnakes = maxSnakes;
        this.maxFoods = maxFoods;
        this.worldRadius = worldRadius;

        this.snakePool = new SnakeState[this.maxSnakes];
        for (int i = 0; i < this.maxSnakes; i++)
        {
            GameObject newSnake = Instantiate<GameObject>(snakeTemplate);
            snakePool[i] = newSnake.GetComponent<SnakeState>();
        }

        this.foodPool = new FoodState[this.maxFoods];
        for (int i = 0; i < this.maxFoods; i++)
        {
            GameObject newFood = Instantiate<GameObject>(foodTemplate);
            snakePool[i] = newFood.GetComponent<SnakeState>();
        }
    }
	
	
}
