flatc -o "..\Client\Assets\Scripts\Network\Flatbuffers" -n ClientMessage.fbs ServerMessage.fbs Vec2.fbs TickAck.fbs NetworkMessage.fbs NetworkObjectState.fbs --gen-onefile
@pause