using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using slyther.flatbuffers;

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

    public void ReplicateState(ServerWorldState state)
    {
        Debug.Log("States length = " + state.ObjectStatesLength);

        for (int i = 0; i < state.ObjectStatesLength; i++)
        {
            NetworkObjectState objectState = state.GetObjectStates(i);
            NetworkObjectStateType objectType = objectState.StateType;
            Debug.Log(objectType.ToString());
            if (objectType == NetworkObjectStateType.FoodState)
            {
                slyther.flatbuffers.FoodState foodState = objectState.GetState<slyther.flatbuffers.FoodState>(new slyther.flatbuffers.FoodState());
                ActivateFood(foodState.FoodId,new Vector2 (foodState.Position.X,foodState.Position.Y),Color.red,foodState.Weight);
            }
        }
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
            foodPool[i] = GameObject.Instantiate(foodTemplate).GetComponent<FoodState>();
            foodPool[i].enabled = false;
            foodPool[i].GetComponent<MeshRenderer>().enabled = false;
            foodPool[i].GetComponent<FoodView>().enabled = false;
        }
    }
	
    public GameObject ActivateFood(int foodID, Vector2 position,Color color, int weight)
    {
        if(foodPool[foodID].enabled == true)
        {
            DeactivateFood(foodID);
        }
        foodPool[foodID].enabled = true;
        foodPool[foodID].position = position;
        foodPool[foodID].color = color;
        foodPool[foodID].weight = weight;

        foodPool[foodID].GetComponent<FoodView>().enabled = true;
        foodPool[foodID].GetComponent<MeshRenderer>().enabled = true;

        return foodPool[foodID].gameObject;
    }

    public void DeactivateFood(int foodID)
    {
        foodPool[foodID].enabled = false;
        foodPool[foodID].GetComponent<FoodView>().enabled = false;
        foodPool[foodID].GetComponent<MeshRenderer>().enabled = false;

    }
    public GameObject ActivateSnake<T>(int snakeID, string name, int score, Vector2 position, int skinID)
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
        return snakePool[snakeID].gameObject;
    }

    public GameObject GetSnake(int snakeID)
    {
        return snakePool[snakeID].gameObject;
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
