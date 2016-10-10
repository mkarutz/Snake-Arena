using UnityEngine;
using System.Collections;
using slyther.flatbuffers;

public class ScoreBoardNetworkGameObject : INetworkGameObject {
    public ScoreBoardState scoreBoardState;

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
	}


	public override void Destroy()
	{
		GameObject.Destroy(gameObject);
	}
}
