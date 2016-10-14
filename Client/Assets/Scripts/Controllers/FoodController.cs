using UnityEngine;
using System.Collections;

public class FoodController : MonoBehaviour {

    public float foodGrowthSpeed = 1.0f;
    public float foodCollectionSpeed = 3.0f;
    private FoodState foodState;
    private float scaleFactorUnsmoothed;

	// Use this for initialization
	void Start () {
        this.foodState = this.gameObject.GetComponent<FoodState>();
        this.scaleFactorUnsmoothed = this.foodState.scaleFactor;

        // Vary food collection rates
        this.foodGrowthSpeed += Random.Range(0.0f, 2.0f);
	}

    // Update is called once per frame
    void Update()
    {
        // this.transform.position = this.foodState.position;

        if (this.foodState.collected && this.foodState.scaleFactor > float.Epsilon)
        {
            this.scaleFactorUnsmoothed -= this.foodCollectionSpeed * Time.deltaTime;

            // Lerp towards collecting snake
            if (this.foodState.collectingSnake)
                this.transform.position = 
                    Vector2.Lerp(this.transform.position, this.foodState.collectingSnake.transform.position, 1.0f - this.scaleFactorUnsmoothed);

            this.foodState.scaleFactor = CollectLerpFunction(this.scaleFactorUnsmoothed);
        }
        else if (!this.foodState.collected && this.scaleFactorUnsmoothed < 1.0f - float.Epsilon)
        {
            // Growth phase
            this.scaleFactorUnsmoothed += this.foodGrowthSpeed * Time.deltaTime;
            this.foodState.scaleFactor = GrowLerpFunction(this.scaleFactorUnsmoothed);
        }
        else
        {
            if (this.foodState.collected)
            {
                this.scaleFactorUnsmoothed = 0.0f;
                Destroy(this.gameObject); // Collection animation over
            }
            else
                this.scaleFactorUnsmoothed = 1.0f;
            this.foodState.scaleFactor = this.scaleFactorUnsmoothed;
        }
    }

    private float GrowLerpFunction(float value)
    {
        // https://aposto2.wordpress.com/2013/01/28/easing/
        float start = 0.0f;
        float end = 1.0f;
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (value == 0) return start;

        if ((value /= d) == 1) return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);

        //return t * t * t * (t * (6f * t - 15f) + 10f);
    }

    private float CollectLerpFunction(float t)
    {
        return t * t * t * (t * (6f * t - 15f) + 10f);
    }
}
