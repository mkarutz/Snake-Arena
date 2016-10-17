using UnityEngine;
using System.Collections;

public class FollowFingerControlScheme : ControlScheme
{
	public DoubleTapInput doubleTapInput;


	public override bool IsTurbo()
	{
		return doubleTapInput.DoubleTapIsHeld();
	}


	public override Vector2 TargetDirection()
	{
		return Input.mousePosition - new Vector3(Screen.width, Screen.height, 0.0f) * 0.5f;
	}
}
