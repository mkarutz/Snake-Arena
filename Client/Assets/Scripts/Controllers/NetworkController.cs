using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using FlatBuffers;
using slyther.flatbuffers;
using System.Collections.Generic;

public class NetworkController : MonoBehaviour {
	public ReplicationManager replicationManager;
	public CameraController cameraController;
	public InputManager inputManager;

    public int playerID;

    public ClientMessageConstructor clientMessageConstructor = new ClientMessageConstructor();

    private IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 3000);
    private UdpClient udpc;
    private Queue<ServerMessage> messageQueue = new Queue<ServerMessage>();


    void Start() 
	{
        InitConnection();
    }


    void Update()
	{
        ReadPacketsToQueue();
        ProcessQueuedMessages();
		SendInputPacket();
    }


	void SendInputPacket()
	{
		Vector3 desiredMove = inputManager.TargetDirection();
		var message = clientMessageConstructor.ConstructClientInputState(ClientMessageType.ClientInputState, (short) playerID, 0, desiredMove.normalized, false);
		udpc.Send(message, message.Length);
	}


    void ReadPacketsToQueue()
    {
        while (udpc.Available > 0)
        {
            byte[] buf = udpc.Receive(ref serverEndPoint);

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
                this.ReplicateState(serverWorldState);
                break;
        }
    }


    private void InitConnection()
    {
        this.udpc = new UdpClient("localhost", 3000);
		SendServerHello();
        ReceiveServerHello();
    }


	private void SendServerHello()
	{
		var message = clientMessageConstructor.ConstructClientHello(ClientMessageType.ClientHello, 0, "foo");
		udpc.Send(message, message.Length);
	}


    public void ReceiveServerHello()
    {
        byte[] buf = udpc.Receive(ref serverEndPoint);
        ByteBuffer byteBuf = new ByteBuffer(buf);
        ServerMessage sm = ServerMessage.GetRootAsServerMessage(byteBuf);
        this.playerID = sm.GetMsg(new ServerHello()).ClientId;
    }
		

    public void ReplicateState(ServerWorldState state)
    {
		replicationManager.ReceiveReplicatedGameObjects(state);
    }
}
