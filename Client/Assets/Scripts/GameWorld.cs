using UnityEngine;
using System.Collections;

public class GameWorld : MonoBehaviour {

    public float worldRadius = GameConfig.WORLD_RADIUS;

    void Update()
    {
        this.transform.localScale = new Vector3(1.0f, 1.0f, 0.001f) * worldRadius * 2.0f;
    }
}
