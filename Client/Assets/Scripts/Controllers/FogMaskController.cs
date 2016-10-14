using UnityEngine;
using System.Collections;

public class FogMaskController : MonoBehaviour {

    private SnakeState playerSnake;
	
	// Update is called once per frame
	void Update () {
        GameObject snake = GameObject.FindGameObjectWithTag("Player");
        if (snake)
        {
            this.playerSnake = snake.GetComponent<SnakeState>();
            this.transform.localScale = Vector3.one * 0.2f * this.playerSnake.SnakeFogDistance();
            this.gameObject.transform.position = this.playerSnake.transform.position + Vector3.back * 0.5f;
        }
	}
}
