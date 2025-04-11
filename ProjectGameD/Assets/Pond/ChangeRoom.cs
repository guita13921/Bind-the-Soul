using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SG
{

    public class ChangeRoom : MonoBehaviour
    {
        public Transform rooms;
        public float roomSpawnOffSet = 5;
        public EnemyRoomManager enemyRoomManager;

        private Sprite previousImage;

        void Start()
        {
            previousImage = Level.defaultRoomIcon;
            EnableDoor(PlayerManager.currentRoom);
        }

        void ChangeRoomIcon(Room currentRoom, Room newRoom)
        {
            currentRoom.roomImage.sprite = previousImage;
            previousImage = newRoom.roomImage.sprite;
            newRoom.roomImage.sprite = Level.currentRoomIcon;
        }

        public void EnableDoor(Room r)
        {
            Transform t = rooms.Find(r.roomNumber.ToString());
            Transform doors = t.Find("Doors");

            // First disable all doors and walls
            foreach (Transform child in doors)
            {
                child.gameObject.SetActive(false);
            }

            // Left
            {
                Vector2 newPosition = r.location + new Vector2(-1, 0);
                if (Level.rooms.Exists(x => x.location == newPosition))
                {
                    doors.Find("Left Door").gameObject.SetActive(true);
                }
                else
                {
                    doors.Find("Left Wall").gameObject.SetActive(true);
                }
            }

            // Up
            {
                Vector2 newPosition = r.location + new Vector2(0, 1);
                if (Level.rooms.Exists(x => x.location == newPosition))
                {
                    doors.Find("Top Door").gameObject.SetActive(true);
                }
                else
                {
                    doors.Find("Top Wall").gameObject.SetActive(true);
                }
            }

            // Down
            {
                Vector2 newPosition = r.location + new Vector2(0, -1);
                if (Level.rooms.Exists(x => x.location == newPosition))
                {
                    doors.Find("Bottom Door").gameObject.SetActive(true);
                }
                else
                {
                    doors.Find("Bottom Wall").gameObject.SetActive(true);
                }
            }

            // Right
            {
                Vector2 newPosition = r.location + new Vector2(1, 0);
                if (Level.rooms.Exists(x => x.location == newPosition))
                {
                    doors.Find("Right Door").gameObject.SetActive(true);
                }
                else
                {
                    doors.Find("Right Wall").gameObject.SetActive(true);
                }
            }
        }

        bool changeRoomCooldown = false;

        void EndChangeRoomColdown()
        {
            changeRoomCooldown = false;
        }

        private void OnTriggerEnter(Collider hit)
        {
            if (changeRoomCooldown || PlayerManager.currentRoom.cleared != true)
            {
                return;
            }
            else
            {
                changeRoomCooldown = true;
                Invoke(nameof(EndChangeRoomColdown), Level.RoomChangeTime);
            }

            if (hit.gameObject.name == "Left Door")
            {
                CheckDoor(new Vector2(-1, 0), "Right Door", new Vector3(-roomSpawnOffSet, 0, 0));
            }

            if (hit.gameObject.name == "Right Door")
            {
                CheckDoor(new Vector2(1, 0), "Left Door", new Vector3(roomSpawnOffSet, 0, 0));
            }

            if (hit.gameObject.name == "Top Door")
            {
                CheckDoor(new Vector2(0, 1), "Bottom Door", new Vector3(0, 0, roomSpawnOffSet));
            }

            if (hit.gameObject.name == "Bottom Door")
            {
                CheckDoor(new Vector2(0, -1), "Top Door", new Vector3(0, 0, -roomSpawnOffSet));
            }
        }

        public static void ReDrawRevealRoom()
        {
            foreach (Room room in Level.rooms)
            {
                if (!room.revealed && !room.explored) room.roomImage.color = new Color(1, 1, 1, 0);
                if (room.revealed && !room.explored && room.roomNumber > 5) room.roomImage.sprite = Level.unexploreRoom;
                if (room.explored && room.roomNumber > 5) room.roomImage.sprite = Level.defaultRoomIcon;
                if (room.explored || room.revealed) room.roomImage.color = new Color(1, 1, 1, 1);
                PlayerManager.currentRoom.roomImage.sprite = Level.currentRoomIcon;
            }

        }

        public static void RevealRoom(Room r)
        {
            foreach (Room room in Level.rooms)
            {
                //Left
                if (room.location == r.location + new Vector2(-1, 0))
                {
                    room.revealed = true;
                }

                //Right
                if (room.location == r.location + new Vector2(1, 0))
                {
                    room.revealed = true;
                }

                //Down
                if (room.location == r.location + new Vector2(0, -1))
                {
                    room.revealed = true;
                }

                //Top
                if (room.location == r.location + new Vector2(0, 1))
                {
                    room.revealed = true;
                }
            }
        }

        void CheckDoor(Vector2 newLocation, string direction, Vector3 roomSpawnOffSet)
        {
            //Where are we?
            Vector2 location = PlayerManager.currentRoom.location;

            //Where are we going?
            location = location + newLocation;

            if (Level.rooms.Exists(x => x.location == location))
            {
                Room r = Level.rooms.First(x => x.location == location);

                //disable the room that you are in
                rooms.Find(PlayerManager.currentRoom.roomNumber.ToString()).gameObject.SetActive(false);

                //Find the new Room and Active it
                GameObject newRoom = rooms.Find(r.roomNumber.ToString()).gameObject;
                newRoom.SetActive(true);

                //Move the player to the door area where he would be coming to
                PlayerManager.transform.position = newRoom.transform.Find("Doors").transform.Find(direction).position + roomSpawnOffSet;
                ChangeRoomIcon(PlayerManager.currentRoom, r);

                PlayerManager.currentRoom = r;

                EnableDoor(r);

                PlayerManager.currentRoom.explored = true;

                RevealRoom(r);
                ReDrawRevealRoom();

                Transform enemy = newRoom.transform.Find("Enemies");

                if (enemy != null)
                {
                    PlayerManager.currentRoom.cleared = false;
                    Level.enemyCount = enemy.childCount;
                    enemyRoomManager = GameObject.Find(PlayerManager.currentRoom.roomNumber.ToString())
                              .transform.Find("Spawners")
                              .GetComponent<EnemyRoomManager>();

                    if (enemyRoomManager != null)
                    {
                        enemyRoomManager.SpawnEnemiesInRoom(PlayerManager.currentRoom);
                    }
                }
                else
                {
                    Debug.LogWarning("Enemies object not found in the new room. Assuming room is already cleared.");

                    Transform doors = newRoom.transform.Find("Doors");
                    if (doors != null)
                    {
                        OpenDoorIfExists(doors, "Left Door");
                        OpenDoorIfExists(doors, "Right Door");
                        OpenDoorIfExists(doors, "Top Door");
                        OpenDoorIfExists(doors, "Bottom Door");
                    }
                    else
                    {
                        Debug.LogWarning("Doors object not found in the new room.");
                    }
                }

                // Helper method to open a door if it exists
                void OpenDoorIfExists(Transform doors, string doorName)
                {
                    Transform door = doors.Find(doorName);
                    if (door != null)
                    {
                        DoorManager doorManager = door.GetComponent<DoorManager>();
                        if (doorManager != null)
                        {
                            doorManager.OpenDoor();
                        }
                        else
                        {
                            Debug.LogWarning($"DoorManager not found on {doorName}.");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"{doorName} not found in Doors.");
                    }
                }

            }
        }
    }
}