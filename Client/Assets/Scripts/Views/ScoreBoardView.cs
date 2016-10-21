using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreBoardView : MonoBehaviour {

    public ScoreBoardState state;
    public float verticalSpacing = 5.0f;

    public PlayerScoreEntryView playerScoreTextPrefab;
    public PlayerScoreEntryView[] playerScoreTextElems;

    private int playerScore;
    private string playerName;

	// Use this for initialization
	void Start () {
        this.playerScore = 0;
        this.playerName = "~";
        this.playerScoreTextElems = new PlayerScoreEntryView[state.maxScoreboardPlayers + 1];
	    for (int i = 0; i < state.maxScoreboardPlayers + 1; i++)
        {
            this.playerScoreTextElems[i] = Instantiate<PlayerScoreEntryView>(playerScoreTextPrefab);
            this.playerScoreTextElems[i].transform.SetParent(this.transform, false);
            this.playerScoreTextElems[i].transform.localPosition = Vector3.zero;
            this.playerScoreTextElems[i].GetComponent<RectTransform>().anchoredPosition = Vector3.down * ((i + 1) * verticalSpacing + 10.0f);
            this.playerScoreTextElems[i].transform.localScale = Vector3.one;
            this.playerScoreTextElems[i].transform.localRotation = Quaternion.identity;
            if (i == state.maxScoreboardPlayers)
            {
                this.playerScoreTextElems[i].GetComponent<RectTransform>().anchoredPosition = Vector3.down * (0 * verticalSpacing);
                this.playerScoreTextElems[i].playerNameText.color = Color.red;
                this.playerScoreTextElems[i].playerScoreText.color = Color.red;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
        int rank = 0;
        int totalPlayers = 0;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            playerName = player.GetComponent<SnakeState>().name;
            playerScore = player.GetComponent<SnakeState>().score;
        }
        int nTaggedEnemies = GameObject.FindGameObjectsWithTag("SnakeBody").Length;
        if (nTaggedEnemies > 9)
        {
            totalPlayers = nTaggedEnemies;
        }
        for (int i = 0; i < this.state.maxScoreboardPlayers; i++)
        {
            this.playerScoreTextElems[i].playerNameText.text = this.state.playerScores[i].playerName;
            this.playerScoreTextElems[i].playerScoreText.text = this.state.playerScores[i].playerScore.ToString();

            if (this.state.playerScores[i].playerName == playerName)
                rank = i + 1;
            if (this.state.playerScores[i].playerName != "-" && nTaggedEnemies <= 9)
                totalPlayers++;
        }
        if (this.state.playerRank != -1)
            rank = this.state.playerRank;
        // Rank row
        this.playerScoreTextElems[this.state.maxScoreboardPlayers].playerNameText.text = "YOUR RANK (Score: " + this.playerScore + ")";
        this.playerScoreTextElems[this.state.maxScoreboardPlayers].playerScoreText.text = rank.ToString() + " / " + totalPlayers;
    }
}
