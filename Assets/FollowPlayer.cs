using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {
	public GameObject player;

	void Update () {
		transform.position = player.transform.position + Vector3.up * 50;
	}
}
