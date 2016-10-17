using UnityEngine;
using System.Collections;
using slyther.flatbuffers;

public class ScoreBoardNetworkGameObject : INetworkGameObject {
    private ScoreBoardState scoreBoardState;

    void Awake ()
    {
        this.scoreBoardState = GameObject.FindGameObjectWithTag("ScoreBoard").GetComponent<ScoreBoardState>();
    }

	public override void Replicate(NetworkObjectState state)
	{
        NetworkScoreBoardState networkScoreboardState = new NetworkScoreBoardState();
        state.GetState<NetworkScoreBoardState>(networkScoreboardState);

        ScoreboardEntry scoreBoardEntry = new ScoreboardEntry();
        for (int i = 0; i < networkScoreboardState.EntriesLength; i++)
        {
            networkScoreboardState.GetEntries(scoreBoardEntry, i);
           
            scoreBoardState.SetEntry(i, scoreBoardEntry.PlayerName, scoreBoardEntry.Score);
 
        }
        
        for (int i = networkScoreboardState.EntriesLength; i < scoreBoardState.maxScoreboardPlayers; i++)
        {
            scoreBoardState.SetEntry(i, "-", 0);
        }
        
    }


	public override void Destroy()
	{
		GameObject.Destroy(gameObject);
	}
}
