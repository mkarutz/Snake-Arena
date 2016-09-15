using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public SnakeState snakeToTrack;
    public new Camera camera;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position = snakeToTrack.head.transform.position + Vector3.back * 20.0f;
        this.camera.orthographicSize = snakeToTrack.GetSnakeThickness() * 10.0f;
	}
}
