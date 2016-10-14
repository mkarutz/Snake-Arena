using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if VS_SHARE
namespace AppAdvisory.SharingSystem
{
	public class BtnTakeScreenshot : MonoBehaviour 
	{
		public bool AddButtonListenerFromCode = true;
		Button button;

		void Awake()
		{
			button = GetComponent<Button>();

			if(AddButtonListenerFromCode)
				button.onClick.AddListener(OnClickedButton);
		}

		public void OnClickedButton()
		{
			VSSHARE.OnScreenshotTaken += OnScreenshotTakenDelegate;


			VSSHARE.DOTakeScreenShot();



		}

		void OnScreenshotTakenDelegate(Texture2D tex)
		{
			VSSHARE.OnScreenshotTaken -= OnScreenshotTakenDelegate;
			Debug.Log("UnityEventListener - Screenshot taken!!");
		}

		void Update()
		{
			bool enabledIt = VSSHARE.GetButtonShareState() == ButtonShareState.isClosed;
			button.interactable = enabledIt;
		}


		void rien()
		{

			VSSHARE.OnButtonShareIsShareWindow += OnButtonShareIsClosed;

		}


		void OnButtonShareIsClosed()
		{
			Debug.Log("UnityEventListener - the screenshot button is closed!!");
		}
	}
}
#endif
