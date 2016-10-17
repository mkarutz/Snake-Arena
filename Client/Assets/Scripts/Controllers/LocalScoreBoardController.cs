using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class LocalScoreBoardController : MonoBehaviour {
    private ScoreBoardState scoreBoardState;
    private SnakeScoreComparer snakeScoreComparer;

    private class SnakeScoreComparer : IComparer<SnakeState>
    {
        public int Compare(SnakeState x, SnakeState y)
        {
            return y.score - x.score;
        }
    }

    void Start()
    {
        this.scoreBoardState = GameObject.FindGameObjectWithTag("ScoreBoard").GetComponent<ScoreBoardState>();
        this.snakeScoreComparer = new SnakeScoreComparer();
    }

    // Update is called once per frame
    void Update () {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Snake");

        SnakeState[] snakes = new SnakeState[enemies.Length + 1];
        for (int i = 0; i < enemies.Length; i++)
        {
            snakes[i] = enemies[i].GetComponent<SnakeState>();
        }
        snakes[enemies.Length] = player.GetComponent<SnakeState>();
        Array.Sort(snakes, this.snakeScoreComparer);

        int playerRank = -1;
        int j = 1;
        foreach (SnakeState snake in snakes)
        {
            if (snake.gameObject == player)
                playerRank = j;
            j++;
        }

        for (int i = 0; i < this.scoreBoardState.maxScoreboardPlayers; i++)
        {
            if (i < snakes.Length)
            {
                this.scoreBoardState.SetEntry(i, snakes[i].name, snakes[i].score);
            }
            else
            {
                this.scoreBoardState.SetEntry(i, "-", 0);
            }
        }

        this.scoreBoardState.SetPlayerRank(playerRank);
	}
}
