using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using FlatBuffers;
using slyther.flatbuffers;
using System.Collections.Generic;

public class NetworkController : MonoBehaviour {
	private const int UPDATE_RATE = 30;

	public LinkingContext linkingContext;
	public ReplicationManager replicationManager;
	public InputManager inputManager;
    public GameWorld gameWorld;

    public int playerID = -1;

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
		MaybeSendInputPacket();

        // Needs to be a better place to put this...
        TagLocalPlayer();
    }


    void TagLocalPlayer()
    {
        GameObject player = this.GetLocalPlayer().gameObject;
        if (player)
            player.tag = "Player";
    }


	float inputPacketCooldownTimer = 0.0f;

	void MaybeSendInputPacket()
	{
		inputPacketCooldownTimer -= Time.deltaTime;
		if (inputPacketCooldownTimer < 0.0f) {
			SendInputPacket();
			inputPacketCooldownTimer = 1.0f / UPDATE_RATE;
		}
	}


	void SendInputPacket()
	{
		Vector3 desiredMove = inputManager.TargetDirection();
		bool isTurbo = inputManager.IsTurbo();

		var message = clientMessageConstructor.ConstructClientInputState(ClientMessageType.ClientInputState, (ushort) playerID, 0, desiredMove.normalized, isTurbo);
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
		this.udpc = new UdpClient("10.12.55.234", 3000);
		SendServerHello();
        ReceiveServerHello();
    }


	private void SendServerHello()
	{
		var message = clientMessageConstructor.ConstructClientHello(ClientMessageType.ClientHello, 0, PlayerProfile.Instance().Nickname);
		udpc.Send(message, message.Length);
	}


    public void ReceiveServerHello()
    {
        byte[] buf = udpc.Receive(ref serverEndPoint);
        ByteBuffer byteBuf = new ByteBuffer(buf);
        ServerMessage sm = ServerMessage.GetRootAsServerMessage(byteBuf);
        this.playerID = sm.GetMsg(new ServerHello()).ClientId;
		Debug.Log("Received player ID: " + playerID);
        this.gameWorld.worldRadius = GameConfig.WORLD_RADIUS_REMOTE; // Replace with radius sent from server eventually
    }
		

    public void ReplicateState(ServerWorldState state)
    {
		replicationManager.ReceiveReplicatedGameObjects(state);
    }


	public INetworkGameObject GetLocalPlayer()
	{
		return playerID == -1 ? null : linkingContext.GetGameObject(playerID);
	}
}
