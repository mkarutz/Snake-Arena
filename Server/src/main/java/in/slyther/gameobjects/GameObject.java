package in.slyther.gameobjects;

import com.google.flatbuffers.FlatBufferBuilder;

public interface GameObject {
    int serialize(FlatBufferBuilder builder);
    byte classId();
}
