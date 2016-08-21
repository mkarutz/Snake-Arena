using UnityEngine;
using System.Collections;

public class SnakeHead : MonoBehaviour {
	public float MOVE_SPEED = 10;
	public float MAX_ROTATE = 5;
	public float DISTANCE = 1;

	public Vector3 direction;

	public void Move (Vector3 desiredDirection) {
		if (desiredDirection.magnitude > 0) {
			desiredDirection.Normalize ();
			direction = Vector3.RotateTowards (direction, desiredDirection, MAX_ROTATE * Time.deltaTime, 0.0F);
		}
		transform.position += direction * MOVE_SPEED * Time.deltaTime;
	}
}
