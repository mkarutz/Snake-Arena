using UnityEngine;
using System.Collections;

public class ControlSchemeButton : MonoBehaviour
{
	public int ControlSchemeCode = 0;
	public EnableDisableButton button;

	void Update() 
	{
		if (PlayerProfile.Instance().ControlScheme != ControlSchemeCode) {
			button.EnableButton();
		} else {
			button.DisableButton();
		}
	}


	public void OnClick()
	{
		PlayerProfile.Instance().ControlScheme = ControlSchemeCode;
	}
}
