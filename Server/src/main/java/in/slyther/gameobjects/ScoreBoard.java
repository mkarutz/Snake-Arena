package in.slyther.gameobjects;

import com.google.flatbuffers.FlatBufferBuilder;
import slyther.flatbuffers.NetworkScoreBoardState;
import slyther.flatbuffers.ScoreboardEntry;

import java.util.*;

public class ScoreBoard {
    private static final int DISPLAY_TOP_N = 10;

    private final SortedSet<ScoreBoardEntry> entries = new TreeSet<>();
    private final Map<Snake, ScoreBoardEntry> playerEntryMap = new HashMap<>();


    public void updateScore(Snake snake) {
        ScoreBoardEntry entry = playerEntryMap.get(snake);
        if (entry == null) {
            entry = new ScoreBoardEntry(snake.getName(), snake.getScore());
            playerEntryMap.put(snake, entry);
        }

        entries.remove(entry);
        entry.name = snake.getName();
        entry.setScore(snake.getScore());
        entries.add(entry);

        System.out.println("Updated score for player " + entry.name);
    }


    public void remove(Snake snake) {
        ScoreBoardEntry entry = playerEntryMap.get(snake);
        if (entry == null) {
            return;
        }

        entries.remove(entry);
        playerEntryMap.remove(snake);
    }


    public int serialize(FlatBufferBuilder builder) {
        int[] entryOffsets = new int[Math.min(DISPLAY_TOP_N, entries.size())];
        int n = 0;

        for (ScoreBoardEntry entry : entries) {
            entryOffsets[n++] = entry.serialize(builder);

            if (n == DISPLAY_TOP_N) {
                break;
            }
        }

        int vectorOffset = NetworkScoreBoardState.createEntriesVector(builder, entryOffsets);

        NetworkScoreBoardState.startNetworkScoreBoardState(builder);
        NetworkScoreBoardState.addEntries(builder, vectorOffset);

        return NetworkScoreBoardState.endNetworkScoreBoardState(builder);
    }



    private class ScoreBoardEntry implements Comparable<ScoreBoardEntry> {
        private String name;
        private int score;

        public ScoreBoardEntry(String name, int score) {
            this.name = name;
            this.score = score;
        }

        @Override
        public int compareTo(ScoreBoardEntry o) {
            return o.score - score;
        }


        public int serialize(FlatBufferBuilder builder) {
            System.out.println("name = " + name);
            int playerNameOffset = builder.createString(name);

            ScoreboardEntry.startScoreboardEntry(builder);
            ScoreboardEntry.addPlayerName(builder, playerNameOffset);
            ScoreboardEntry.addScore(builder, score);

            return ScoreboardEntry.endScoreboardEntry(builder);
        }


        public String getName() {
            return name;
        }

        public void setName(String name) {
            this.name = name;
        }

        public int getScore() {
            return score;
        }

        public void setScore(int score) {
            this.score = score;
        }
    }
}
