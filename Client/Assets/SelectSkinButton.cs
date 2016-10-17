using UnityEngine;
using System.Collections;

public class SelectSkinButton : MonoBehaviour {

    public int skinID;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public void OnButtonPress () {
        PlayerProfile.Instance().Skin = skinID;
	}

    void Update()
    {
        if (PlayerProfile.Instance().Skin == skinID)
            this.gameObject.GetComponent<EnableDisableButton>().DisableButton();
        else
            this.gameObject.GetComponent<EnableDisableButton>().EnableButton();
    }
}
