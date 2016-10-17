using UnityEngine;
using System.Collections;
using beffio.OneMinuteGUI;

public class ShowDialogueOnDisconnect : MonoBehaviour 
{
	public NetworkController networkController;
	public GameObject serverDisconnectedDiaglogue;
	public MenuManager menuManager;

	private bool connectionHasBeenEstablished = false;
	private bool hasDisconnected = false;

	void Update () 
	{
		if (hasDisconnected) {
			return;
		}

		if (!connectionHasBeenEstablished && networkController.IsConnected()) {
			connectionHasBeenEstablished = true;
		}

		if (connectionHasBeenEstablished && !networkController.IsConnected()) {
			hasDisconnected = true;
			menuManager.GoToMenu(serverDisconnectedDiaglogue);
		}
	}
}
