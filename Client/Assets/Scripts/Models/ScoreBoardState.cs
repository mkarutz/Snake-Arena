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

	// Use this for initialization
	void Start () {
        playerScores = new PlayerScoreEntry[this.maxScoreboardPlayers];
        playerScores[0].playerName = "Alex";
        playerScores[0].playerScore = 100000000;
        /*for (int i = 0; i < maxScoreboardPlayers; i++)
        {
            this.playerScores[i] = new PlayerScoreEntry();
        }*/
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
