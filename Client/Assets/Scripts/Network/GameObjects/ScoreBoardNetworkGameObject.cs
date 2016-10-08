using UnityEngine;
using System.Collections;
using slyther.flatbuffers;

public class ScoreBoardNetworkGameObject : INetworkGameObject {
	
	public override void Replicate(NetworkObjectState state)
	{
		
	}


	public override void Destroy()
	{
		GameObject.Destroy(gameObject);
	}
}
