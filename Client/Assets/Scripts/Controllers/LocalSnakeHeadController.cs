using UnityEngine;
using System.Collections;

public class LocalSnakeHeadController : MonoBehaviour {

    public SnakeState snakeState;

    private CircleCollider2D circleCollider;
    private Rigidbody2D rbody;
    
    void Start()
    {
        this.circleCollider = this.gameObject.GetComponent<CircleCollider2D>();
        if (this.circleCollider == null)
            this.circleCollider = this.gameObject.AddComponent<CircleCollider2D>();

        this.circleCollider.radius = 0.0f;

        this.rbody = this.gameObject.GetComponent<Rigidbody2D>();
        if (this.rbody == null)
            this.rbody = this.gameObject.AddComponent<Rigidbody2D>();
        this.rbody.isKinematic = true;
    }

    void OnDestroy()
    {
        Destroy(this.circleCollider);
        Destroy(this.rbody);
    }

    void Update()
    {
        // Collision body update
        this.circleCollider.radius = this.snakeState.GetSnakeThickness();
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Food")
        {
            // Collision with food, collect it
            LocalFoodController foodController = coll.gameObject.GetComponent<LocalFoodController>();
            this.snakeState.score += foodController.CollectFood(this.snakeState);
            //Debug.Log(this.circleCollider.radius);
        }
    }
}
