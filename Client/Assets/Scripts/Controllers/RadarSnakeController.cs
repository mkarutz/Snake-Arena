using UnityEngine;
using System.Collections;

public class RadarSnakeController : MonoBehaviour {

    public NetworkController networkController;
    public GameWorld gameWorld;

    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
        GameObject player = this.networkController.GetLocalPlayer().gameObject;
        Vector2 normalizedPos = player.transform.position / gameWorld.worldRadius;

        RectTransform transform = this.GetComponent<RectTransform>();
        RectTransform parentTransform = this.gameObject.transform.parent.GetComponent<RectTransform>();
        transform.anchoredPosition = normalizedPos * (parentTransform.rect.width / 2.0f);
	}
}
