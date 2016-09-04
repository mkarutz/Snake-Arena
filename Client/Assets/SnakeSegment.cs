using UnityEngine;
using System.Collections;

public class SnakeSegment : MonoBehaviour {
	public float MOVE_SPEED = 10;
	public float MAX_ROTATE = 5;
	public float DISTANCE = 1;

	public Vector3 direction;

	public void Move (Vector3 deltaPosition) {
		if (deltaPosition.magnitude > DISTANCE) {
			Vector3 desiredMove = deltaPosition.normalized * (deltaPosition.magnitude - DISTANCE);
			direction = Vector3.RotateTowards (direction, deltaPosition, MAX_ROTATE * Time.deltaTime, 0.0F);
			Vector3 desiredMoveProjected = Vector3.Project (desiredMove, direction);
			transform.position += desiredMoveProjected;
		}
	}
}
