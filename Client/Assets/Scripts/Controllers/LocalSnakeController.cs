using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class LocalSnakeController : MonoBehaviour, IController {
    private const float MOVE_SPEED = 2.0f;
    private const float MAX_HEAD_OFFSET = 0.02f;
    private Quaternion direction;
    private Quaternion targetDirection;

    protected SnakeState snakeState;

    protected BoxCollider boundingBox;

    // Use this for initialization
    void Awake () {
        this.snakeState = this.GetComponent<SnakeState>();
        this.snakeState.head.AddComponent<LocalSnakeHeadController>().snakeState = this.snakeState;

        //boundingBox = this.gameObject.GetComponent<BoxCollider>();
        //if (boundingBox == null)
       // {
       //     this.boundingBox = this.gameObject.AddComponent<BoxCollider>();
       //     updateBoundingBox();         
       // }
       // this.boundingBox.isTrigger = true;
    }

    
    void OnDestroy()
    {
        Destroy(this.snakeState.head.GetComponent<LocalSnakeHeadController>());
    }

    abstract protected void MovementControl();

    protected void SetTargetDirection(Vector3 lookDirection)
    {
        this.targetDirection = Quaternion.LookRotation(lookDirection, Vector3.back);
    }


    private float maxRotate()
    {
        return (360 * MOVE_SPEED) / (4.0f * Mathf.PI * this.snakeState.GetSnakeThickness());
    }
	
	// Update is called once per frame
	void Update () {

        this.MovementControl();
        this.direction = Quaternion.RotateTowards(this.direction, this.targetDirection, maxRotate() * Time.deltaTime);
        this.snakeState.head.transform.rotation = this.direction;

        // Translate snake forward
        this.snakeState.head.transform.Translate(Vector3.forward * MOVE_SPEED * Time.deltaTime);

        if (this.snakeState.GetBackboneLength() <= 2)
        {
            Debug.LogError("Not enough backbone points defined.");
            return;
        }

        // Check if we need to add a new backbone point
        Vector2 headVec = this.snakeState.GetBackbonePoint(1) - (Vector2)this.snakeState.head.transform.position;
        Vector2 neckVec = this.snakeState.GetBackbonePoint(2) - this.snakeState.GetBackbonePoint(1);

        Vector2 a = Vector3.Project(headVec, neckVec);
        if (headVec.sqrMagnitude - a.sqrMagnitude > MAX_HEAD_OFFSET * MAX_HEAD_OFFSET)
        {
            this.snakeState.AddBackboneHeadPoint(this.snakeState.head.transform.position);
        }

        this.snakeState.UpdateBackboneHeadPoint(this.snakeState.head.transform.position);
    }

    public Component getControllerComponent()
    {
        return this;
    } 
}
