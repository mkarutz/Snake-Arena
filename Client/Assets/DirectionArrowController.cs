using UnityEngine;
using System.Collections;

public class DirectionArrowController : MonoBehaviour {

    public InputManager inputManager;
    public float heightOffPlane = 0.5f;

    private float distance;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            SnakeState playerSnake = player.GetComponent<SnakeState>();
            this.transform.localScale = playerSnake.GetSnakeThickness() * Vector3.one * 2.0f;
            this.transform.parent = player.transform;
            this.distance = inputManager.TargetDirection().magnitude * 0.005f;
            this.transform.localPosition = new Vector3(0.0f, heightOffPlane, distance);
        }
	}
}
