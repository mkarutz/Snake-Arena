using UnityEngine;
using System.Collections;

public class NetworkFoodController : MonoBehaviour, IController {
    
    void Update()
    {

    }

    public Component getControllerComponent()
    {
        return this;
    }
}
