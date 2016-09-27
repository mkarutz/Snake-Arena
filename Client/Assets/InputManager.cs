using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	private Vector2 targetDirection;

	void Update () {
		Vector3 lookVec = (Input.mousePosition - new Vector3(Screen.width, Screen.height, 0.0f) * 0.5f);
		targetDirection = lookVec;
	}

	public Vector2 TargetDirection() {
		return targetDirection;
	}

	public bool IsTurbo() {
		return false;
	}
}
