using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UpdatePlayerNickname : MonoBehaviour 
{
	public InputField nicknameField;

	public void Start()
	{
		nicknameField.text = PlayerProfile.Instance().Nickname;
	}

	public void OnEndEdit()
	{
		PlayerProfile.Instance().Nickname = nicknameField.text;
	}
}
