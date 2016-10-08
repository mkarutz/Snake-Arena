using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreBoardState : MonoBehaviour {

    public struct PlayerScoreEntry {
        public string playerName;
        public int playerScore;
    }

    public int maxScoreboardPlayers = GameConfig.MAX_SCOREBOARD_PLAYERS;
    public PlayerScoreEntry[] playerScores;

    public void SetEntry(int pos, string playerName, int playerScore)
    {
        playerScores[pos].playerName = playerName;
        playerScores[pos].playerScore = playerScore;
    }

	// Use this for initialization
	void Start () {
        playerScores = new PlayerScoreEntry[this.maxScoreboardPlayers];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
