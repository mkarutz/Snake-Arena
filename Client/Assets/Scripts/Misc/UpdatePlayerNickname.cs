using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.VR;

public class UpdatePlayerNickname : MonoBehaviour 
{
	public InputField nicknameField;

	public void Start()
	{
		nicknameField.text = PlayerProfile.Instance().Nickname;
        if (VRSettings.enabled)
        {
            nicknameField.text = GiveSnakeRandomName.GenerateRandomName();
        }
	}

	public void OnEndEdit()
	{
		PlayerProfile.Instance().Nickname = nicknameField.text;
	}
}
