using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using slyther.flatbuffers;

public class ObjectCreationRegistry : MonoBehaviour {
	private Dictionary<byte, INetworkGameObjectFactory> classIdToFactoryDictionary = new Dictionary<byte, INetworkGameObjectFactory>();

	public FoodFactory foodFactory;
	public SnakeFactory snakeFactory;
	public ScoreBoardFactory scoreBoardFactory;


	void Awake()
	{
		RegisterFactory((byte) NetworkObjectStateType.NetworkFoodState, foodFactory);
		RegisterFactory((byte) NetworkObjectStateType.NetworkSnakeState, snakeFactory);
		RegisterFactory((byte)NetworkObjectStateType.NetworkScoreBoardState, scoreBoardFactory);
	}


	public INetworkGameObject CreateGameObject(byte classId)
	{
		if (!classIdToFactoryDictionary.ContainsKey(classId)) {
			return null;
		}
		return classIdToFactoryDictionary[classId].CreateGameObject();
	}


	public void RegisterFactory(byte classId, INetworkGameObjectFactory factory)
	{
		classIdToFactoryDictionary[classId] = factory;
	}
}
