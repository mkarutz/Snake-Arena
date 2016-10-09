using UnityEngine;
using System.Collections;

public class ScoreBoardFactory : INetworkGameObjectFactory
{
	public INetworkGameObject scoreBoardPrefab;


	public override INetworkGameObject CreateGameObject()
	{
		return GameObject.Instantiate(scoreBoardPrefab);
	}
}
