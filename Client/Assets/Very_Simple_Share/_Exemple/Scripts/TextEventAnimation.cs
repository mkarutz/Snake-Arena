using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if VS_SHARE
namespace AppAdvisory.SharingSystem
{
	public class TextEventAnimation : MonoBehaviour 
	{
		Text text;

		void Awake()
		{
			text = GetComponent<Text>();
			text.text = "Animation - ";
		}

		void SetText(string _s)
		{
			text.text = "Animation - " + _s;

//			print(_s);
		}

		void OnEnable()
		{
			VSSHARE.OnButtonShareIsClosed += OnButtonShareIsClosed;
			VSSHARE.OnButtonShareisIcon += OnButtonShareisIcon;
			VSSHARE.OnButtonShareIsShareWindow += OnButtonShareIsShareWindow;
		}

		void OnButtonShareIsClosed()
		{
			SetText("button share is closed");
		}

		void OnButtonShareisIcon()
		{
			SetText("button share is icon");
		}

		void OnButtonShareIsShareWindow()
		{
			SetText("button share is share window");
		}
	}
}
#endif