using System;
using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine;
using UnityEngine.UI;

public class GenerateLevel : MonoBehaviour
{
    public Sprite currentRoom;
    public Sprite bossRoom;
    public Sprite emptyRoom;
    public Sprite shopRoom;
    public Sprite treasureRoom;
    public Sprite unexploreRoom;
    public Sprite secretRoom;

    public bool regenerating = false;

    private void Awake()
    {
        Level.defaultRoomIcon = emptyRoom;
        Level.bossRoomIcon = bossRoom;
        Level.currentRoomIcon = currentRoom;
        Level.shopRoomIcon = shopRoom;
        Level.treasureRoomIcon = treasureRoom;
        Level.unexploreRoom = unexploreRoom;
        Level.secretRoom = secretRoom;
    }

    void Start()
    {
        regenerating = true;
        Room startRoom = new Room();
        startRoom.location = new Vector2(0, 0);
        startRoom.roomImage = Level.currentRoomIcon;
        startRoom.roomNumber = 0;

        PlayerManager.currentRoom = startRoom;

        //Draw Start Room
        DrawRoomOnMap(startRoom);

        //Left
        if (UnityEngine.Random.value > Level.roomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.location = new Vector2(-1, 0);
            newRoom.roomImage = Level.defaultRoomIcon;
            newRoom.roomNumber = RandomRoomNumber();

            if (!CheckIfRoomExists(newRoom.location))
            {
                if (!CheckIfRoomAroundGeneratedRoom(newRoom.location, "Right"))
                {
                    Generate(newRoom);
                }
            }
        }
        //Right
        if (UnityEngine.Random.value > Level.roomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.location = new Vector2(1, 0);
            newRoom.roomImage = Level.defaultRoomIcon;
            newRoom.roomNumber = RandomRoomNumber();

            if (!CheckIfRoomExists(newRoom.location))
            {
                if (!CheckIfRoomAroundGeneratedRoom(newRoom.location, "Left"))
                {
                    Generate(newRoom);
                }
            }
        }
        //Up
        if (UnityEngine.Random.value > Level.roomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.location = new Vector2(0, 1);
            newRoom.roomImage = Level.defaultRoomIcon;
            newRoom.roomNumber = RandomRoomNumber();

            if (!CheckIfRoomExists(newRoom.location))
            {
                if (!CheckIfRoomAroundGeneratedRoom(newRoom.location, "Down"))
                {
                    Generate(newRoom);
                }
            }
        }
        //Down
        if (UnityEngine.Random.value > Level.roomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.location = new Vector2(0, -1);
            newRoom.roomImage = Level.defaultRoomIcon;
            newRoom.roomNumber = RandomRoomNumber();

            if (!CheckIfRoomExists(newRoom.location))
            {
                if (!CheckIfRoomAroundGeneratedRoom(newRoom.location, "Up"))
                {
                    Generate(newRoom);
                }
            }
        }

        ShuffleList(Level.rooms);

        bool boss = GenerateBossRoom();
        bool shop = GenerateSpecialRoom(Level.shopRoomIcon, 2);
        bool treasure = GenerateSpecialRoom(Level.treasureRoomIcon, 3);
        bool secret = GenerateSpecialRoom(Level.unexploreRoom, 4);

        if (!treasure && !shop && !secret)
        {
            regenerating = false;
        }

        if (boss == false)
        {
            regenerating = false;
        }
    }

    void Update()
    {
        if (!regenerating)
        {
            regenerating = true;
            failSafe = 0;
            Level.rooms.Clear();
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                Destroy(child.gameObject);
            }

            Start();
        }
    }

    void DrawRoomOnMap(Room room)
    {
        string tileName = "MapTile";
        if (room.roomNumber == 1) tileName = "BossRoomTile";
        if (room.roomNumber == 2) tileName = "ShopRoomTile";
        if (room.roomNumber == 3) tileName = "ItemRoomTile";
        if (room.roomNumber == 4) tileName = "SecretRoomTile";

        GameObject mapTile = new GameObject(tileName);
        Image roomImage = mapTile.AddComponent<Image>();
        roomImage.sprite = room.roomImage;
        RectTransform rectTransform = roomImage.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(Level.height, Level.width) * Level.iconScale;
        rectTransform.position = room.location * (Level.iconScale * Level.height * Level.scale + (Level.padding * Level.height * Level.scale));
        roomImage.transform.SetParent(transform, false);

        Level.rooms.Add(room);
    }

    bool CheckIfRoomExists(Vector2 vector2)
    {
        return Level.rooms.Exists(x => x.location == vector2);
    }

    bool CheckIfRoomAroundGeneratedRoom(Vector2 vector2, string direction)
    {
        switch (direction)
        {
            case "Right":
                {   //Check Down, left, up
                    if (Level.rooms.Exists(x => x.location == new Vector2(vector2.x - 1, vector2.y)) ||
                        Level.rooms.Exists(x => x.location == new Vector2(vector2.x, vector2.y - 1)) ||
                        Level.rooms.Exists(x => x.location == new Vector2(vector2.x, vector2.y + 1)))
                        return true;
                    break;
                }
            case "Left":
                {   //Check Down, Right, up
                    if (Level.rooms.Exists(x => x.location == new Vector2(vector2.x, vector2.y - 1)) ||
                        Level.rooms.Exists(x => x.location == new Vector2(vector2.x + 1, vector2.y)) ||
                        Level.rooms.Exists(x => x.location == new Vector2(vector2.x, vector2.y + 1)))
                        return true;
                    break;
                }
            case "Up":
                {   //Check Down, Right, Left
                    if (Level.rooms.Exists(x => x.location == new Vector2(vector2.x, vector2.y - 1)) ||
                        Level.rooms.Exists(x => x.location == new Vector2(vector2.x + 1, vector2.y)) ||
                        Level.rooms.Exists(x => x.location == new Vector2(vector2.x - 1, vector2.y)))
                        return true;
                    break;
                }
            case "Down":
                {   //Check Up, left, Right
                    if (Level.rooms.Exists(x => x.location == new Vector2(vector2.x, vector2.y + 1)) ||
                        Level.rooms.Exists(x => x.location == new Vector2(vector2.x - 1, vector2.y)) ||
                        Level.rooms.Exists(x => x.location == new Vector2(vector2.x + 1, vector2.y)))
                        return true;
                    break;
                }
        }

        return false;
    }

    int failSafe = 0;

    void Generate(Room room)
    {

        failSafe++;
        if (failSafe > 50)
        {
            return;
        }

        DrawRoomOnMap(room);

        //Left
        if (UnityEngine.Random.value > Level.roomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.location = new Vector2(-1, 0) + room.location;
            newRoom.roomImage = Level.defaultRoomIcon;
            newRoom.roomNumber = RandomRoomNumber();

            if (!CheckIfRoomExists(newRoom.location))
            {
                if (!CheckIfRoomAroundGeneratedRoom(newRoom.location, "Right"))
                {
                    if (Math.Abs(newRoom.location.x) < Level.RoomLimit && Math.Abs(newRoom.location.y) < Level.RoomLimit)
                        Generate(newRoom);
                }
            }
        }

        //Right
        if (UnityEngine.Random.value > Level.roomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.location = new Vector2(1, 0) + room.location;
            newRoom.roomImage = Level.defaultRoomIcon;
            newRoom.roomNumber = RandomRoomNumber();

            if (!CheckIfRoomExists(newRoom.location))
            {
                if (!CheckIfRoomAroundGeneratedRoom(newRoom.location, "Left"))
                {
                    if (Math.Abs(newRoom.location.x) < Level.RoomLimit && Math.Abs(newRoom.location.y) < Level.RoomLimit)
                        Generate(newRoom);
                }
            }
        }


        //Up
        if (UnityEngine.Random.value > Level.roomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.location = new Vector2(0, 1) + room.location;
            newRoom.roomImage = Level.defaultRoomIcon;
            newRoom.roomNumber = RandomRoomNumber();

            if (!CheckIfRoomExists(newRoom.location))
            {
                if (!CheckIfRoomAroundGeneratedRoom(newRoom.location, "Down"))
                {
                    if (Math.Abs(newRoom.location.x) < Level.RoomLimit && Math.Abs(newRoom.location.y) < Level.RoomLimit)
                        Generate(newRoom);
                }
            }
        }

        //Down
        if (UnityEngine.Random.value > Level.roomGenerationChance)
        {
            Room newRoom = new Room();
            newRoom.location = new Vector2(0, -1) + room.location;
            newRoom.roomImage = Level.defaultRoomIcon;
            newRoom.roomNumber = RandomRoomNumber();

            if (!CheckIfRoomExists(newRoom.location))
            {
                if (!CheckIfRoomAroundGeneratedRoom(newRoom.location, "Up"))
                {
                    if (Math.Abs(newRoom.location.x) < Level.RoomLimit && Math.Abs(newRoom.location.y) < Level.RoomLimit)
                        Generate(newRoom);
                }
            }
        }
    }

    bool GenerateBossRoom()
    {
        bool foundAvailableLocation = false;

        float maxNumber = 0;
        Vector2 farthestRoom = Vector2.zero;

        foreach (Room R in Level.rooms)
        {
            if (Mathf.Abs(R.location.x) + Mathf.Abs(R.location.y) >= maxNumber)
            {
                maxNumber = Mathf.Abs(R.location.x) + Mathf.Abs(R.location.y);
                farthestRoom = R.location;
            }
        }

        Room bossRoom = new Room();
        bossRoom.roomImage = Level.bossRoomIcon;
        bossRoom.roomNumber = 1;


        //Left
        if (!CheckIfRoomExists(farthestRoom + new Vector2(-1, 0)))
        {
            if (!CheckIfRoomAroundGeneratedRoom(farthestRoom + new Vector2(-1, 0), "Right"))
            {
                bossRoom.location = farthestRoom + new Vector2(-1, 0);
                foundAvailableLocation = true;
            }
        }

        //Right
        else if (!CheckIfRoomExists(farthestRoom + new Vector2(1, 0)))
        {
            if (!CheckIfRoomAroundGeneratedRoom(farthestRoom + new Vector2(1, 0), "Left"))
            {
                bossRoom.location = farthestRoom + new Vector2(1, 0);
                foundAvailableLocation = true;
            }
        }

        //Up
        else if (!CheckIfRoomExists(farthestRoom + new Vector2(0, 1)))
        {
            if (!CheckIfRoomAroundGeneratedRoom(farthestRoom + new Vector2(0, 1), "Down"))
            {
                bossRoom.location = farthestRoom + new Vector2(0, 1);
                foundAvailableLocation = true;
            }
        }


        //Down
        else if (!CheckIfRoomExists(farthestRoom + new Vector2(0, -1)))
        {
            if (!CheckIfRoomAroundGeneratedRoom(farthestRoom + new Vector2(0, -1), "Up"))
            {
                bossRoom.location = farthestRoom + new Vector2(0, -1);
                foundAvailableLocation = true;
            }
        }


        //Debug.Log(farthestRoom);

        if (bossRoom.location == Vector2.zero)
        {
            //bossRoom.location = farthestRoom + new Vector2(0, -1);
            foundAvailableLocation = false;
        }

        DrawRoomOnMap(bossRoom);
        return foundAvailableLocation;
    }

    int RandomRoomNumber()
    {
        return 6;
    }

    private bool GenerateSpecialRoom(Sprite mapIcon, int roomNumber)
    {
        List<Room> ShuffledList = new List<Room>(Level.rooms);

        Room specialRoom = new Room();
        specialRoom.roomImage = mapIcon;
        specialRoom.roomNumber = roomNumber;

        bool foundAvailableLocation = false;

        foreach (Room r in ShuffledList)
        {
            Vector2 specialRoomLocation = r.location;

            if (r.roomNumber < 6)
            {
                continue;
            }

            //Left
            if (!CheckIfRoomExists(specialRoomLocation + new Vector2(-1, 0)))
            {
                if (!CheckIfRoomAroundGeneratedRoom(specialRoomLocation + new Vector2(-1, 0), "Right"))
                {
                    specialRoom.location = specialRoomLocation + new Vector2(-1, 0);
                    foundAvailableLocation = true;
                }
            }

            //Right
            else if (!CheckIfRoomExists(specialRoomLocation + new Vector2(1, 0)))
            {
                if (!CheckIfRoomAroundGeneratedRoom(specialRoomLocation + new Vector2(1, 0), "Left"))
                {
                    specialRoom.location = specialRoomLocation + new Vector2(1, 0);
                    foundAvailableLocation = true;
                }
            }

            //Up
            else if (!CheckIfRoomExists(specialRoomLocation + new Vector2(0, 1)))
            {
                if (!CheckIfRoomAroundGeneratedRoom(specialRoomLocation + new Vector2(0, 1), "Down"))
                {
                    specialRoom.location = specialRoomLocation + new Vector2(0, 1);
                    foundAvailableLocation = true;
                }
            }

            //Down
            else if (!CheckIfRoomExists(specialRoomLocation + new Vector2(0, -1)))
            {
                if (!CheckIfRoomAroundGeneratedRoom(specialRoomLocation + new Vector2(0, -1), "Up"))
                {
                    specialRoom.location = specialRoomLocation + new Vector2(0, -1);
                    foundAvailableLocation = true;
                }
            }
        }

        if (foundAvailableLocation)
        {
            DrawRoomOnMap(specialRoom);
            return true;
        }
        else
        {
            return false;
        }
    }

    void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        System.Random rng = new System.Random();

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

    }

}
