using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Net;
using FlatBuffers;
using slyther.flatbuffers;
using System.Collections.Generic;

public class NetworkController : MonoBehaviour {
    public int maxSnakes = 100;
    public int maxFoods = 10000;
    public int worldRadius = 500;

    public GameState gameState;
    public ClientMessageConstructor clientMessageConstructor = new ClientMessageConstructor();

    private IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 3000);
    private UdpClient udpc;
    private Queue<ServerMessage> messageQueue = new Queue<ServerMessage>();

    //private FlatBufferBuilder fbBuilder = new FlatBufferBuilder(1);

    // Use this for initialization
    void Start () {
        gameState.InitState(maxSnakes, maxFoods, worldRadius);
        InitConnection();
	}
	
	// Update is called once per frame
	void Update () {
        ReadPacketsToQueue();
        ProcessQueuedMessages();
	}


    void ReadPacketsToQueue()
    {
        while (udpc.Available > 0)
        {
            byte[] buf = udpc.Receive(ref serverEndPoint);
            Debug.Log("Received " + buf.Length + " bytes.");

            ByteBuffer byteBuf = new ByteBuffer(buf);
            ServerMessage sm = ServerMessage.GetRootAsServerMessage(byteBuf);
            messageQueue.Enqueue(sm);
        }
    }


    void ProcessQueuedMessages()
    {
        while (messageQueue.Count > 0)
        {
            ServerMessage msg = messageQueue.Dequeue();
            ProcessMessage(msg);
        }
    }

    void ProcessMessage(ServerMessage msg)
    {
        ServerMessageType msgType = msg.MsgType;
        switch (msgType)
        {
            case ServerMessageType.ServerWorldState:
                ServerWorldState serverWorldState = msg.GetMsg(new ServerWorldState());
                gameState.ReplicateState(serverWorldState);
                break;
        }
    }

    private void InitConnection()
    {
<<<<<<< HEAD
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
=======
        this.udpc = new UdpClient("10.12.38.43", 3000);
        var message = clientMessageConstructor.ConstructClientHello(ClientMessageType.ClientHello,0,"foo");
>>>>>>> 9c447b7547d49a85b570acb81e1d7f3a98f44ecc

        Debug.Log("Message sending");
        Debug.Log(message.Length);
        this.udpc.Send(message, message.Length);

<<<<<<< HEAD
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
=======
        ReceiveMsg();
>>>>>>> 9c447b7547d49a85b570acb81e1d7f3a98f44ecc
    }

    public void ReceiveMsg()
    {
        if (udpc.Available > 0)
        {
            byte[] buf = udpc.Receive(ref serverEndPoint);
            ByteBuffer byteBuf = new ByteBuffer(buf);
            ServerMessage sm = ServerMessage.GetRootAsServerMessage(byteBuf);
            Debug.Log(sm.MsgType);
        }
    }


<<<<<<< HEAD

=======
    private void PollConnection()
    {
        
>>>>>>> 9c447b7547d49a85b570acb81e1d7f3a98f44ecc
    }
}