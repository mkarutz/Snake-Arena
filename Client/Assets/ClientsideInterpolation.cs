using UnityEngine;
using System.Collections;

public class ClientsideInterpolation : MonoBehaviour 
{
	public SnakeState snake;
	public Vector2 targetHeadPosition = Vector2.zero;
	public float LerpAmount;


	void Update()
	{
		InterpolateHeadPosition();
	}


	private void InterpolateHeadPosition()
	{
		snake.GetRawBackboneArray()[snake.GetRawBackboneStartIdx()] += (targetHeadPosition - snake.GetRawBackboneArray()[snake.GetRawBackboneStartIdx()]) * LerpAmount;
	}
}
