using System;
using UnityEngine;

public abstract class INetworkGameObjectFactory : MonoBehaviour {
	public abstract INetworkGameObject CreateGameObject();
}
