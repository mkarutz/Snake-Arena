using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using slyther.flatbuffers;

public class SnakeState : MonoBehaviour {
    public float speedFactor = 1.0f;
    public bool isTurbo;

    public int snakeSkinID;
    public int score;
    public string name;

    public static int MAX_BACKBONE_POINTS = 1000;
    public static int GROWTH_CAP = GameConfig.SNAKE_GROWTH_CAP;
    public static float MIN_LENGTH = 1.0f;
    public static float MIN_THICKNESS = 0.2f;
    public static float GROWTH_RATE = 1.0f / 100.0f;

	public static float MOVE_SPEED = 3.0f;
	public static float MAX_HEAD_OFFSET = 0.02f;

    public bool hardPosition = true;
    
    private Vector2[] backbone;
    private int backboneStartIdx;
    private int backboneLength;


    void Awake()
	{   
        this.InitBackbone();
		this.SetRandomStartBackbone (Vector2.zero);
    }


	public void SetRandomStartBackbone(Vector2 offset)
	{
		Vector2 randDir = new Vector2 (Random.Range (-1000, 1000), Random.Range (-1000, 1000)).normalized;
		this.AddBackboneHeadPoint(offset);
		this.AddBackboneHeadPoint(randDir + offset);
		this.AddBackboneHeadPoint(randDir * 2 + offset);
	}


    void Update()
    {
        if (hardPosition)
		    CenterPositionOnHead();
        UpdateCollider();
    }


	void OnDestroy()
	{
		UpdatePlayerHighScore();
	}


	private void UpdatePlayerHighScore()
	{
		if ("Player".Equals(gameObject.tag)) {
			PlayerProfile.Instance().AddScore(score);
		}
	}


    void UpdateCollider()
    {
        CircleCollider2D collider = this.gameObject.GetComponent<CircleCollider2D>();
        if (collider)
            collider.radius = this.GetSnakeThickness();
    }


	void CenterPositionOnHead()
	{
		transform.position = backbone[backboneStartIdx];
	}


    private void InitBackbone()
    {
        this.backbone = new Vector2[MAX_BACKBONE_POINTS];
        this.backboneStartIdx = 0;
        this.backboneLength = 0;
    }


	private float maxRotate()
	{
		return (360 * MOVE_SPEED) / (GameConfig.TURN_RADIUS_FACTOR * Mathf.PI * GetSnakeThickness());
	}


	private Quaternion DirectionVectorToQuaterion(Vector3 lookDirection)
	{
		return Quaternion.LookRotation(lookDirection, Vector3.back);
	}


	public void TurnTowards(Vector2 desiredMoveDirection, float dt) 
	{
		Quaternion desiredRotation = DirectionVectorToQuaterion(desiredMoveDirection);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, maxRotate() * dt);
	}


    public void MoveInDirection(Vector2 desiredMoveDirection, float dt)
    {
        if (desiredMoveDirection.sqrMagnitude > float.Epsilon)
        {
            TurnTowards(desiredMoveDirection, dt);
        }

        // Translate snake forward
        transform.Translate(Vector3.forward * MOVE_SPEED *speedFactor* dt);

        if (GetBackboneLength() <= 2)
        {
            Debug.LogError("Not enough backbone points defined.");
            return;
        }

        // Check if we need to add a new backbone point
        Vector2 headVec = GetBackbonePoint(1) - (Vector2)transform.position;
        Vector2 neckVec = GetBackbonePoint(2) - GetBackbonePoint(1);

        Vector2 a = Vector3.Project(headVec, neckVec);
        if (headVec.sqrMagnitude - a.sqrMagnitude > MAX_HEAD_OFFSET * MAX_HEAD_OFFSET)
        {
            AddBackboneHeadPoint(transform.position);
        }

        UpdateBackboneHeadPoint(transform.position);
    }


	public void Move(Vector2 desiredMovePosition, float dt) 
	{
		Vector2 desiredMoveDirection = desiredMovePosition - (Vector2) transform.position;
        MoveInDirection(desiredMoveDirection, dt);

	}


    public bool IsRunInto(SnakeState other)
    {
        if (other == this)
        {
            return false;
        }

        float distanceToOther = other.DistanceFrom(this.GetHeadPosition());
        return distanceToOther < other.GetSnakeThickness() / 2.0f;
    }


    //should be moved to math section
    public static float GetDistanceToLine(Vector2 point, Vector2 a, Vector2 b)
    {
        float dx = b.x - a.x;
        float dy = b.y - a.y;
        float area2 = Mathf.Abs(dy * point.x - dx * point.y + b.x * a.y - b.y * a.x);
        float dist = (float)Mathf.Sqrt(dy * dy + dx * dx);
        return area2 / dist;
    }


    public static bool IsPerpendicularToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        Vector2 aToB = b - a;
        Vector2 aToP = p - a;
        Vector2 pToB = b - p;

        return Vector2.Dot(aToB, pToB) > 0 && Vector2.Dot(aToB, aToP) > 0;
    }


    public float DistanceFrom(Vector2 point)
    {
        float minDistance = float.MaxValue;
        float delta = 0.5f;
        float d = delta;

        Vector2 curr = this.GetHeadPosition();
        Vector2 next = this.CalcBackboneParametizedPosition(d);

        Vector2 tailPointer = this.CalcBackboneParametizedPosition(this.GetSnakeLength());
        while (d <= this.GetSnakeLength())
        {
            Vector2 currPoint = curr;
            Vector2 nextPoint = next;

            if (!IsPerpendicularToSegment(point, currPoint, nextPoint))
            {
                d += delta;
                curr = next;
                next = this.CalcBackboneParametizedPosition(d);
                continue;
            }

            float distanceToLine = GetDistanceToLine(point, currPoint, nextPoint);
            minDistance = Mathf.Min(distanceToLine, minDistance);
            d += delta;
            curr = next;
            next = this.CalcBackboneParametizedPosition(d);
        }

        return minDistance;
    }


    private void DropFoodAtTail()
    {
        FoodState food = this.gameObject.GetComponent<LocalSnakeBody>().foodPrefab;
        GameObject.Instantiate<FoodState>(food);
        food.transform.position = this.CalcBackboneParametizedPosition(this.GetSnakeLength());
        food.weight = GameConfig.FOOD_WEIGHT_DROP_TURBO;
        this.score -= food.weight*2;

    }

    public void SetTurboLocal(bool isTurbo)
    {

        if (this.GetSnakeLength() > MIN_LENGTH + 1)
        {
            this.isTurbo = isTurbo;
            if (this.isTurbo)
            {
                this.speedFactor = GameConfig.TURBO_ON_SNAKE_SPEED_FACTOR;
                DropFoodAtTail();
            }
            else
            {
                this.speedFactor = GameConfig.TURBO_OFF_SNAKE_SPEED_FACTOR;
            }
        }
        else
        {
            this.isTurbo = false;
            this.speedFactor = GameConfig.TURBO_OFF_SNAKE_SPEED_FACTOR;
        }
        
       
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


    public Vector2 GetHeadPosition()
    {
        return this.backbone[this.backboneStartIdx];
    }

    
    public Vector2 GetTailPosition()
    {
        return (this.CalcBackboneParametizedPosition(this.GetSnakeLength()));
    }


    public int GetNextIdx(int curr)
    {
        return ((curr +1) % MAX_BACKBONE_POINTS);
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

        this.backboneLength = Mathf.Min(i + 3, this.backboneLength);
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


	public ClientsideInterpolation interpolater;

    public void ReplicateState(NetworkSnakeState state)
    {
		ReplicateHeadPointer(state.Head);
        
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

        for (int i = 0; i < state.PartsLength; i++)
        {
            state.GetParts(snakePartState, i);
			if (snakePartState.Index == backboneStartIdx) {
				// We want to Lerp the head towards the new position
				interpolater.targetHeadPosition = new Vector2(snakePartState.Position.X, snakePartState.Position.Y);
			} else {
				this.backbone[snakePartState.Index] = new Vector2(snakePartState.Position.X, snakePartState.Position.Y);
			}
        }
        score = (int) state.Score;
		gameObject.SetActive(!state.IsDead);
    }


	private void ReplicateHeadPointer(int headPointer)
	{
		int oldHeadPointer = backboneStartIdx;
		backbone[headPointer] = backbone[oldHeadPointer];
		backboneStartIdx = headPointer;
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

    public void SetBackbonePoint(int idx, Vector2 point)
    {
        this.backbone[(backboneStartIdx + idx) % MAX_BACKBONE_POINTS] = point;
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

    public float MaxSnakeThickness()
    {
        return MIN_THICKNESS + GROWTH_RATE * Mathf.Sqrt(GROWTH_CAP);
    }

    public float SnakeFogDistance()
    {
        return this.GetSnakeThickness() * GameConfig.SNAKE_FOG_MULTIPLIER;
    }

    // Head collision logic (todo: move)
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Food")
        {
            FoodState food = other.gameObject.GetComponent<FoodState>();
            this.score += food.CollectFood(this);
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "SnakeBody")
        {
            if (this.IsRunInto(other.gameObject.GetComponent<SnakeStateReference>().snakeState))
            {
                Destroy(this.gameObject);
            }
        }
    }

}
