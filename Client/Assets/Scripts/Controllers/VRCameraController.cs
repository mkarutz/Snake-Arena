using UnityEngine;
using System.Collections;

public class VRCameraController : MonoBehaviour {
    public const float SCALE_FACTOR = 10.0f;
    public SnakeState snakeToTrack = null;
    public float LerpAmount = 0.1f;


    void Update()
    {
        FollowSnake();
    }


    void FollowSnake()
    {
        snakeToTrack = GameObject.FindGameObjectWithTag("Player").GetComponent<SnakeState>();
        //networkController.GetLocalPlayer().GetComponent<SnakeState>();
        Vector3 targetPosition = snakeToTrack.transform.position;
        targetPosition += Vector3.back * SCALE_FACTOR;
        transform.position = Vector3.Lerp(this.transform.position, targetPosition, LerpAmount);


    }
}
