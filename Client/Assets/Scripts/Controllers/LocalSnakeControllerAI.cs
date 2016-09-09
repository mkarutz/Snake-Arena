using UnityEngine;
using System.Collections;

public class LocalSnakeControllerAI : LocalSnakeController {

    private Vector3 targetPoint;

    protected override void MovementControl()
    {
        Vector3 dirVec = targetPoint - this.snakeState.head.transform.position;
        if (Random.Range(0.0f, 1.0f) < 0.02f || dirVec.magnitude < 3.0f)
        {
            targetPoint = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
            this.SetTargetDirection(dirVec.normalized);
        }
        
    }
}
