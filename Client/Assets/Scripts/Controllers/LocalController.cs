using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class LocalController : MonoBehaviour {

    private class MockFoodState
    {
        public int id;
        public Vector2 position;
        public Color color;
        public int weight;
    }

    private float cellSize = 25.0f; 

    public int maxSnakes = 100;
    public int maxFoods = 10000;
    public int worldRadius = 500;

    public GameState state;

    public new CameraController camera;

    private SnakeState playerSnake;

    private SpatialHashMap<MockFoodState> worldFoodsX;
    
    // keeps account of the previous body of objects that were activated, so we can deactivate if necessary
    private IEnumerable<MockFoodState> prevResult = new HashSet<MockFoodState>();

	// Use this for initialization
	void Start () {
        state.InitState(maxSnakes, maxFoods, worldRadius);

        this.worldFoodsX = new SpatialHashMap<MockFoodState>(new Vector2(-(worldRadius),-(worldRadius)), new Vector2(worldRadius, worldRadius),cellSize);
        for (int i = 0; i < maxFoods; i++)
        {
            float angle = Random.Range(0.0f, 360.0f);
            float dist = Random.Range(0.0f, worldRadius);
            //change
            MockFoodState mFood = new MockFoodState();
            mFood.id = i;
            mFood.position = Quaternion.AngleAxis(angle, Vector3.back) * (Vector3.right * dist);
            mFood.color = Random.ColorHSV(0.0f, 1.0f);
            mFood.weight = (int)Random.Range(2.0f, 8.0f);
            this.worldFoodsX.put(mFood,mFood.position);
        }

        this.playerSnake = state.ActivateSnake<LocalSnakeControllerInput>(0, "Player", 20, Vector2.zero, 0);

        this.camera.snakeToTrack = this.playerSnake.GetComponent<SnakeState>();
        
        state.ActivateSnake<LocalSnakeControllerAI>(1, "Enemy", 100, Vector2.zero, 1).GetComponent<SnakeState>();
        state.ActivateSnake<LocalSnakeControllerAI>(2, "Enemy", 2000, Vector2.zero, 2).GetComponent<SnakeState>();
        state.ActivateSnake<LocalSnakeControllerAI>(3, "Enemy", 300, Vector2.zero, 2).GetComponent<SnakeState>();
    }

    private void ManageFoodActivation()
    {
        //minX and maxY to set upper left corner of Rect 
        // documemntation says upper left but seems to create rect from lower left
        float cameraOrthSize = this.camera.GetComponent<Camera>().orthographicSize;

        // the Rect size needs to be optimized
        float minX = this.playerSnake.head.transform.position.x - cameraOrthSize*2;
        float minY = this.playerSnake.head.transform.position.y - cameraOrthSize*2;
        Rect playerNear = new Rect(minX,minY,(cellSize/2),(cellSize/2));

        IEnumerable<MockFoodState> result = this.worldFoodsX.getNear(playerNear);
        foreach (MockFoodState mFS in result)
        {
            if (!this.state.IsFoodActive(mFS.id))
            {
                //Debug.Log(this.worldFoods[i].position);
                this.state.ActivateFood<LocalFoodController>(mFS.id, mFS.position,
                    mFS.color, mFS.weight);
            }
        }

        foreach (MockFoodState mFS in prevResult.Except(result))
        {    
            if (this.state.IsFoodActive(mFS.id))
            {
                this.state.DeactivateFood(mFS.id);
            }
        }
        prevResult = result;
    }

    private void CollectFoodGarbage()
    {

    }
	
	// Update is called once per frame
	void Update () {        
        ManageFoodActivation();        

        if (Input.GetKeyDown(KeyCode.A))
            this.state.DeactivateSnake(0);
        if (Input.GetKeyDown(KeyCode.B))
            this.playerSnake = state.ActivateSnake<LocalSnakeControllerInput>(0, "Player", 20, Vector2.zero, 0);
    }
}
