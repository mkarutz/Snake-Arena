using UnityEngine;
using System.Collections;

public class RadarSnakeController : MonoBehaviour {

    public SnakeState playerSnake;
    public GameWorld gameWorld;

    // Use this for initialization
    void Start () {
        this.gameWorld = GameObject.FindGameObjectWithTag("World").GetComponent<GameWorld>();
	}
	
	// Update is called once per frame
	void Update () {

        this.playerSnake = GameObject.FindGameObjectWithTag("Player").GetComponent<SnakeState>();
        if (this.playerSnake)
        {
            Vector2 normalizedPos = playerSnake.transform.position / gameWorld.worldRadius;

            RectTransform transform = this.GetComponent<RectTransform>();
            RectTransform parentTransform = this.gameObject.transform.parent.GetComponent<RectTransform>();
            transform.anchoredPosition = normalizedPos * (parentTransform.rect.width / 2.0f);
        }
	}
}
