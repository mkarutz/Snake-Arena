flatc -o "..\Client\Assets\Scripts\Network\Flatbuffers" -n ClientMessage.fbs ServerMessage.fbs Vector2.fbs TickAck.fbs NetworkMessage.fbs NetworkObjectState.fbs --gen-onefile
@pause