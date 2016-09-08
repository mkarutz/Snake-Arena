using slyther.flatbuffers;
using FlatBuffers;
using System.Net.Sockets;
using UnityEngine;
using System.Collections;

public class NetworkTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		UdpClient udpc = new UdpClient ("localhost", 3000);

		FlatBufferBuilder builder = new FlatBufferBuilder (1);

		StringOffset offsetName = builder.CreateString ("foobar");

		ClientHello.StartClientHello (builder);
		ClientHello.AddPlayerName (builder, offsetName);
		Offset<ClientHello> offsetClientHello = ClientHello.EndClientHello (builder);

		ClientMessage.StartClientMessage (builder);
		ClientMessage.AddMsgType (builder, ClientMessageType.ClientHello);
		ClientMessage.AddMsg (builder, offsetClientHello.Value);
		Offset<ClientMessage> offsetClientMessage = ClientMessage.EndClientMessage (builder);

		ClientMessage.FinishClientMessageBuffer (builder, offsetClientMessage);

		var bytes = builder.SizedByteArray();
		//Debug.Log ("Length of the serialized buffer: " + bytes.Length);

		udpc.Send(bytes, bytes.Length);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
