using UnityEngine;
using System.Collections;

public class SnakePlayerInput : MonoBehaviour {
	public InputManager inputManager;
	public SnakeState snakeState;

	void Update () {
		snakeState.Move(inputManager.TargetDirection(), Time.deltaTime);
        snakeState.SetTurboLocal(inputManager.IsTurbo());
	}
}
