using UnityEngine;
using UnityEngine.VR;
using System.Collections;

public class GameModuleGenerator : MonoBehaviour {
    
    public GameObject moduleVRPrefab;
    public GameObject module2DPrefab;

	// Use this for initialization
	void Awake () {
        if (VRSettings.enabled)
            Instantiate(moduleVRPrefab);
        else
            Instantiate(module2DPrefab);
	}
}
