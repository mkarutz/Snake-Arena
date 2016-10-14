using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public const float VIEWPORT_SCALE = 15.0f;

	//public NetworkController networkController;
	public SnakeState snakeToTrack = null;
    public new Camera camera;
	public float LerpAmount = 0.1f;
    public float maxViewportDimension;

	void Update () 
	{
		FollowSnake();
		UpdateCameraSize();
	}


	void FollowSnake()
	{
        GameObject snake = GameObject.FindGameObjectWithTag("Player");
        if (snake)
        {
            snakeToTrack = snake.GetComponent<SnakeState>();
            transform.position = Vector3.Lerp(this.transform.position, snakeToTrack.transform.position, LerpAmount) + Vector3.back;
        }
        else
            snakeToTrack = null;
	}


	void UpdateCameraSize()
	{
		camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, ViewPortHeight(), LerpAmount);
	}


	float ViewPortHeight()
	{
        if (snakeToTrack)
		    this.maxViewportDimension = snakeToTrack.SnakeFogDistance();
		float aspectRatio = 1.0f * Screen.width / Screen.height;
		if (aspectRatio > 1.0f) {
			return maxViewportDimension / aspectRatio;
		}
		return maxViewportDimension;
	}
}
