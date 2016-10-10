package in.slyther.network;

import in.slyther.gameobjects.GameObject;

import java.util.HashMap;
import java.util.Map;

public class LinkingContext {
    private Map<Integer, GameObject> networkIdToGameObjectMap = new HashMap<>();
    private Map<GameObject, Integer> gameObjectToNetworkIdMap = new HashMap<>();


    public GameObject getGameObject(int networkId) {
        if (!networkIdToGameObjectMap.containsKey(networkId)) {
            return null;
        }
        return networkIdToGameObjectMap.get(networkId);
    }


    public int getNetworkId(GameObject go, boolean shouldCreateIfNotFound) {
        if (gameObjectToNetworkIdMap.containsKey(go)) {
            return gameObjectToNetworkIdMap.get(go);
        } else if (shouldCreateIfNotFound) {
            addGameObject(go);
            return gameObjectToNetworkIdMap.get(go);
        }
        return -1;
    }


    private int nextNetworkId = 1;

    private void addGameObject(GameObject go) {
        addGameObject(go, nextNetworkId++);
    }


    public void addGameObject(GameObject go, int networkId) {
        networkIdToGameObjectMap.put(networkId, go);
        gameObjectToNetworkIdMap.put(go, networkId);
    }


    public void removeGameObject(GameObject go) {
        if (!gameObjectToNetworkIdMap.containsKey(go)) {
            return;
        }

        int networkId = gameObjectToNetworkIdMap.get(go);
        networkIdToGameObjectMap.remove(networkId);
        gameObjectToNetworkIdMap.remove(go);
    }
}
