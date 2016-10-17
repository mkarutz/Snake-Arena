using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnableDisableButton : MonoBehaviour 
{
	public Button button;
	public Image buttonBackground;
	public Text buttonText;

	private Color textColor;
	private Color backgroundColor;

	void Start()
	{
		textColor = buttonText.color;
		backgroundColor = buttonBackground.color;
	}

	public void EnableButton() 
	{
		button.interactable = true;
		buttonBackground.color = backgroundColor;
		buttonText.color = textColor;
	}

	public void DisableButton() 
	{
		button.interactable = false;
		buttonBackground.color = backgroundColor / 1.5f;
		buttonText.color = textColor / 1.5f;
	}
}
