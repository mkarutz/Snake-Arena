using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if VS_SHARE
namespace AppAdvisory.SharingSystem
{
	public class BtnHideScreenshotIcon : MonoBehaviour 
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


			VSSHARE.DOHideScreenshotIcon();


		}

		void Update()
		{
			bool enabledIt = VSSHARE.GetButtonShareState() == ButtonShareState.isIcon;
			button.interactable = enabledIt;
		}
	}
}
#endif