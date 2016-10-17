using UnityEngine;
using System.Collections;

public class JoystickControlScheme : ControlScheme
{
	public override bool IsTurbo()
	{
		return Input.anyKey;
	}

	public override Vector2 TargetDirection()
	{
		return Input.mousePosition - new Vector3(Screen.width, Screen.height, 0.0f) * 0.5f;
	}
}
