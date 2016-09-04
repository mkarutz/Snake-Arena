using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using FlatBuffers;
using slyther.flatbuffers;

public class NetworkController : MonoBehaviour {

    public GameState gameState;

    private UdpClient udpc;
    
	// Use this for initialization
	void Start () {
        InitConnection();
	}
	
	// Update is called once per frame
	void Update () {
        PollConnection();
	}

    private void InitConnection()
    {
        this.udpc = new UdpClient("10.12.55.210", 3000);
        slyther.flatbuffers.Vector2 fb = new slyther.flatbuffers.Vector2();
        
        //this.udpc.
    }

    private void PollConnection()
    {

    }
}
