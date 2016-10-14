using UnityEngine;
using System.Collections;

public class FoodController : MonoBehaviour {

    public float foodGrowthSpeed = 1.0f;
    public float foodCollectionSpeed = 3.0f;
    private FoodState foodState;

	// Use this for initialization
	void Start () {
        this.foodState = this.gameObject.GetComponent<FoodState>();
	}

    // Update is called once per frame
    void Update()
    {
        // this.transform.position = this.foodState.position;

        if (this.foodState.collected && this.foodState.scaleFactor > float.Epsilon)
        {
            this.foodState.scaleFactor -= this.foodCollectionSpeed * Time.deltaTime;

            // Lerp towards collecting snake
            this.transform.position = 
              Vector2.Lerp(this.transform.position, this.foodState.collectingSnake.transform.position, 1.0f - this.foodState.scaleFactor);
        }
        else if (!this.foodState.collected && this.foodState.scaleFactor < 1.0f - float.Epsilon)
        {
            this.foodState.scaleFactor += this.foodGrowthSpeed * Time.deltaTime;
        }
        else
        {
            if (this.foodState.collected)
                this.foodState.scaleFactor = 0.0f;
            else
                this.foodState.scaleFactor = 1.0f;
        }
    }
}
