using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public class EnabledInVROnly : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (!VRSettings.enabled)
            this.gameObject.SetActive(false);
	}
}
