using UnityEngine;
using System.Collections;

public class ScoreBoardFactory : INetworkGameObjectFactory
{
	public INetworkGameObject scoreBoardPrefab;


	public override INetworkGameObject CreateGameObject()
	{
        INetworkGameObject g = GameObject.Instantiate(scoreBoardPrefab);
        g.gameObject.SetActive(false);
        return g;
	}
}
