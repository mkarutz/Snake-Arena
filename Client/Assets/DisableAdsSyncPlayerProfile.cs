using UnityEngine;
using System.Collections;

public class DisableAdsSyncPlayerProfile : MonoBehaviour 
{
	public EnableDisableButton button;

	void Update () 
	{
		if (!PlayerProfile.Instance().AdsDisabled) {
			button.EnableButton();
		} else {
			button.DisableButton();
		}
	}
}
