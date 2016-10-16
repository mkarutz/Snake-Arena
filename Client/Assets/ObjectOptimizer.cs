using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectOptimizer : MonoBehaviour {

    public HashSet<GameObject> inactiveObjectsToCheck;
    public float radiusFactor = 1.5f;

    private int countDown = 0;

    // Use this for initialization
    void Start () {
        inactiveObjectsToCheck = new HashSet<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
        if (countDown > 0)
        {
            countDown--;
            return;
        }
        DisableDistantObjects();
        EnableCloseObjects();
        countDown = 10;
	}

    private void DisableDistantObjects()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (!player)
            return;
        SnakeState playerSnake = player.GetComponent<SnakeState>();

        GameObject[] foodObjects = GameObject.FindGameObjectsWithTag("Food");
        foreach (GameObject food in foodObjects)
        {
            if (Vector2.Distance(food.transform.position, playerSnake.transform.position) > playerSnake.SnakeFogDistance() * 0.01f)
            {
                food.SetActive(false);
                inactiveObjectsToCheck.Add(food);
            }
        }
    }

    public void EnableCloseObjects()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (!player)
            return;
        SnakeState playerSnake = player.GetComponent<SnakeState>();

        List<GameObject> objectsToRemove = new List<GameObject>();
        foreach (GameObject obj in this.inactiveObjectsToCheck)
        {
            if (Vector2.Distance(obj.transform.position, playerSnake.transform.position) < playerSnake.SnakeFogDistance() * radiusFactor)
            {
                obj.SetActive(true);
                objectsToRemove.Add(obj);
            }
        }

        foreach (GameObject obj in objectsToRemove)
        {
            inactiveObjectsToCheck.Remove(obj);
        }
    }
}
