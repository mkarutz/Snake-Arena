using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using FlatBuffers;
using slyther.flatbuffers;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class NetworkController : MonoBehaviour {
	private const int UPDATE_RATE = 30;
	private const float TIME_OUT = 5.0f;

	public LinkingContext linkingContext;
	public ReplicationManager replicationManager;
	public InputManager inputManager;
    public GameWorld gameWorld;
	public GameObject lostConnectionDialogue;
	public GameObject connectionFailedDialogue;

    public int playerID = -1;
	bool isConnected = false;

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
		if (!IsConnected()) {
			return;
		}

		CheckTimeout();

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


	/// <summary>
	/// The time since last sent input packet.
	/// </summary>
	float timeSinceLastSentInputPacket = 0.0f;

	void MaybeSendInputPacket()
	{
		timeSinceLastSentInputPacket += Time.deltaTime;
		if (timeSinceLastSentInputPacket > 1.0f / UPDATE_RATE) {
			SendInputPacket();
			timeSinceLastSentInputPacket = 0.0f;
		}
	}


	void SendInputPacket()
	{
		Vector3 desiredMove = inputManager.TargetDirection();
		bool isTurbo = inputManager.IsTurbo();

        AtrociousRotateFunctionToKeepToDeadline(); // todo: get rid of it

        var message = clientMessageConstructor.ConstructClientInputState(ClientMessageType.ClientInputState, (ushort) playerID, 0, desiredMove.normalized, isTurbo);
		udpc.Send(message, message.Length);
	}

    void AtrociousRotateFunctionToKeepToDeadline()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            SnakeState playerSnake = player.GetComponent<SnakeState>();
            playerSnake.TurnTowards(inputManager.TargetDirection(), Time.deltaTime);
        }
    }


	/// <summary>
	/// The time since last received packet.
	/// </summary>
	private float timeSinceLastReceivedPacket = 0.0f;

	void CheckTimeout()
	{
		timeSinceLastReceivedPacket += Time.deltaTime;
		if (timeSinceLastReceivedPacket > TIME_OUT) {
			Disconnect();
		}
	}


	void Connect()
	{
		isConnected = true;
	}


	public void Disconnect()
	{
		isConnected = false;
		lostConnectionDialogue.SetActive(true);
	}


	public bool IsConnected()
	{
		return isConnected;
	}


    void ReadPacketsToQueue()
    {
		try {
			while (udpc.Available > 0)
			{
				byte[] buf = udpc.Receive(ref serverEndPoint);

				ByteBuffer byteBuf = new ByteBuffer(buf);
				ServerMessage sm = ServerMessage.GetRootAsServerMessage(byteBuf);
				messageQueue.Enqueue(sm);

				timeSinceLastReceivedPacket = 0.0f;
			}
		} catch (Exception e) {
			lostConnectionDialogue.SetActive(true);
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
		try {
			this.udpc = new UdpClient(GameConfig.REMOTE_HOST_NAME, GameConfig.REMOTE_HOST_PORT);
			SendServerHello();
			ReceiveServerHello();
			Connect();
		} catch (Exception e) {
			connectionFailedDialogue.SetActive(true);
		}
    }


	private void SendServerHello()
	{
		var message = clientMessageConstructor.ConstructClientHello(ClientMessageType.ClientHello, (ushort)PlayerProfile.Instance().Skin, PlayerProfile.Instance().Nickname);
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
