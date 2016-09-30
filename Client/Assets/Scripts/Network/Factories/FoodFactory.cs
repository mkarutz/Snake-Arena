using UnityEngine;
using System.Collections;

public class FoodFactory : INetworkGameObjectFactory
{
	public INetworkGameObject foodPrefab;


	public override INetworkGameObject CreateGameObject()
	{
		return GameObject.Instantiate(foodPrefab);
	}
}
