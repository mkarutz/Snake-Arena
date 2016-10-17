using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BestScoreDisplay : MonoBehaviour 
{
	public Text text;

	void Update()
	{
		text.text = "" + PlayerProfile.Instance().BestScore;
	}
}
