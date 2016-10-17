using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;
using UnityEngine.SceneManagement;

public class ShowAd : MonoBehaviour 
{
    private int showAdCountdown;

	public void Start()
	{
		if (!Advertisement.isInitialized) {
			Advertisement.Initialize("1167791", true);
		}
        showAdCountdown = -1;
	}


	public void Update()
	{
		ShowAdIfGameOver();
        if (showAdCountdown == 0)
        {
            ShowAdvertisement();
            showAdCountdown = -1;
        }
        else if (showAdCountdown > 0)
        {
            showAdCountdown--;
        }
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
            showAdCountdown = 60;
			adHasBeenShown = true;
		}
	}


	public void ShowAdvertisement()
	{
		if (PlayerProfile.Instance().AdsDisabled) {
			SceneManager.LoadScene("MainMenu");
		} else {
			StartCoroutine(Show());
		}
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

			SceneManager.LoadSceneAsync("MainMenu");
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