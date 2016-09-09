using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public abstract class LocalSnakeController : MonoBehaviour, IController {
    private const float MOVE_SPEED = 2.0f;
    private Quaternion direction;
    private Quaternion targetDirection;

    protected SnakeState snakeState;

    // Use this for initialization
    void Start () {
        this.snakeState = this.GetComponent<SnakeState>();
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

        // Add to backbone path only if deviating (straight line movements don't need middle points)
        Vector3 headVec = this.snakeState.getBackbone().Last() - this.snakeState.head.transform.position;
        if (headVec.sqrMagnitude > 0.00001f)
        {
            Vector3 firstSegmentVec;
            if (this.snakeState.getBackbone().Count >= 2)
            {
                firstSegmentVec = this.snakeState.getBackbone()[this.snakeState.getBackbone().Count - 2] - this.snakeState.getBackbone().Last();

                if (Vector3.Angle(headVec, firstSegmentVec) > 5.0f)
                    this.snakeState.getBackbone().Add(this.snakeState.head.transform.position);
                else
                    this.snakeState.getBackbone()[this.snakeState.getBackbone().Count - 1] = this.snakeState.head.transform.position;
            }
            else
                this.snakeState.getBackbone().Add(this.snakeState.head.transform.position);
        }

        this.snakeState.head.transform.Translate(Vector3.forward * MOVE_SPEED * Time.deltaTime);
    }

    public Component getControllerComponent()
    {
        return this;
    }
}
