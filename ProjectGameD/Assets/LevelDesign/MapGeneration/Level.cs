using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Level
{
    public static float height = 500;
    public static float width = 500;

    public static float scale = 1f;
    public static float iconScale = .07f;
    public static float padding = .01f;

    public static float roomGenerationChance = .5f;

    public static int RoomLimit = 5;

    public static Sprite treasureRoomIcon;
    public static Sprite bossRoomIcon;
    public static Sprite shopRoomIcon;
    public static Sprite unexploreRoom;
    public static Sprite defaultRoomIcon;
    public static Sprite currentRoomIcon;
    public static Sprite secretRoom;

    public static List<Room> rooms = new List<Room>();
    public static Room currentRoom;

}

public class Room
{
    public int roomNumber = 6;
    public Vector2 location;
    public Sprite roomImage;
}
