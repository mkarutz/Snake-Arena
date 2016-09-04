using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SnakeState : MonoBehaviour {

    public GameObject head;

    public float snakeLength;
    public float snakeTailStartLength;
    public float speedFactor;

    private IList<Vector3> backbonePoints = new List<Vector3>();

    private Quaternion direction;
    private Quaternion targetDirection;

	// Use this for initialization
	void Start () {
        // Default backbone
        this.backbonePoints.Add(this.head.transform.position);
    }
	
	// Update is called once per frame
	void Update () {

        //this.head.transform.position += Vector3.right * Input.GetAxis("Horizontal") + Vector3.up * Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.UpArrow))
            this.targetDirection = Quaternion.LookRotation(Vector3.up, Vector3.back);
        if (Input.GetKey(KeyCode.DownArrow))
            this.targetDirection = Quaternion.LookRotation(Vector3.down, Vector3.back);
        if (Input.GetKey(KeyCode.LeftArrow))
            this.targetDirection = Quaternion.LookRotation(Vector3.left, Vector3.back);
        if (Input.GetKey(KeyCode.RightArrow))
            this.targetDirection = Quaternion.LookRotation(Vector3.right, Vector3.back);

        this.direction = Quaternion.RotateTowards(this.direction, this.targetDirection, 200.0f * Time.deltaTime);

        //this.head.transform.localRotation = Quaternion.identity;

        // Add to backbone path only if deviating (straight line movements don't need middle points)
        Vector3 headVec = this.backbonePoints.Last() - this.head.transform.position;
        if (headVec.sqrMagnitude > 0.00001f)
        {
            Vector3 firstSegmentVec;
            if (this.backbonePoints.Count >= 2)
            {
                firstSegmentVec = this.backbonePoints[this.backbonePoints.Count - 2] - this.backbonePoints.Last();

                if (Vector3.Angle(headVec, firstSegmentVec) > 5.0f)
                    backbonePoints.Add(this.head.transform.position);
                else
                    backbonePoints[this.backbonePoints.Count - 1] = this.head.transform.position;
            }
            else
                backbonePoints.Add(this.head.transform.position);
        }

        this.head.transform.localRotation = this.direction;
        this.head.transform.Translate(Vector3.forward * 10.0f * Time.deltaTime);
    }

    public Vector3 CalcBackboneParametizedPosition(float distance)
    {
        float accDistance = 0.0f;
        Vector3 prev = this.backbonePoints.Last();
        foreach (Vector3 curr in this.backbonePoints.Reverse())
        {
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
}
