using UnityEngine;
using System.Collections;

public class AccelerometerControlScheme : ControlScheme
{
	public DoubleTapInput doubleTapInput;


	public override bool IsTurbo()
	{
		return doubleTapInput.DoubleTapIsHeld();
	}


	public override Vector2 TargetDirection()
	{
		return (Vector2) Input.acceleration;
	}
}
