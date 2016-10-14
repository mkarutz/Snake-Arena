using UnityEngine;
using System.Collections;

public class GameConfig : MonoBehaviour {
	public static int MAX_LOCAL_SNAKES = 10;
	public static int MAX_LOCAL_FOODS = 1000;
    public static float WORLD_RADIUS_REMOTE = 50.0f;
    public static float WORLD_RADIUS_LOCAL = 25.0f;
    public static int MAX_SCOREBOARD_PLAYERS = 10;
	public static int NUM_SNAKE_SKINS = 3;
    public static float SNAKE_FOG_MULTIPLIER = 15.0f;
    public static float EAT_DISTANCE_RATIO = 1.0f;
}
