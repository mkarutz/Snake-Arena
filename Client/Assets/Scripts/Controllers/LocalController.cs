using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LocalController : MonoBehaviour {
	private int maxSnakes = GameConfig.MAX_LOCAL_SNAKES;
	private int maxFoods = GameConfig.MAX_LOCAL_FOODS;

	public GameWorld gameWorld;
	public FoodState localFoodPrefab;
	public SnakeState localSnakePrefab;
	public SnakeState localAISnakePrefab;

    private SnakeState[] snakePool;
    private SpatialHashMap<SnakeState> snakeMap;
    private float cellSize =5.0f;

	// Use this for initialization
	void Start () {
        this.snakePool = new SnakeState[maxSnakes];
        this.snakeMap = new SpatialHashMap<SnakeState>(new Vector2(-gameWorld.worldRadius,-gameWorld.worldRadius),new Vector2(gameWorld.worldRadius,gameWorld.worldRadius),cellSize);
		GenerateFoods ();
		GenerateLocalSnake ();
		GenerateAISnakes ();
    }

	private void GenerateFoods()
	{
		for (int i = 0; i < maxFoods; i++)
		{
			FoodState food = Instantiate<FoodState>(this.localFoodPrefab);
			food.transform.position = this.gameWorld.GenerateRandomWorldPoint(0.0f, this.gameWorld.worldRadius);
			food.weight = (int)Random.Range(2.0f, 8.0f);
		}
	}

	private void GenerateLocalSnake()
	{
		SnakeState playerSnake = Instantiate<SnakeState> (this.localSnakePrefab);
		playerSnake.transform.position = this.gameWorld.GenerateRandomWorldPoint(0, this.gameWorld.worldRadius / 5.0f);
        this.snakeMap.put(playerSnake,playerSnake.GetSnakeRect());
        this.snakePool[0] = playerSnake;
    }

	private void GenerateAISnakes()
	{
		for (int i = 0; i < maxSnakes - 1; i++)
		{
			SnakeState snake = Instantiate<SnakeState> (this.localAISnakePrefab);
			snake.SetRandomStartBackbone(this.gameWorld.GenerateRandomWorldPoint (this.gameWorld.worldRadius / 4.0f, this.gameWorld.worldRadius));
			snake.snakeSkinID = Random.Range(0, GameConfig.NUM_SNAKE_SKINS);
            this.snakeMap.put(snake, snake.GetSnakeRect());
            this.snakePool[i + 1] = snake;
		}
	}

    private void UpdateSnakeBounds()
    {
        foreach(SnakeState snake in this.snakePool)
        {
           
            this.snakeMap.update(snake, snake.GetSnakeRect());
        }
    }

//    private void checkSnakeCollisions()
//    {
//        for(int i = 0;i < maxSnakes; i++)
//        {
//            foreach (SnakeState other in snakeMap.getNear(snakePool[i].GetHeadPosition()))
//            {
//                if(other.enabled == false)
//                {
//                    continue;
//                }
//                if(other == snakePool[i])
//                {
                    
//                    continue;
//                }
                
//                if (snakePool[i].isRunInto(other))
//                {
//                    snakePool[i].Despawn();
//                }
//            }
//        }
        
 //   }
	// Update is called once per frame
	void Update () {
        //checkSnakeCollisions();
        UpdateSnakeBounds();
        
    }
}
