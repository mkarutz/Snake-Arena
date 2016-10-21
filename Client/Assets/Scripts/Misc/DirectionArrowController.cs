using UnityEngine;
using System.Collections;
using UnityEngine.VR;

public class DirectionArrowController : MonoBehaviour {

    public InputManager inputManager;
    public float heightOffPlane = 0.5f;

    private float distance;
    private float alpha;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        alpha -= 0.05f;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (PlayerProfile.Instance().ControlScheme == 2)
        {
            Destroy(this.gameObject);
        }
        if (player)
        {
            SnakeState playerSnake = player.GetComponent<SnakeState>();
            this.transform.localScale = playerSnake.GetSnakeThickness() * Vector3.one * 2.0f;
            this.transform.parent = player.transform;
            this.distance = Mathf.Log(1 + inputManager.TargetDirection().magnitude * 0.007f);
            this.transform.localPosition = new Vector3(0.0f, heightOffPlane, distance);
            //playerSnake.SnakeFogDistance()
            /*
            if (VRSettings.enabled)
            {
                this.gameObject.SetActive(true);
            }
            else
            {
                this.gameObject.SetActive(Input.touchCount > 0);
            }*/
            if (Input.anyKey)
                alpha = 1.0f;
        }
        this.GetComponent<MeshRenderer>().material.color = new Color(1,1,1,alpha);
	}
}
