using UnityEngine;
using System.Collections;

public class EnableAdsButtonSyncPlayerProfile : MonoBehaviour 
{
	public EnableDisableButton button;

	void Update () 
	{
		if (PlayerProfile.Instance().AdsDisabled) {
			button.EnableButton();
		} else {
			button.DisableButton();
		}
	}
}
