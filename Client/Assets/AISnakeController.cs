using UnityEngine;
using System.Collections;

public class AISnakeController : MonoBehaviour {
	public SnakeState snake;

	private Vector2 targetPoint;

	void Start()
	{
		targetPoint = RandomTargetPoint ();
	}

	Vector2 RandomTargetPoint()
	{
		return new Vector2(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
	}

	void Update () 
	{
		if (Vector2.Distance(snake.transform.position, targetPoint) < 1.0f)
		{
			targetPoint = RandomTargetPoint();
		}

		snake.Move(targetPoint, Time.deltaTime);
	}
}
