using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class LocalSnakeController : MonoBehaviour {

    public SnakeState snake;

	// Update is called once per frame
	void Update () {

        Vector3 lookVec = (Input.mousePosition - new Vector3(Screen.width, Screen.height, 0.0f) * 0.5f);

        this.snake.Move(this.snake.transform.position + lookVec.normalized, Time.deltaTime);
        
        
    }
}
