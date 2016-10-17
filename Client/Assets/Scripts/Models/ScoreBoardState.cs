using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreBoardState : MonoBehaviour {

    public struct PlayerScoreEntry {
        public string playerName;
        public int playerScore;
    }

    public int maxScoreboardPlayers = GameConfig.MAX_SCOREBOARD_PLAYERS;
    public int playerRank = -1;
    public PlayerScoreEntry[] playerScores;

    public void SetEntry(int pos, string playerName, int playerScore)
    {
        playerScores[pos].playerName = playerName;
        playerScores[pos].playerScore = playerScore;
    }

    public void SetPlayerRank(int rank)
    {
        playerRank = rank;
    }

	// Use this for initialization
	void Start () {
        playerScores = new PlayerScoreEntry[this.maxScoreboardPlayers];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
