using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using slyther.flatbuffers;

public class GameState : MonoBehaviour {

    // Public/exposed attributes
    //public GameObject snakeTemplate;
    //public GameObject foodTemplate;

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
        for (int i = 0; i < state.ObjectStatesLength; i++)
        {
            NetworkObjectState objectState = state.GetObjectStates(i);
            NetworkObjectStateType objectType = objectState.StateType;
            if (objectType == NetworkObjectStateType.FoodState)
            {
                slyther.flatbuffers.FoodState foodState = objectState.GetState<slyther.flatbuffers.FoodState>(new slyther.flatbuffers.FoodState());
                if (!IsFoodActive(foodState.FoodId))
                    //should be NetworkFoodController .. changed to Local for testing
                    ActivateFood<LocalFoodController>(foodState.FoodId,new Vector2 (foodState.Position.X,foodState.Position.Y),Color.red, foodState.Weight);
            }
            if(objectType == NetworkObjectStateType.SnakeState)
            {
                slyther.flatbuffers.SnakeState snakeState = objectState.GetState<slyther.flatbuffers.SnakeState>(new slyther.flatbuffers.SnakeState());
                if (!IsSnakeActive(snakeState.PlayerId))
                {
                        ActivateSnake<NetworkSnakeController>(snakeState.PlayerId, snakeState.Name, (int)snakeState.Score, Vector3.zero, 1);
                }
                else
                {
                    snakePool[snakeState.PlayerId].GetComponent<NetworkSnakeController>().ReplicateSnakeState(snakeState);
                }
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
            GameObject newFood = new GameObject();
            foodPool[i] = newFood.AddComponent<FoodState>();
            foodPool[i].enabled = false;
            foodPool[i].gameObject.AddComponent<FoodView>().enabled = false;
        }
    }

    public SnakeState ActivateSnake<T>(int snakeID, string name, int score, Vector2 position, int skinID)
        where T : IController
    {
        if (snakePool[snakeID].enabled == true)
            this.DeactivateSnake(snakeID);

        snakePool[snakeID].enabled = true;
        snakePool[snakeID].name = name;
        snakePool[snakeID].head.transform.position = position;
        snakePool[snakeID].snakeSkinID = skinID;

        SnakeMeshGenerator view = snakePool[snakeID].GetComponent<SnakeMeshGenerator>();
        view.enabled = true;
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
        Destroy(snakePool[snakeID].GetComponent<IController>().getControllerComponent());
    }

    public FoodState ActivateFood<T>(int foodID, Vector2 position, Color color, int weight)
        where T : IController
    {
        if (foodPool[foodID].enabled == true)
            DeactivateFood(foodID);
        
        foodPool[foodID].enabled = true;
        foodPool[foodID].position = position;
        foodPool[foodID].color = color;
        foodPool[foodID].weight = weight;
        foodPool[foodID].collected = false;

        foodPool[foodID].gameObject.AddComponent(typeof(T));

        foodPool[foodID].GetComponent<FoodView>().enabled = true;

        return foodPool[foodID];
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
