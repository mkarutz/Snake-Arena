using UnityEngine;
using System.Collections;
using Slyther;

public class CameraFollowLocalPlayer : MonoBehaviour {
	public ClientNetworkManager networkManager;

	void Update () {
		transform.position = networkManager.getLocalPlayer().transform.position + Vector3.back;
	}
}
