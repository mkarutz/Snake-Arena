using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
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
        this.udpc = new UdpClient("10.12.38.43", 3000);
        var message = clientMessageConstructor.ConstructClientHello(ClientMessageType.ClientHello,0,"foo");

        Debug.Log("Message sending");
        Debug.Log(message.Length);
        this.udpc.Send(message, message.Length);
        
        ReceiveMsg();
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

    
    private void PollConnection()
    {
        
    }
}