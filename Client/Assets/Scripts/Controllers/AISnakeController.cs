using UnityEngine;
using System.Collections;

public class AISnakeController : MonoBehaviour {
	public SnakeState snake;

	private GameWorld gameWorld;
	private Vector2 targetPoint;
    private int chanceToTurbo;
	void Start()
	{
		this.gameWorld = GameObject.FindWithTag ("World").GetComponent<GameWorld>();
		this.targetPoint = RandomTargetPoint ();
	}

	Vector2 RandomTargetPoint()
	{
		return this.gameWorld.GenerateRandomWorldPoint();
	}

	void Update () 
	{
		if (Vector2.Distance(snake.transform.position, targetPoint) < 1.0f || Random.Range(0, 500) == 0)
		{
			targetPoint = RandomTargetPoint();
		}
        
		snake.Move(targetPoint, Time.deltaTime);

        float snakeScore = this.snake.score;
        int maxScore = GameConfig.SNAKE_GROWTH_CAP;

        
        if(this.snake.isTurbo)
        {
            chanceToTurbo = 90;
        }
        else
        {
            //maxScore is too large
            chanceToTurbo = (int)Mathf.Round((100 / (maxScore/100)) * snakeScore);
        }

        Debug.Log(chanceToTurbo);
        snake.SetTurboLocal(Random.Range(0,100)<chanceToTurbo);
	}
}
