using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoToMultiplayerScene : MonoBehaviour
{
	public void OnClick()
	{
		SceneManager.LoadScene("InGameMultiplayer");
	}
}
