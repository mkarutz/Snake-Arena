using UnityEngine;
using System.Collections;

public class LocalSnakeBody : MonoBehaviour {

    public FoodState foodPrefab;

    private GameObject snakeBody;
    private BoxCollider2D snakeBodyCollider;
    private SnakeState snakeState;

    // Use this for initialization
    void Start () {
        this.snakeState = this.gameObject.GetComponent<SnakeState>();
        CreateSnakeBodyCollider();
    }
	
	// Update is called once per frame
	void Update () {
        UpdateSnakeBodyCollider();
        CheckWorldCollision();
	}

    void OnDestroy()
    {
        // Drop food
        this.DropFood();
        Destroy(this.snakeBody.gameObject);
    }

    private void CreateSnakeBodyCollider()
    {
        this.snakeBody = new GameObject();
  
        this.snakeBody.tag = "SnakeBody";
        this.snakeBody.transform.position = this.snakeState.GetHeadPosition();
        this.snakeBody.AddComponent<SnakeStateReference>().snakeState = this.snakeState;
        this.snakeBodyCollider = this.snakeBody.AddComponent<BoxCollider2D>();
        this.snakeBodyCollider.isTrigger = true;
    }

    private void UpdateSnakeBodyCollider()
    {
        Bounds b = this.GetBodyBounds();
        this.snakeBody.transform.position = this.snakeState.GetHeadPosition();
        this.snakeBodyCollider.size = b.size;
        this.snakeBodyCollider.offset = this.snakeBody.transform.InverseTransformPoint(b.center);
    }

    private Bounds GetBodyBounds()
    {
        Vector3 head = this.snakeState.GetHeadPosition();
        Vector3 tail = this.snakeState.CalcBackboneParametizedPosition(this.snakeState.GetSnakeLength());

        float delta = 0.5f;
        float d = delta;
        Bounds b = new Bounds(head, new Vector3(delta, delta, 0));

        float radius = this.snakeState.GetSnakeThickness();

        b.Encapsulate(head);
        while (d < this.snakeState.GetSnakeLength())
        {
            Vector2 next = this.snakeState.CalcBackboneParametizedPosition(d);
            b.Encapsulate(next + new Vector2(radius, radius));
            b.Encapsulate(next + new Vector2(-radius, -radius));
            b.Encapsulate(next + new Vector2(radius, -radius));
            b.Encapsulate(next + new Vector2(-radius, radius));
            d += delta;
        }
        b.Encapsulate(tail);
        return b;
    }
      

    public void DropFood()
    {
        int score = this.snakeState.score / 2;
        float delta = 0.1f;
        float d = delta;

        float radius = this.snakeState.GetSnakeThickness();

        int ds = score;

        Vector2 curr = this.snakeState.GetHeadPosition();
        while (d <= this.snakeState.GetSnakeLength())
        {
            if (ds <= 0)
            {
                break;
            }
            FoodState food = GameObject.Instantiate<FoodState>(this.foodPrefab);
            food.transform.position = curr + new Vector2(Random.Range(-radius, +radius), Random.Range(-radius, +radius));
            food.weight = Mathf.Min(Random.Range(2, 8), ds);
            ds -= food.weight;
            d += delta;
            curr = this.snakeState.CalcBackboneParametizedPosition(d);
        }
    }

    void CheckWorldCollision()
    {
        if ((Vector2.Distance(this.snakeState.GetHeadPosition(), Vector2.zero)) > GameConfig.WORLD_RADIUS_LOCAL)
        {
            Destroy(this.snakeState.gameObject);
        }
    }
}
