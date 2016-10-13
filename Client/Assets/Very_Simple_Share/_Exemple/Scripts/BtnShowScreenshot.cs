using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if VS_SHARE
using AppAdvisory.SharingSystem;

public class BtnShowScreenshot : MonoBehaviour 
{
	public bool AddButtonListenerFromCode = true;

	Button button;

	void Awake()
	{
		button = GetComponent<Button>();

		if(AddButtonListenerFromCode)
			GetComponent<Button>().onClick.AddListener(OnClickedButton);
	}

	public void OnClickedButton()
	{
		bool canOpen = VSSHARE.DOOpenScreenshotButton();

		if(canOpen)
			Debug.Log("succefully opened screenshot because one screenshot is available");
		else
			Debug.LogWarning("We can't open screenshot if no screenshot is available");
	}

	void Update()
	{
		bool enabledIt = VSSHARE.haveScreenshotAvailable && VSSHARE.GetButtonShareState() == ButtonShareState.isClosed;
		button.interactable = enabledIt;
	}
}
#endif