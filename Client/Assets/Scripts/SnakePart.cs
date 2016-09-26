using UnityEngine;
using System.Collections;
using slyther.flatbuffers;

public class SnakePart : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void replicateState(SnakePartStateFB snakePartState) {
		this.transform.position = new Vector3(snakePartState.Position.X, snakePartState.Position.Y, 0.0f);
	}
}
