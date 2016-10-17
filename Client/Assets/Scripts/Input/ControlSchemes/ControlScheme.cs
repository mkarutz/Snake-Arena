using UnityEngine;
using System.Collections;

public abstract class ControlScheme : MonoBehaviour 
{
	public abstract bool IsTurbo();
	public abstract Vector2 TargetDirection();
}
