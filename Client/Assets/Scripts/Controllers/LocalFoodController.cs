using UnityEngine;
using System.Collections;
using System;

public class LocalFoodController : MonoBehaviour, IController {

    public float foodGrowthSpeed = 1.0f;
    public float foodCollectionSpeed = 3.0f;

    private SnakeState collectingSnake;
    private FoodState foodState;
    private FoodView foodView;

    private CircleCollider2D circleCollider;
    //private Rigidbody2D rbody;

    void Awake ()
    {
        this.foodState = this.GetComponent<FoodState>();
        this.foodView = this.GetComponent<FoodView>();
        this.foodView.scaleFactor = 0.0f;

        this.circleCollider = this.gameObject.GetComponent<CircleCollider2D>();
        if (this.circleCollider == null)
            this.circleCollider = this.gameObject.AddComponent<CircleCollider2D>();

        this.circleCollider.isTrigger = true;
        this.circleCollider.radius = 0.0f;
        
        this.tag = "Food";

        //this.transform.position = this.foodState.position;
    }

    void OnDestroy()
    {
        Destroy(this.circleCollider);
    }
	
	void Update ()
    {
       // this.transform.position = this.foodState.position;

        if (this.foodState.collected && this.foodView.scaleFactor > float.Epsilon)
        {
            this.foodView.scaleFactor -= this.foodCollectionSpeed * Time.deltaTime;

            // Lerp towards collecting snake
            //this.transform.position = 
              //  Vector2.Lerp(this.foodState.position, this.collectingSnake.head.transform.position, 1.0f - this.foodView.scaleFactor);
        }
        else if (!this.foodState.collected && this.foodView.scaleFactor < 1.0f - float.Epsilon)
        {
            this.foodView.scaleFactor += this.foodGrowthSpeed * Time.deltaTime;
        }
        else
        {
            if (this.foodState.collected)
                this.foodView.scaleFactor = 0.0f;
            else
                this.foodView.scaleFactor = 1.0f;
        }

        this.circleCollider.enabled = !this.foodState.collected;
        this.circleCollider.radius = (this.transform.localScale.x / 2.0f) + 0.5f;
    }

    public int CollectFood(SnakeState collectingSnake)
    {
        this.collectingSnake = collectingSnake;
        this.foodState.collected = true;
        return this.foodState.weight;
    }

    //public void 

    public Component getControllerComponent()
    {
        return this;
    }
}
