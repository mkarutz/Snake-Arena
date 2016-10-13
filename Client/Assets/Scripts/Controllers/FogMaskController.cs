using UnityEngine;
using System.Collections;

public class FogMaskController : MonoBehaviour {

    private SnakeState playerSnake;

	
	// Update is called once per frame
	void Update () {
        this.playerSnake = GameObject.FindGameObjectWithTag("Player").GetComponent<SnakeState>();
        if (this.playerSnake)
        {
            this.transform.localScale = Vector3.one * 0.2f * this.playerSnake.SnakeFogDistance();
            this.gameObject.transform.position = this.playerSnake.transform.position + Vector3.back * 0.5f;
        }
	}
}
