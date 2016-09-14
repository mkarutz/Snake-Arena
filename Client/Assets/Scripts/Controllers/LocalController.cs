using UnityEngine;
using System.Collections;

public class LocalController : MonoBehaviour {

    public int maxSnakes = 100;
    public int maxFoods = 10000;
    public int worldRadius = 500;

    public GameState state;

    public CameraController camera;

    private GameObject playerSnake;

	// Use this for initialization
	void Start () {
        state.InitState(maxSnakes, maxFoods, worldRadius);
        
        this.playerSnake = state.ActivateSnake<LocalSnakeControllerInput>(0, "Player", 20, Vector2.zero, 0);

        this.camera.snakeToTrack = this.playerSnake.GetComponent<SnakeState>();

        state.ActivateFood(1, Vector2.zero, Color.red, 2);
        state.ActivateFood(2, Vector2.one, Color.blue, 2);
        state.ActivateFood(3, Vector2.right, Color.cyan, 2);

        state.ActivateSnake<LocalSnakeControllerAI>(1, "Enemy", 100, Vector2.zero, 1).GetComponent<SnakeState>();
        state.ActivateSnake<LocalSnakeControllerAI>(2, "Enemy", 2000, Vector2.zero, 2).GetComponent<SnakeState>();
        state.ActivateSnake<LocalSnakeControllerAI>(3, "Enemy", 40000, Vector2.zero, 2).GetComponent<SnakeState>();

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
            this.state.DeactivateSnake(0);
        if (Input.GetKeyDown(KeyCode.B))
            this.playerSnake = state.ActivateSnake<LocalSnakeControllerInput>(0, "Player", 20, Vector2.zero, 0);
    }
}
