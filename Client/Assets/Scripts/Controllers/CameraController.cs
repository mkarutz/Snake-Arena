using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public SnakeState snakeToTrack;
    public new Camera camera;

    private float targetOrthoSize;

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.position = snakeToTrack.head.transform.position + Vector3.back * 20.0f;
        this.targetOrthoSize = snakeToTrack.GetSnakeThickness() * 10.0f;
        this.camera.orthographicSize = this.camera.orthographicSize * 0.99f + targetOrthoSize * 0.01f;
	}
}
