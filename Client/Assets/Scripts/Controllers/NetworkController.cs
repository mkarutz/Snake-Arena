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

    //private SpatialHashMap<SnakeState> worldSnakeMap;
    //private SpatialHashMap<FoodState> worldFoodMap;

    public int cellSize = 25;
    public int playerID;

    public GameState gameState;
    public ClientMessageConstructor clientMessageConstructor = new ClientMessageConstructor();

    private IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 3000);
    private UdpClient udpc;
    private Queue<ServerMessage> messageQueue = new Queue<ServerMessage>();

    //private FlatBufferBuilder fbBuilder = new FlatBufferBuilder(1);

    // Use this for initialization
    void Start () {
        //this.worldFoodMap = new SpatialHashMap<FoodState>(new Vector2(-(worldRadius), -(worldRadius)), new Vector2(worldRadius, worldRadius), cellSize);
        //this.worldSnakeMap = new SpatialHashMap<SnakeState>(new Vector2(-(worldRadius), -(worldRadius)), new Vector2(worldRadius, worldRadius), cellSize);
        gameState.InitState(maxSnakes, maxFoods, worldRadius);
        InitConnection();
        
    }

    // Update is called once per frame
    void Update () {
        ReadPacketsToQueue();
        ProcessQueuedMessages();
        if (this.gameState.IsSnakeActive(this.playerID))
        {
            Quaternion targetDir = this.gameState.GetSnake(this.playerID).GetComponent<LocalSnakeController>().targetDirection;

            SendInputState(this.gameState.GetSnake(this.playerID).GetComponent<NetworkSnakeController>().GetDesiredMove(Camera.main));
        }
        
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
//        this.udpc = new UdpClient("localhost", 3000);
        this.udpc = new UdpClient("10.12.56.120", 3000);
        var message = clientMessageConstructor.ConstructClientHello(ClientMessageType.ClientHello,0,"foo");

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
            this.playerID = sm.GetMsg(new ServerHello()).ClientId;
        }
    }

    void SendInputState(Vector3 desiredMove)
    {
        Debug.Log(desiredMove);
        var inputStatemessage = clientMessageConstructor.ConstructClientInputState(ClientMessageType.ClientInputState,(byte)this.playerID,30,desiredMove,false);
        this.udpc.Send(inputStatemessage,inputStatemessage.Length);
    }

    public void ReplicateState(ServerWorldState state)
    {
        for (int i = 0; i < state.ObjectStatesLength; i++)
        {
            NetworkObjectState objectState = state.GetObjectStates(i);
            NetworkObjectStateType objectType = objectState.StateType;
            if (objectType == NetworkObjectStateType.NetworkFoodState)
            {
                slyther.flatbuffers.NetworkFoodState foodState = objectState.GetState<NetworkFoodState>(new NetworkFoodState());
                if (!this.gameState.IsFoodActive(foodState.FoodId))
                    //should be NetworkFoodController .. changed to Local for testing
                    this.gameState.ActivateFood<LocalFoodController>(foodState.FoodId, new Vector2(foodState.Position.X, foodState.Position.Y), Color.red, foodState.Weight);
            }
            if (objectType == NetworkObjectStateType.NetworkSnakeState)
            {
                slyther.flatbuffers.NetworkSnakeState snakeState = objectState.GetState<NetworkSnakeState>(new NetworkSnakeState());
                if (!this.gameState.IsSnakeActive(snakeState.PlayerId))
                {
                    this.gameState.ActivateSnake<NetworkSnakeController>(snakeState.PlayerId, snakeState.Name, (int)snakeState.Score, Vector3.zero, 1);
                    if(snakeState.PlayerId == this.playerID)
                    {
                        //Debug.Log(snakeState.PlayerId + " " + this.playerID);
                        SnakeState playerSnake = this.gameState.GetSnake(snakeState.PlayerId);
                        Debug.Log(playerSnake);
                        Camera.main.GetComponent<CameraController>().snakeToTrack = playerSnake; 
                    }
                }

                this.gameState.GetSnake(snakeState.PlayerId).GetComponent<NetworkSnakeController>().ReplicateSnakeState(snakeState);
            }
        }
    }

    private void PollConnection()
    {

    }
}