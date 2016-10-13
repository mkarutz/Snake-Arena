using UnityEngine;
using System.Collections;
using slyther.flatbuffers;

public abstract class INetworkGameObject : MonoBehaviour {
	public abstract void Replicate(NetworkObjectState state);
	public abstract void Destroy();
}
