using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public const float VIEWPORT_SCALE = 15.0f;

	//public NetworkController networkController;
	public SnakeState snakeToTrack = null;
    public new Camera camera;
	public float LerpAmount = 0.1f;


	void Update () 
	{
		FollowSnake();
		UpdateCameraSize();
	}


	void FollowSnake()
	{
		snakeToTrack = GameObject.FindGameObjectWithTag("Player").GetComponent<SnakeState>();
		transform.position = Vector3.Lerp(this.transform.position, snakeToTrack.transform.position, LerpAmount) + Vector3.back;
	}


	void UpdateCameraSize()
	{
		camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, ViewPortHeight(), LerpAmount);
	}


	float ViewPortHeight()
	{
		float maxViewportDimension = snakeToTrack.SnakeFogDistance();
		float aspectRatio = 1.0f * Screen.width / Screen.height;
		if (aspectRatio > 1.0f) {
			return maxViewportDimension / aspectRatio;
		}
		return maxViewportDimension;
	}
}
