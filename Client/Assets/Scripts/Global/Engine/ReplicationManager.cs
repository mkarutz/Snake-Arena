using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using slyther.flatbuffers;

public class ReplicationManager : MonoBehaviour {
	public LinkingContext linkingContext;
	public ObjectCreationRegistry objectCreationRegistry;

	private HashSet<INetworkGameObject> previouslyReceivedGameObjects = new HashSet<INetworkGameObject>();
	private HashSet<INetworkGameObject> receivedGameObjects = new HashSet<INetworkGameObject>();


	public void ReceiveReplicatedGameObjects(ServerWorldState state)
	{
		receivedGameObjects.Clear();

		// Update objects
		for (int i = 0; i < state.ObjectStatesLength; i++) {
			NetworkObjectState objectState = state.GetObjectStates(i);
			INetworkGameObject go = ReceiveReplicatedGameObject(objectState);
			receivedGameObjects.Add(go);
		}

		// Destroy objects we didn't receive in this update
		foreach (INetworkGameObject go in previouslyReceivedGameObjects) {
			if (!receivedGameObjects.Contains(go)) {
				linkingContext.RemoveGameObject(go);
				go.Destroy();
			}
		}

		// Swap the previous and current sets
		HashSet<INetworkGameObject> temp = previouslyReceivedGameObjects;
		previouslyReceivedGameObjects = receivedGameObjects;
		receivedGameObjects = temp;
	}


	private INetworkGameObject ReceiveReplicatedGameObject(NetworkObjectState state)
	{
		int networkId = state.NetworkId;
		byte classId = (byte) state.StateType;

		INetworkGameObject go = linkingContext.GetGameObject(networkId);

		if (go == null) {
			go = objectCreationRegistry.CreateGameObject(classId);
			linkingContext.AddGameObject(go, networkId);
		}
        
        go.Replicate(state);

		return go;
	}
}
