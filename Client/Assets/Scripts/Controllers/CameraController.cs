using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public const float VIEWPORT_SCALE = 10.0f;

	//public NetworkController networkController;
	public SnakeState snakeToTrack = null;
    public new Camera camera;
	public float LerpAmount = 0.1f;


	void Update () 
	{
        snakeToTrack = GameObject.FindGameObjectWithTag("Player").GetComponent<SnakeState>();
           //networkController.GetLocalPlayer().GetComponent<SnakeState>();
		transform.position = Vector3.Lerp(this.transform.position, snakeToTrack.transform.position, LerpAmount) + Vector3.back;
		camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, snakeToTrack.GetSnakeThickness() * VIEWPORT_SCALE, LerpAmount);
	}
}
