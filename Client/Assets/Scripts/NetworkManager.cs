using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using FlatBuffers;
using slyther.flatbuffers;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {
	public ReplicationManager replicationManager;

	public ClientMessageConstructor clientMessageConstructor = new ClientMessageConstructor();

	private Queue<ServerMessage> messageQueue = new Queue<ServerMessage>();

	void Start() {
		InitConnection();
	}


	void Update() {
		ReadPacketsToQueue();
		ProcessQueuedMessages();
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


	/**
	 * Processes all the messages in the queue.
	 */
	void ProcessQueuedMessages()
	{
		while (messageQueue.Count > 0)
		{
			ServerMessage msg = messageQueue.Dequeue();
			ProcessMessage(msg);
		}
	}


	/**
	 * Handle received message.
	 */
	void ProcessMessage(ServerMessage msg)
	{
		ServerMessageType msgType = msg.MsgType;
		switch (msgType)
		{
		case ServerMessageType.ServerWorldState:
			ServerWorldState serverWorldState = msg.GetMsg(new ServerWorldState ());
			replicationManager.replicateState(serverWorldState);
			break;
		}
	}



	private IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 3000);
	private UdpClient udpc;

	/**
	 * Establishes connection to the server.
	 */
	private void InitConnection()
	{
		this.udpc = new UdpClient("localhost", 3000);
		var message = clientMessageConstructor.ConstructClientHello(ClientMessageType.ClientHello,0,"foo");

		this.udpc.Send(message, message.Length);

		ReceiveMsg();
	}


	/**
	 * Receives the next message from the socket if available.
	 */
	public void ReceiveMsg()
	{
		if (udpc.Available > 0)
		{
			byte[] buf = udpc.Receive(ref serverEndPoint);
			ByteBuffer byteBuf = new ByteBuffer(buf);
			ServerMessage sm = ServerMessage.GetRootAsServerMessage(byteBuf);
		}
	}
}
