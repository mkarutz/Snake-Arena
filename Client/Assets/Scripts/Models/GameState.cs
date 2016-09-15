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

    public void InitState(int maxSnakes, int maxFoods, float worldRadius)
    {
        this.maxSnakes = maxSnakes;
        this.maxFoods = maxFoods;
        this.worldRadius = worldRadius;

        this.snakePool = new SnakeState[this.maxSnakes];
        for (int i = 0; i < this.maxSnakes; i++)
        {
            GameObject newSnake = new GameObject();
            GameObject newSnakeHead = new GameObject();
            snakePool[i] = newSnake.AddComponent<SnakeState>();
            newSnake.AddComponent<SnakeMeshGenerator>();
            newSnakeHead.gameObject.transform.parent = newSnake.transform;
            snakePool[i].head = newSnakeHead;
            snakePool[i].enabled = false;
        }

        this.foodPool = new FoodState[this.maxFoods];
        for (int i = 0; i < this.maxFoods; i++)
        {
            GameObject newFood = new GameObject();
            foodPool[i] = newFood.AddComponent<FoodState>();
        }
    }
	
    public SnakeState ActivateSnake<T>(int snakeID, string name, int score, Vector2 position, int skinID)
    {
        if (snakePool[snakeID].enabled == true)
            this.DeactivateSnake(snakeID);

        snakePool[snakeID].enabled = true;
        snakePool[snakeID].name = name;
        snakePool[snakeID].head.transform.position = position;
        snakePool[snakeID].snakeSkinID = skinID;

        SnakeMeshGenerator view = snakePool[snakeID].GetComponent<SnakeMeshGenerator>();
        view.enabled = true;
        snakePool[snakeID].GetComponent<MeshRenderer>().enabled = true;
        snakePool[snakeID].score = score;
        snakePool[snakeID].gameObject.AddComponent(typeof(T));
        return snakePool[snakeID];
    }

    public SnakeState GetSnake(int snakeID)
    {
        return snakePool[snakeID];
    }

    public void DeactivateSnake(int snakeID)
    {
        snakePool[snakeID].enabled = false;
        SnakeMeshGenerator view = snakePool[snakeID].GetComponent<SnakeMeshGenerator>();
        view.enabled = false;
        snakePool[snakeID].GetComponent<MeshRenderer>().enabled = false;
        Destroy(snakePool[snakeID].GetComponent<IController>().getControllerComponent());
    }
}
