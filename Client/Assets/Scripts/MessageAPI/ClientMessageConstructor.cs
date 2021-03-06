﻿using slyther.flatbuffers;
using FlatBuffers;
using System.Net.Sockets;
using UnityEngine;
using System.Collections;

public class ClientMessageConstructor {

    private FlatBufferBuilder fbBuilder = new FlatBufferBuilder(1);

    public byte[] ConstructClientHello(ClientMessageType type, ushort clientId, string playerName)
    {
        fbBuilder.Clear();

        StringOffset playerNameOffset = fbBuilder.CreateString(playerName);

        ClientHello.StartClientHello(fbBuilder);
        ClientHello.AddPlayerName(fbBuilder, playerNameOffset);
        Offset<ClientHello> clientHelloOffset = ClientHello.EndClientHello(fbBuilder);

        ClientMessage.StartClientMessage(fbBuilder);
        //var clientIdOffset = clientId;
        ClientMessage.AddClientId(fbBuilder,clientId);
        ClientMessage.AddMsgType(fbBuilder, type);
        ClientMessage.AddMsg(fbBuilder, clientHelloOffset.Value);
        Offset<ClientMessage> clientMessageOffset = ClientMessage.EndClientMessage(fbBuilder);

        ClientMessage.FinishClientMessageBuffer(fbBuilder, clientMessageOffset);

        return fbBuilder.SizedByteArray();
         
    }

    public byte[] ConstructClientInputState(ClientMessageType type, ushort clientId, uint tick, Vector3 desiredMove, bool isTurbo)
    {
        fbBuilder.Clear();


        ClientInputState.StartClientInputState(fbBuilder);
        ClientInputState.AddTick(fbBuilder, tick);
        ClientInputState.AddDesiredMove(fbBuilder, Vec2.CreateVec2(fbBuilder, desiredMove.x, desiredMove.y));
        ClientInputState.AddIsTurbo(fbBuilder, isTurbo);
        Offset<ClientInputState> clientInputStateOffset = ClientInputState.EndClientInputState(fbBuilder);

        ClientMessage.StartClientMessage(fbBuilder);
        //var clientIdOffset = clientId;
		ClientMessage.AddClientId(fbBuilder, clientId);
        ClientMessage.AddMsgType(fbBuilder, type);
        ClientMessage.AddMsg(fbBuilder, clientInputStateOffset.Value);
        Offset<ClientMessage> clientMessageOffset = ClientMessage.EndClientMessage(fbBuilder);

        ClientMessage.FinishClientMessageBuffer(fbBuilder, clientMessageOffset);

        return fbBuilder.SizedByteArray();
    }

    public byte[] ConstructClientGoodbye(ClientMessageType type, ushort clientId)
    {
        fbBuilder.Clear();

        ClientGoodbye.StartClientGoodbye(fbBuilder);
        Offset<ClientGoodbye> clientGoodbyeOffset = ClientGoodbye.EndClientGoodbye(fbBuilder);

        ClientMessage.StartClientMessage(fbBuilder);
        //var clientIdOffset = clientId;
        ClientMessage.AddClientId(fbBuilder, clientId);
        ClientMessage.AddMsgType(fbBuilder, type);
        ClientMessage.AddMsg(fbBuilder, clientGoodbyeOffset.Value);
        Offset<ClientMessage> clientMessageOffset = ClientMessage.EndClientMessage(fbBuilder);

        ClientMessage.FinishClientMessageBuffer(fbBuilder, clientMessageOffset);

        return fbBuilder.SizedByteArray();
    }

    public byte[] ConstructTickAck(ClientMessageType type, ushort clientId, uint tick)
    {
        fbBuilder.Clear();

        TickAck.StartTickAck(fbBuilder);
        TickAck.AddTick(fbBuilder, tick);
        Offset<TickAck> tickAckOffset = TickAck.EndTickAck(fbBuilder);

        ClientMessage.StartClientMessage(fbBuilder);
        //var clientIdOffset = clientId;
        ClientMessage.AddClientId(fbBuilder, clientId);
        ClientMessage.AddMsgType(fbBuilder, type);
        ClientMessage.AddMsg(fbBuilder, tickAckOffset.Value);
        Offset<ClientMessage> clientMessageOffset = ClientMessage.EndClientMessage(fbBuilder);

        ClientMessage.FinishClientMessageBuffer(fbBuilder, clientMessageOffset);

        return fbBuilder.SizedByteArray();
    }
}
