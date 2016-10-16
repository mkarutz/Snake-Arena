using UnityEngine;
using System.Collections;

public class AISnakeController : MonoBehaviour {
	public SnakeState snake;
    public float circleCastDistance = 4.0f;

	private GameWorld gameWorld;
	private Vector2 targetPoint;

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
        Vector2 lookDir = targetPoint - (Vector2)this.transform.position;
        RaycastHit2D[] circleHits = Physics2D.CircleCastAll(this.transform.position, this.circleCastDistance, lookDir);

        Vector2 hitPointAvg = Vector2.zero;
        int hitPointCount = 0;
        foreach (RaycastHit2D circleHit in circleHits)
        {
            if (circleHit.collider.gameObject.tag == "SnakeBody" &&
                circleHit.collider.gameObject.GetComponent<SnakeStateReference>().snakeState != this.snake)
            {
                hitPointAvg += circleHit.point;
                hitPointCount++;
                break;
            }
        }

        hitPointAvg /= hitPointCount;
        targetPoint = (Vector2)this.transform.position + (((Vector2)this.transform.position - hitPointAvg).normalized * 5.0f);
        if (targetPoint.magnitude >= this.gameWorld.worldRadius)
        {
            targetPoint = RandomTargetPoint();
        }


        if (Vector2.Distance(snake.transform.position, targetPoint) < 1.0f || Random.Range(0, 500) == 0)
		{
			targetPoint = RandomTargetPoint();
		}

		snake.Move(targetPoint, Time.deltaTime);

        EatInvisibleFoodRandomly();
	}

    private void EatInvisibleFoodRandomly()
    {
        if (Random.Range(0, 200) == 0)
        {
            this.snake.score += Random.Range(2, 10);
        }
    }
}
