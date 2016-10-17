using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoToMainMenuScene : MonoBehaviour 
{
	public void OnClick() 
	{
		SceneManager.LoadScene("MainMenu");
	}
}
