using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Snake : MonoBehaviour {
	public float DISTANCE = 1;
	public SnakeSegment snakeSegmentPrefab;
	public SnakeHead head;
	public IList<SnakeSegment> bodySegments = new List<SnakeSegment>();
	public int initialNumSegments = 10;

	void Start () {
		for (int i = 0; i < initialNumSegments; i++) {
			SnakeSegment newSegment = Instantiate<SnakeSegment> (snakeSegmentPrefab);
			newSegment.gameObject.transform.parent = this.transform;
			newSegment.transform.position = head.transform.position + new Vector3 ((i + 1), 0, (i + 1));
			bodySegments.Add (newSegment);
		}
	}

	void Update () {
		Vector3 parentPrevPosition = head.transform.position;

		// Get input
		Vector3 desiredDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		// update the head
		head.Move(desiredDirection);

		// update the rest
		foreach (SnakeSegment segment in bodySegments) {
			Vector3 tempPosition = segment.transform.position;
			segment.transform.position = Vector3.Lerp (segment.transform.position, parentPrevPosition, 0.3F);
			parentPrevPosition = tempPosition;
		}
	}
}
