using UnityEngine;
using System.Collections;

public class LocalController : MonoBehaviour {

    private class MockFoodState
    {
        public Vector2 position;
        public Color color;
        public int weight;
    }

    private float garbageRadius;

    public int maxSnakes = 100;
    public int maxFoods = 10000;
    public int worldRadius = 500;

    public GameState state;

    public new CameraController camera;

    private SnakeState playerSnake;
    private MockFoodState[] worldFoods;

	// Use this for initialization
	void Start () {
        state.InitState(maxSnakes, maxFoods, worldRadius);

        this.worldFoods = new MockFoodState[maxFoods];
        for (int i = 0; i < maxFoods; i++)
        {
            float angle = Random.Range(0.0f, 360.0f);
            float dist = Random.Range(0.0f, worldRadius);
            this.worldFoods[i] = new MockFoodState();
            this.worldFoods[i].position = Quaternion.AngleAxis(angle, Vector3.back) * (Vector3.right * dist);
            this.worldFoods[i].color = Random.ColorHSV(0.0f, 1.0f);
            this.worldFoods[i].weight = (int)Random.Range(2.0f, 8.0f);
        }

        this.playerSnake = state.ActivateSnake<LocalSnakeControllerInput>(0, "Player", 20, Vector2.zero, 0);

        this.camera.snakeToTrack = this.playerSnake.GetComponent<SnakeState>();
        
        state.ActivateSnake<LocalSnakeControllerAI>(1, "Enemy", 100, Vector2.zero, 1).GetComponent<SnakeState>();
        state.ActivateSnake<LocalSnakeControllerAI>(2, "Enemy", 2000, Vector2.zero, 2).GetComponent<SnakeState>();
        state.ActivateSnake<LocalSnakeControllerAI>(3, "Enemy", 300, Vector2.zero, 2).GetComponent<SnakeState>();
    }

    private void ManageFoodActivation()
    {
        this.garbageRadius = this.camera.GetComponent<Camera>().orthographicSize * 4.0f;
        for (int i = 0; i < maxFoods; i++)
        {
            Vector2 dv = (Vector2)this.playerSnake.head.transform.position - this.worldFoods[i].position;
            
            if (dv.sqrMagnitude < garbageRadius * garbageRadius)
            {

                if (!this.state.IsFoodActive(i))
                {
                    //Debug.Log(this.worldFoods[i].position);
                    this.state.ActivateFood<LocalFoodController>(i, this.worldFoods[i].position,
                        this.worldFoods[i].color, this.worldFoods[i].weight);
                }
            }
            else
            {
                if (this.state.IsFoodActive(i))
                    this.state.DeactivateFood(i);
            }
        }
    }

    private void CollectFoodGarbage()
    {

    }
	
	// Update is called once per frame
	void Update () {

        

        ManageFoodActivation();
        //state.ActivateFood<LocalFoodController>(i, new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f)), Random.ColorHSV(0.0f, 1.0f), (int)Random.Range(1.0f, 10.0f));
        

        if (Input.GetKeyDown(KeyCode.A))
            this.state.DeactivateSnake(0);
        if (Input.GetKeyDown(KeyCode.B))
            this.playerSnake = state.ActivateSnake<LocalSnakeControllerInput>(0, "Player", 20, Vector2.zero, 0);
    }
}
