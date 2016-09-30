using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LinkingContext : MonoBehaviour {
	private Dictionary<int, INetworkGameObject> networkIdToGameObjectDictionary = new Dictionary<int, INetworkGameObject>();
	private Dictionary<INetworkGameObject, int> gameObjectToNetworkIdDictionary = new Dictionary<INetworkGameObject, int>();


	public INetworkGameObject GetGameObject(int networkId)
	{
		if (!networkIdToGameObjectDictionary.ContainsKey(networkId)) {
			return null;
		}
		return networkIdToGameObjectDictionary[networkId];
	}


	public int GetNetworkId(INetworkGameObject go, bool shouldCreateIfNotFound)
	{
		if (gameObjectToNetworkIdDictionary.ContainsKey(go)) {
			return gameObjectToNetworkIdDictionary[go];
		} else if (shouldCreateIfNotFound) {
			AddGameObject(go);
			return gameObjectToNetworkIdDictionary[go];
		}
		return 0;
	}


	private int nextNetworkId = 1;

	private void AddGameObject(INetworkGameObject go)
	{
		AddGameObject(go, nextNetworkId++);
	}


	public void AddGameObject(INetworkGameObject go, int networkId)
	{
		networkIdToGameObjectDictionary.Add(networkId, go);
		gameObjectToNetworkIdDictionary.Add(go, networkId);
	}


	public void RemoveGameObject(INetworkGameObject go)
	{
		if (!gameObjectToNetworkIdDictionary.ContainsKey(go)) {
			return;
		}

		int networkId = gameObjectToNetworkIdDictionary[go];
		networkIdToGameObjectDictionary.Remove(networkId);
		gameObjectToNetworkIdDictionary.Remove(go);
	}
}
