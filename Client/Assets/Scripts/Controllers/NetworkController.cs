using UnityEngine;
using System.Collections;
using System.Net;
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
        this.udpc = new UdpClient("10.12.32.220", 3000);
        this.udpc.Client.ReceiveTimeout = 5000;
        fbBuilder.Clear();

        StringOffset playerNameOffset = fbBuilder.CreateString("foo");
        Offset<ClientHello> helloMessageOffset = ClientHello.CreateClientHello(fbBuilder, playerNameOffset);

        ClientMessage.StartClientMessage(fbBuilder);
        ClientMessage.AddMsgType(fbBuilder, ClientMessageType.ClientHello);
        ClientMessage.AddMsg(fbBuilder, helloMessageOffset.Value);
        var clientMessageOffset = ClientMessage.EndClientMessage(fbBuilder);

        ClientMessage.FinishClientMessageBuffer(fbBuilder, clientMessageOffset);
        
        byte[] message = fbBuilder.SizedByteArray();

        Debug.Log("Message sending");
        Debug.Log(message.Length);
        this.udpc.Send(message, message.Length);

        ReceiveHello();
    }

    private void ReceiveHello()
    {
        var ep = new IPEndPoint(IPAddress.Any, 3000);
        byte[] buf = this.udpc.Receive(ref ep);
        var byteBuffer = new ByteBuffer(buf);

        var msg = ServerMessage.GetRootAsServerMessage(byteBuffer);

        var type = msg.MsgType;

        Debug.Log("Received");
        Debug.Log(type);

        if (ServerMessageType.ServerHello.Equals(type))
        {
            var helloMsg = msg.GetMsg<ServerHello>(new ServerHello());
            Debug.Log("Received Hello from server. Client ID = " + helloMsg.ClientId);
        }
    }

    private void PollConnection()
    {


    }
}