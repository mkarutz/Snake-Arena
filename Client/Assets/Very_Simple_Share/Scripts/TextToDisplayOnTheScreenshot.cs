using UnityEngine;
using System.Collections;

#if VS_SHARE
namespace AppAdvisory.SharingSystem
{
	public class TextToDisplayOnTheScreenshot : MonoBehaviour 
	{
		public void SetTextToDisplayOnTheScreenshot(string text)
		{
			GetComponent<UnityEngine.UI.Text>().text = text;
		}
	}
}
#endif