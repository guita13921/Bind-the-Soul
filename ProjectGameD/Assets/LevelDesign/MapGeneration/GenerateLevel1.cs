using System;
using System.Collections;
using System.Collections.Generic;
using SG;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class GenerateLevel1 : MonoBehaviour
    {
        public Sprite currentRoom;
        public Sprite bossRoom;
        public Sprite emptyRoom;
        public Sprite shopRoom;
        public Sprite treasureRoom;
        public Sprite unexploreRoom;
        public Sprite secretRoom;
        public Sprite challengeRoom;

        public ChangeRoom changeRoom;

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
            Level.challengeRoom = challengeRoom;
        }

        void Start()
        {
            regenerating = true;
            Level.rooms.Clear();

            // Create Start Room (but don't assign or reveal yet)
            Room startRoom = new Room
            {
                location = Vector2.zero,
                roomSprite = Level.currentRoomIcon,
                explored = true,
                revealed = true,
                roomNumber = 0
            };

            Generate(startRoom);
            DrawRoomOnMap(startRoom);

            // Calculate total rooms to generate
            int targetRoomCount = UnityEngine.Random.Range(0, 2) + 5 + Mathf.RoundToInt(Level.currentLevel * 2.6f);
            Debug.Log(targetRoomCount);

            // BFS-style room expansion
            Queue<Room> frontier = new Queue<Room>();
            frontier.Enqueue(startRoom);

            Vector2[] directions = new Vector2[]
            {
        Vector2.left, Vector2.right, Vector2.up, Vector2.down
            };

            while (Level.rooms.Count < targetRoomCount && frontier.Count > 0)
            {
                Room current = frontier.Dequeue();

                foreach (Vector2 dir in directions)
                {
                    Vector2 newLocation = current.location + dir;

                    if (CheckIfRoomExists(newLocation)) continue;
                    if (UnityEngine.Random.value < Level.roomGenerationChance) continue;
                    if (CheckIfRoomAroundGeneratedRoom(newLocation, GetOppositeDirection(dir))) continue;

                    Room newRoom = new Room
                    {
                        location = newLocation,
                        roomSprite = Level.defaultRoomIcon,
                        roomNumber = RandomRoomNumber()
                    };

                    Generate(newRoom);
                    frontier.Enqueue(newRoom);

                    if (Level.rooms.Count >= targetRoomCount)
                        break;
                }
            }

            // Shuffle rooms
            ShuffleList(Level.rooms);

            // Special room generation
            bool boss = GenerateBossRoom();
            bool shop = GenerateSpecialRoom(Level.shopRoomIcon, 2);
            bool treasure = GenerateSpecialRoom(Level.treasureRoomIcon, 3);
            bool secret = GenerateSpecialRoom(Level.secretRoom, 4);
            bool challenge = GenerateSpecialRoom(Level.challengeRoom, 5);

            // If generation fails, cancel regeneration but do NOT assign PlayerManager.currentRoom
            if (!boss || !shop || !treasure || !secret || !challenge)
            {
                regenerating = false;
                return;
            }

            // ✅ Only now we finalize and assign the start room
            PlayerManager.currentRoom = startRoom;
            ChangeRoom.RevealRoom(startRoom);
            ChangeRoom.ReDrawRevealRoom();
            changeRoom.EnableDoor(startRoom);
        }

        void Update()
        {
            if (!regenerating)
            {
                regenerating = true;
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
            roomImage.sprite = room.roomSprite;
            room.roomImage = roomImage;
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

        void Generate(Room room)
        {
            // Limit boundary to avoid infinite maps
            if (Mathf.Abs(room.location.x) >= Level.RoomLimit || Mathf.Abs(room.location.y) >= Level.RoomLimit)
                return;

            // Prevent overlapping rooms
            if (CheckIfRoomExists(room.location))
                return;

            // Add to room list and draw
            Level.rooms.Add(room);
            DrawRoomOnMap(room);
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
            bossRoom.roomSprite = Level.bossRoomIcon;
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
            return UnityEngine.Random.Range(6, GameObject.Find("Rooms").transform.childCount);
        }

        private bool GenerateSpecialRoom(Sprite mapIcon, int roomNumber)
        {
            List<Room> ShuffledList = new List<Room>(Level.rooms);

            Room specialRoom = new Room();
            specialRoom.roomSprite = mapIcon;
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

        string GetOppositeDirection(Vector2 dir)
        {
            if (dir == Vector2.left) return "Right";
            if (dir == Vector2.right) return "Left";
            if (dir == Vector2.up) return "Down";
            if (dir == Vector2.down) return "Up";
            return "";
        }

    }
}