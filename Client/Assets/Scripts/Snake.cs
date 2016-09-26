using UnityEngine;
using System.Collections;
using slyther.flatbuffers;

public class Snake : MonoBehaviour {
	public SnakePart snakePartPrefab;

	public const int MAX_PARTS = 100;

	private SnakePart[] parts = new SnakePart[MAX_PARTS];
	private int headPointer;
	private int tailPointer;
	private int score;

	void Awake() {
		
	}

	void Start() {

	}

	void Update() {

	}

	private SnakePartStateFB snakePartState = new SnakePartStateFB ();

	public void replicateState(SnakeStateFB snakeState) {
		headPointer = snakeState.Head;
		tailPointer = snakeState.Tail;

		for (int i = 0; i < snakeState.PartsLength; i++) {
			snakePartState = snakeState.GetParts(snakePartState, i);
			int partIndex = snakePartState.Index;
			parts[partIndex].replicateState (snakePartState);
		}
	}

	private void initSnakeParts() {
		for (int i = 0; i < MAX_PARTS; i++) {
			parts[i] = newSnakePart();
		}
	}

	private SnakePart newSnakePart() {
		SnakePart snakePart = (SnakePart) GameObject.Instantiate(snakePartPrefab);
		return snakePart;
	}


	public SnakePart[] getParts() {
        return parts;
    }

    public int getHeadPointer() {
        return headPointer;
    }

    public void setHeadPointer(int headPointer) {
        this.headPointer = headPointer;
    }

    public int getTailPointer() {
        return tailPointer;
    }

    public void setTailPointer(int tailPointer) {
        this.tailPointer = tailPointer;
    }

	public int getScore() {
        return score;
    }

    public void setScore(int score) {
        this.score = score;
    }
}
