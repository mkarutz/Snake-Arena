using UnityEngine;
using System.Collections;
using AppAdvisory.SharingSystem;

public class ShareButtonHandler : MonoBehaviour {

    private int countdown = -1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (countdown == 0)
        {
            countdown = -1;
            VSSHARE.DOShareScreenshot("Look at my best score!");
        }
        else if (countdown > 0)
            countdown--;
	}

    public void OnButtonPress()
    {
        VSSHARE.DOTakeScreenShot();
        countdown = 5;
    }
}
