using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AppAdvisory.SharingSystem;
using UnityEngine.SceneManagement;

public class Screenshotter : MonoBehaviour 
{
	public Button button;
	public VSSHARE vsshare;

	public void Start()
	{
		button.onClick.AddListener(TakeScreenshot);
	}

	public void TakeScreenshot()
	{
		vsshare.TakeScreenshot();
		Debug.Log("Took screenshot");
		StartCoroutine(ChangeSceneAfterSeconds(1));
	}

	IEnumerator ChangeSceneAfterSeconds(int seconds) {
		yield return new WaitForSeconds(seconds);
		SceneManager.LoadScene(0);
	}
}
