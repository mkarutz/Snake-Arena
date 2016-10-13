using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using slyther.flatbuffers;

public class SnakeState : MonoBehaviour {
    public float speedFactor;
    public bool isTurbo;

    public int snakeSkinID;
    public int score;
    public string name;

    public static int MAX_BACKBONE_POINTS = 1000;
    public static int GROWTH_CAP = 40000;
    public static float MIN_LENGTH = 1.0f;
    public static float MIN_THICKNESS = 0.2f;
    public static float GROWTH_RATE = 1.0f / 100.0f;

	public static float MOVE_SPEED = 3.0f;
	public static float MAX_HEAD_OFFSET = 0.02f;
    
    private Vector2[] backbone;
    private int backboneStartIdx;
    private int backboneLength;

	public float LerpAmount = 0.5f;

    void Awake()
	{   
        this.InitBackbone();
		this.SetRandomStartBackbone (Vector2.zero);

		//Despawn();
    }



	public void SetRandomStartBackbone(Vector2 offset)
	{
		Vector2 randDir = new Vector2 (Random.Range (-1000, 1000), Random.Range (-1000, 1000)).normalized;
		this.AddBackboneHeadPoint(offset);
		this.AddBackboneHeadPoint(randDir + offset);
		this.AddBackboneHeadPoint(randDir * 2 + offset);
	}

	public void Despawn()
	{
		gameObject.SetActive(false);
	}

	public void Respawn()
	{
		gameObject.SetActive(true);
	}

    void Update()
    {
		//InterpolateHeadPosition();
		CenterPositionOnHead();
        UpdateCollider();

    }

    void UpdateCollider()
    {
        CircleCollider2D collider = this.gameObject.GetComponent<CircleCollider2D>();
        collider.radius = this.GetSnakeThickness();
    }

	void CenterPositionOnHead()
	{
		transform.position = backbone[backboneStartIdx];
	}


	Vector3 targetHeadPosition = Vector2.zero;
	private void InterpolateHeadPosition()
	{
		backbone[backboneStartIdx] = Vector2.Lerp(backbone[backboneStartIdx], targetHeadPosition, LerpAmount);
	}


    private void InitBackbone()
    {
        this.backbone = new Vector2[MAX_BACKBONE_POINTS];
        this.backboneStartIdx = 0;
        this.backboneLength = 0;
    }


	private float maxRotate()
	{
		return (360 * MOVE_SPEED) / (4.0f * Mathf.PI * GetSnakeThickness());
	}


	private Quaternion DirectionVectorToQuaterion(Vector3 lookDirection)
	{
		return Quaternion.LookRotation(lookDirection, Vector3.back);
	}


	private void TurnTowards(Vector2 desiredMoveDirection, float dt) 
	{
		Quaternion desiredRotation = DirectionVectorToQuaterion(desiredMoveDirection);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, maxRotate() * dt);
	}


	public void Move(Vector2 desiredMovePosition, float dt) 
	{
		Vector2 desiredMoveDirection = desiredMovePosition - (Vector2) transform.position;
		if (desiredMoveDirection.sqrMagnitude > float.Epsilon)
		{
			TurnTowards(desiredMoveDirection, dt);
		}
			
		// Translate snake forward
		transform.Translate(Vector3.forward * MOVE_SPEED * dt);

		if (GetBackboneLength() <= 2)
		{
			Debug.LogError("Not enough backbone points defined.");
			return;
		}

		// Check if we need to add a new backbone point
		Vector2 headVec = GetBackbonePoint(1) - (Vector2) transform.position;
		Vector2 neckVec = GetBackbonePoint(2) - GetBackbonePoint(1);

		Vector2 a = Vector3.Project(headVec, neckVec);
		if (headVec.sqrMagnitude - a.sqrMagnitude > MAX_HEAD_OFFSET * MAX_HEAD_OFFSET)
		{
			AddBackboneHeadPoint(transform.position);
		}

		UpdateBackboneHeadPoint(transform.position);
	}


    public Bounds LocalBounds()
    {
        Bounds b = new Bounds();
        float radius = GetSnakeThickness() / 2.0f;
        for (int i = 0; i < GetBackboneLength(); i++)
        {
            Vector2 pt = GetBackbonePoint(i);
            b.Encapsulate(transform.InverseTransformPoint(pt + new Vector2(radius, radius)));
            b.Encapsulate(transform.InverseTransformPoint(pt + new Vector2(-radius, radius)));
            b.Encapsulate(transform.InverseTransformPoint(pt + new Vector2(radius, -radius)));
            b.Encapsulate(transform.InverseTransformPoint(pt + new Vector2(-radius, -radius)));
        }

        return b;
    }

    public Rect GetSnakeRect()
    {
        Bounds localBound = this.LocalBounds();
        float height = localBound.size.y;
        float width = localBound.size.x;
        //Debug.Log(localBound.min);
        //Debug.Log(localBound.max);
        Rect rect = new Rect(localBound.min.x, localBound.min.y, width, height);
        return rect;
    }

    public Vector2 GetHeadPosition()
    {
        return this.backbone[this.backboneStartIdx];
    }

//    public int GetHeadIdx()
//    {
//        return this.backboneStartIdx;
//    }

    public Vector2 GetTailPosition()
    {
        return (this.CalcBackboneParametizedPosition(this.GetSnakeLength()));
    }

    public int GetNextIdx(int curr)
    {
        return ((curr +1) % MAX_BACKBONE_POINTS);
    }
   

//    public bool isRunInto(SnakeState other)
//    {
//        if (other == this)
//        {
//            return false;
//        }

//        if (!other.GetSnakeRect().Contains(this.backbone[this.backboneStartIdx]))
//        {
//            return false;
//        }
//        float distanceToOther = other.distanceFrom(this.backbone[this.backboneStartIdx]);
//        return distanceToOther < other.GetSnakeThickness() / 2.0f;
//    }

//    public float distanceFrom(Vector2 point)
//    {
//        float minDistance = float.MaxValue;

//        int curr = this.GetHeadIdx();
//        int next = GetNextIdx(this.GetHeadIdx());

//        int tailPointer = this.GetTailIdx();
//        for (; next != tailPointer; curr = next, next = GetNextIdx(next))
//        {
//            Vector2 currPoint = this.backbone[curr];
//            Vector2 nextPoint = this.backbone[next];

//            if (Vector2.Distance(currPoint, nextPoint) < 10e-7)
//            {
//                continue;
//            }

//            if (!isPerpendicularToSegment(point, currPoint, nextPoint))
//            {
//                continue;
//            }

//            float distanceToLine = GetDistanceToLine(point, currPoint, nextPoint);
//            minDistance = Mathf.Min(distanceToLine, minDistance);
//        }

//        return minDistance;
//    }

    //should be moved to math section
    public static float GetDistanceToLine(Vector2 point, Vector2 a, Vector2 b)
    {
        float dx = b.x - a.x;
        float dy = b.y - a.y;
        float area2 = Mathf.Abs(dy * point.x - dx * point.y + b.x * a.y - b.y * a.x);
        float dist = (float)Mathf.Sqrt(dy * dy + dx * dx);
        return area2 / dist;
    }

    public static bool isPerpendicularToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 aToB = b - a;
        Vector2 aToP = p - a;
        Vector2 pToB = b - p;

        return Vector2.Dot(aToB, pToB) > 0 && Vector2.Dot(aToB, aToP) > 0;
    }
    private void TrimBackbone()
    {
        if (this.backboneLength <= 10)
            return;

        float accDistance = 0.0f;
        Vector3 prev = this.GetBackbonePoint(0);
        int i;
        for (i = 1; i < this.backboneLength; i++)
        {
            Vector3 curr = this.GetBackbonePoint(i);
            Vector3 segmentVector = curr - prev;
            float segmentMagnitude = segmentVector.magnitude;
            accDistance += segmentMagnitude;
            if (accDistance > this.GetSnakeLength())
                break;
            prev = curr;
        }

        this.backboneLength = i + 1;
    }

    public void AddBackboneHeadPoint(Vector2 point)
    {
        if (backboneLength >= MAX_BACKBONE_POINTS - 1)
        {
            Debug.LogError("Backbone point maximum exceeded.");
            return;
        }
        backboneStartIdx = backboneStartIdx - 1;
        if (backboneStartIdx < 0) backboneStartIdx = MAX_BACKBONE_POINTS - 1;
        backbone[backboneStartIdx] = point;
        backboneLength++;
        
        TrimBackbone();
    }

    public void ReplicateState(NetworkSnakeState state)
    {
        this.backboneStartIdx = state.Head;
        
        if (state.Tail <= state.Head)
        {
            this.backboneLength = state.Tail + MAX_BACKBONE_POINTS - state.Head;
        }
        else
        {
            this.backboneLength = state.Tail - state.Head;
        }

        this.isTurbo = state.IsTurbo;
        
		NetworkSnakePartState snakePartState = new NetworkSnakePartState();

		Debug.Log("num parts = " + state.PartsLength);
        for (int i = 0; i < state.PartsLength; i++)
        {
            state.GetParts(snakePartState, i);
			//if (snakePartState.Index == backboneStartIdx) {
				// We want to Lerp the head towards the new position
			//	targetHeadPosition = new Vector2(snakePartState.Position.X, snakePartState.Position.Y);
			//} else {
				this.backbone[snakePartState.Index] = new Vector2(snakePartState.Position.X, snakePartState.Position.Y);
			//}
        }

		score = (int) state.Score;
		gameObject.SetActive(!state.IsDead);
    }

    public void UpdateBackboneHeadPoint(Vector2 point)
    {
        this.backbone[this.backboneStartIdx] = point;
    }

    public int GetBackboneLength()
    {
        return this.backboneLength;
    }

    public Vector2[] GetRawBackboneArray()
    {
        return this.backbone;
    }

    public int GetRawBackboneStartIdx()
    {
        return this.backboneStartIdx;
    }

    public Vector2 GetBackbonePoint(int idx)
    {
        return this.backbone[(backboneStartIdx + idx) % MAX_BACKBONE_POINTS];
    }

    public Vector2 GetLocalBackbonePoint(int idx)
    {
        return GetBackbonePoint(idx) - (Vector2) transform.position;
    }

    public Vector3 CalcBackboneParametizedPosition(float distance)
    {
        if (this.backboneLength == 0)
            return Vector3.zero;

        float accDistance = 0.0f;
        Vector3 prev = this.GetBackbonePoint(0);
        for (int i = 0; i < this.backboneLength; i++)
        {
            Vector3 curr = this.GetBackbonePoint(i);
            Vector3 segmentVector = curr - prev;
            float segmentMagnitude = segmentVector.magnitude;
            if (accDistance + segmentMagnitude >= distance)
            {
                float t = distance - accDistance;
                return prev + segmentVector.normalized * t;
            }
            else
            {
                accDistance += segmentMagnitude;
            }
            prev = curr;
        }
        return Vector3.zero;
    }

    public float CappedScore()
    {
        return Mathf.Min(GROWTH_CAP, this.score);
    }

    public float GetSnakeLength()
    {
        return MIN_LENGTH + GROWTH_RATE * CappedScore();
    }

    public float GetSnakeThickness()
    {
        return MIN_THICKNESS + GROWTH_RATE * Mathf.Sqrt(CappedScore());
    }

    public float MaxSnakeLength()
    {
        return MIN_LENGTH + GROWTH_RATE * GROWTH_CAP;
    }

    public float SnakeFogDistance()
    {
        return this.GetSnakeThickness() * GameConfig.SNAKE_FOG_MULTIPLIER;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Food")
        {
            FoodState food = other.gameObject.GetComponent<FoodState>();
            this.score += food.weight;
            food.Despawn();
        }
    }

}
