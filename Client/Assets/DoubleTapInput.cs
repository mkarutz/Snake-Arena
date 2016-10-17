using UnityEngine;
using System.Collections;

public class DoubleTapInput : MonoBehaviour
{
	private const float DOUBLE_TAP_COOLDOWN = 0.5f;
	private int touches = 0;
	private float timeSinceLastTouch = 0.0f;


	public bool DoubleTapIsHeld()
	{
		return touches >= 2;
	}


	void Update()
	{
		timeSinceLastTouch += Time.deltaTime;

		if (!TouchDown() && timeSinceLastTouch > DOUBLE_TAP_COOLDOWN) {
			touches = 0;
		}

		if (TouchBegan()) {
			touches++;
			timeSinceLastTouch = 0.0f;
		}
	}


	private bool TouchDown()
	{
		return Input.touches.Length > 0;
	}


	private bool TouchBegan()
	{
		foreach (Touch touch in Input.touches) {
			if (touch.phase.Equals(TouchPhase.Began)) {
				return true;
			}
		}

		return false;
	}
}
