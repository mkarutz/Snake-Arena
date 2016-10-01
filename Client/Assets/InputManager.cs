using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	private Vector2 targetDirection;
	private bool isTurbo = false;

	void Update () {
		Vector3 lookVec = (Input.mousePosition - new Vector3(Screen.width, Screen.height, 0.0f) * 0.5f);
		targetDirection = lookVec;

		isTurbo = Input.anyKey;
	}

	public Vector2 TargetDirection() {
		return targetDirection;
	}

	public bool IsTurbo() {
		return isTurbo;
	}
}
