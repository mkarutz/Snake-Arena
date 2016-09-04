using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using FlatBuffers;
using slyther.flatbuffers;

public class NetworkController : MonoBehaviour {

    public GameState gameState;

    private UdpClient udpc;
    private FlatBufferBuilder fbBuilder = new FlatBufferBuilder(1);

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
        fbBuilder.Clear();
        StringOffset playerName = fbBuilder.CreateString("foo");
        Offset<ClientHello> helloMsg = ClientHello.CreateClientHello(fbBuilder, playerName);
        Offset<ClientMessage> clientMsg = ClientMessage.CreateClientMessage(fbBuilder, 0, ClientMessageType.ClientHello);
        fbBuilder.Finish(clientMsg.Value);
        //fbBuilder.Finish(helloMsg.Value);
        byte[] message = fbBuilder.SizedByteArray();

        Debug.Log("Message sending");
        Debug.Log(message.Length);
        this.udpc.Send(message, message.Length);
    }

    private void PollConnection()
    {

    }
}
