using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public class InputManager : MonoBehaviour {
	private Vector2 targetDirection;
	private bool isTurbo = false;
    private SnakeState playerSnake;
    private GameWorld gameWorld;

    void Start ()
    {
        GameObject snake = GameObject.FindGameObjectWithTag("Player");
        if (snake)
            this.playerSnake = snake.GetComponent<SnakeState>();
        this.gameWorld = GameObject.FindGameObjectWithTag("World").GetComponent<GameWorld>();
    }

	void Update () {
        if (VRSettings.enabled)
        {
            // Virtual reality input
            // Move towards where we're looking
            Ray lookRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            float dist;
            if (this.gameWorld.GetWorldPlane().Raycast(lookRay, out dist))
            {
                Vector3 lookWorldSpacePos = lookRay.GetPoint(dist);
                Vector2 lookVec = lookWorldSpacePos - this.playerSnake.transform.position;
                targetDirection = lookVec * 1000;
            }
        }
        else
        {
            // Standard input
            Vector3 lookVec = (Input.mousePosition - new Vector3(Screen.width, Screen.height, 0.0f) * 0.5f);
            targetDirection = lookVec;
        }
        isTurbo = Input.anyKey;
    }

	public Vector2 TargetDirection() {
		return targetDirection;
	}

	public bool IsTurbo() {
		return isTurbo;
	}
}
