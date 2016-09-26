using UnityEngine;
using System.Collections;
using System;
using slyther.flatbuffers;

public class NetworkSnakeController : MonoBehaviour, IController {

    private SnakeState snakeState;


    public Component getControllerComponent()
    {
        return this;
    }
    void Awake()
    {
        this.snakeState = this.GetComponent<SnakeState>();
        this.snakeState.head.AddComponent<NetworkSnakeHeadController>().snakeState = this.snakeState;
    }

    public Vector3 GetDesiredMove(Camera camera)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        //don't know why 19.7f is used
        Vector3 desiredWorldLocation = ray.origin + (19.7f * ray.direction);

        //Vector3 desiredScreenMove = Input.mousePosition - new Vector3(Screen.width, Screen.height, 0.0f) * 0.5f; 
        return (desiredWorldLocation);
    }

    void OnDestroy()
    {
        Destroy(this.snakeState.head.GetComponent<NetworkSnakeHeadController>());
    }

    public void ReplicateSnakeState(NetworkSnakeState fSnakeState)
    {
        float headX = fSnakeState.GetParts(fSnakeState.Head).Position.X;
        float headY = fSnakeState.GetParts(fSnakeState.Head).Position.Y;
       // Debug.Log("Recieved head position = (" + headX + ", " + headY + ")");

        this.snakeState.head.transform.position = new Vector3(headX, headY, 0.0f);
        this.snakeState.snakeSkinID = 1;//int.Parse(snakeState.Skin);
        this.snakeState.name = fSnakeState.Name;
        this.snakeState.score = (int)fSnakeState.Score;
    }

}
