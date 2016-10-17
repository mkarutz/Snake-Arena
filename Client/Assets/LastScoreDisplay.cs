using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LastScoreDisplay : MonoBehaviour 
{
	public Text text;

	void Update()
	{
		text.text = "" + PlayerProfile.Instance().LastScore;
	}
}
