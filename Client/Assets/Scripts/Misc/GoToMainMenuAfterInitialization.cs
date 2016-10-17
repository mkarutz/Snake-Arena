using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GoToMainMenuAfterInitialization : MonoBehaviour 
{
	void Start () 
	{
		SceneManager.LoadScene("MainMenu");
	}
}
