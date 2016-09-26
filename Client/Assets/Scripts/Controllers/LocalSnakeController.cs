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

        //updateBoundingBox();

        this.snakeState.head.transform.Translate(Vector3.forward * MOVE_SPEED * Time.deltaTime);
    }

    public Component getControllerComponent()
    {
        return this;
    }

    public Rect getBoundingBox()
    {
        float minX = 0;
        float minY = 0;
        float maxX = 0;
        float maxY = 0;
        int flag = 0;

        foreach (Vector3 vec in this.snakeState.getBackbone())
        {
            if (flag == 0)
            {
                minX = vec.x;
                minY = vec.y;
                maxX = vec.x;
                maxY = vec.y;
                flag = 1;
            }
            else
            {
                if (vec.x < minX)
                {
                    minX = vec.x;
                }
                if (vec.x > maxX)
                {
                    maxX = vec.x;
                }

                if (vec.y < minY)
                {
                    minY = vec.y;
                }
                if (vec.y > maxY)
                {
                    maxY = vec.y;
                }
            }
        }

        Rect box = new Rect(minX, minY, (maxX - minX), (maxX - maxY));
        return box;
    }

    void updateBoundingBox()
    {
        Rect bound = getBoundingBox();

        //LineRenderer lr = this.gameObject.GetComponent<LineRenderer>();
        //if(lr == null)
       // {
       //     lr = this.gameObject.AddComponent<LineRenderer>();
       // }
       // Vector3[] positions = new Vector3[4];
       // positions[0] = new Vector3(bound.xMin, bound.yMin);
       // positions[1] = new Vector3(bound.xMin, bound.yMax);
       // positions[2] = new Vector3(bound.xMax, bound.yMin);
       // positions[3] = new Vector3(bound.xMax, bound.yMax);
       // lr.SetPositions(positions);
        
        Vector3 centre = new Vector3(bound.xMin + ((bound.xMax - bound.xMin) / 2), bound.yMin + ((bound.yMax - bound.yMin) / 2), 0.0f);
        Vector3 size = new Vector3(Mathf.Abs(Mathf.Abs(bound.xMax) - Mathf.Abs(bound.xMin)), Mathf.Abs(Mathf.Abs(bound.yMax) - Mathf.Abs(bound.yMin)), 0.0f);

      
        this.boundingBox.center = centre;
        this.boundingBox.size = size;
    }
}
