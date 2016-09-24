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
        //{
        //    this.boundingBox = this.gameObject.AddComponent<BoxCollider>();
        //    updateBoundingBox();         
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
        //Vector2 prevPosition = this.snakeState.head.transform.position;
        this.snakeState.head.transform.Translate(Vector3.forward * MOVE_SPEED * Time.deltaTime);
        //this.snakeState.UpdateBackboneHeadPoint(this.snakeState.head.transform.position);

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
            //Vector3 newPoint = this.snakeState.GetBackbonePoint(1) + a + (a - headVec).normalized * MAX_HEAD_OFFSET;
            //this.snakeState.UpdateBackboneHeadPoint(prevPosition);
            this.snakeState.AddBackboneHeadPoint(this.snakeState.head.transform.position);
            //snakeState.head.transform.position = newPoint;
            //GameObject o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //o.transform.position = this.snakeState.GetBackbonePoint(1);
            //o.transform.localScale = Vector3.one * 0.1f;
        }

        this.snakeState.UpdateBackboneHeadPoint(this.snakeState.head.transform.position);
    }

    public Component getControllerComponent()
    {
        return this;
    }

    public Rect getBoundingBox()
    {
        float snakeLength = this.snakeState.GetSnakeLength();
        float snakeRadius = this.snakeState.GetSnakeThickness() / 2.0f;
        float snakeVertexDensity = 3.0f / snakeRadius;
        //IList<Vector3> backBone = snakeState.getBackbone();

        int backboneLength = (int)(snakeLength * snakeVertexDensity);
        if (backboneLength <= 0)
            return new Rect(0,0,0,0);
        Vector3[] backbone = new Vector3[backboneLength];

        float distance = 0.0f;
        for (int i = 0; i < backboneLength; i++)
        {
            distance = ((float)i / backboneLength) * snakeLength;
            backbone[i] = this.snakeState.CalcBackboneParametizedPosition(distance);
        }

        float minX = 0;
        float minY = 0;
        float maxX = 0;
        float maxY = 0;
        int flag = 0;

        foreach (Vector3 vec in backbone)
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

        LineRenderer lr = this.gameObject.GetComponent<LineRenderer>();
        if(lr == null)
        {
            lr = this.gameObject.AddComponent<LineRenderer>();
        }
        Vector3[] positions = new Vector3[4];
        positions[0] = new Vector3(bound.xMin, bound.yMin);
        positions[1] = new Vector3(bound.xMin, bound.yMax);
        positions[2] = new Vector3(bound.xMax, bound.yMin);
        positions[3] = new Vector3(bound.xMax, bound.yMax);
        lr.SetPositions(positions);
        
        // Vector3 centre = new Vector3(bound.xMin + ((bound.xMax - bound.xMin) / 2), bound.yMin + ((bound.yMax - bound.yMin) / 2), 0.0f);
       // Vector3 size = new Vector3(Mathf.Abs(bound.xMax) - Mathf.Abs(bound.xMin), Mathf.Abs(bound.yMax) - Mathf.Abs(bound.yMin), 0.0f);

      
       // this.boundingBox.center = centre;
       // this.boundingBox.size = Vector3.one;// size;
    }
}
