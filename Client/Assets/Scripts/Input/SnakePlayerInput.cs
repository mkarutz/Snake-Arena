using UnityEngine;
using System.Collections;

public class SnakePlayerInput : MonoBehaviour {
	public InputManager inputManager;
	public SnakeState snakeState;

    void Start()
    {
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();
    }

	void Update () {
		snakeState.Move(inputManager.TargetDirection(), Time.deltaTime);
        snakeState.SetTurboLocal(inputManager.IsTurbo());
	}
}
