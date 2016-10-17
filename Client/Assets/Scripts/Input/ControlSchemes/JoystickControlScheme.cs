using UnityEngine;
using System.Collections;

public class JoystickControlScheme : ControlScheme
{
	public DoubleTapInput doubleTapInput;

	private bool fingerIsDown = false;
	private Vector2 fingerDownPosition = Vector2.zero;
	private Vector2 targetDirection = Vector2.zero;


	void Update()
	{
		if (TouchIsNew()) {
			fingerDownPosition = FingerPosition();
		}

		if (TouchIsDown()) {
			targetDirection = FingerPosition() - fingerDownPosition;
		}
	}


	private bool TouchIsNew()
	{
		return FirstTouch().phase.Equals(TouchPhase.Began);
	}


	private bool TouchIsDown()
	{
		return FirstTouch().phase.Equals(TouchPhase.Began) 
			|| FirstTouch().phase.Equals(TouchPhase.Stationary)
			|| FirstTouch().phase.Equals(TouchPhase.Moved);
	}


	private Touch FirstTouch()
	{
		return Input.touches[0];
	}


	private Vector2 FingerPosition()
	{
		return FirstTouch().position;
	}


	public override bool IsTurbo()
	{
		return doubleTapInput.DoubleTapIsHeld();
	}
		

	public override Vector2 TargetDirection()
	{
		return targetDirection;
	}
}
