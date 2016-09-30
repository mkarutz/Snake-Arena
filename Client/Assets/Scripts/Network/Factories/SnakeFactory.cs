using UnityEngine;
using System.Collections;

public class SnakeFactory : INetworkGameObjectFactory {
	public INetworkGameObject snakePrefab;


	public override INetworkGameObject CreateGameObject()
	{
		return GameObject.Instantiate(snakePrefab);
	}
}
