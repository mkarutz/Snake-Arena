using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;
using UnityEngine.SceneManagement;

public class ShowAd : MonoBehaviour 
{
	public void Start()
	{
		if (!Advertisement.isInitialized) {
			Advertisement.Initialize("1167791", true);
		}
	}


	public void Update()
	{
		ShowAdIfGameOver();
	}


	bool playerHasSpawned = false;
	bool adHasBeenShown = false;

	private void ShowAdIfGameOver()
	{
		GameObject localPlayer = GameObject.FindWithTag("Player");

		if (!playerHasSpawned && localPlayer != null) {
			playerHasSpawned = true;
		}

		if (playerHasSpawned && !adHasBeenShown && localPlayer == null) {
			ShowAdvertisement();
			adHasBeenShown = true;
		}
	}


	public void ShowAdvertisement()
	{
		StartCoroutine(Show());
	}


	public IEnumerator Show()
	{
		while (!Advertisement.IsReady()) 
		{
			yield return new WaitForSeconds(0.1f);
		}

		ShowRewardedAd();
	}

	public void ShowRewardedAd()
	{
		if (Advertisement.IsReady("rewardedVideo"))
		{
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show("rewardedVideo", options);
		}
	}

	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log("The ad was successfully shown.");

			SceneManager.LoadScene("MainMenu");
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}
}