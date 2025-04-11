using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class EnemyRoomManager : MonoBehaviour
    {
        [Header("Room Enemy Manager")]
        public GameObject enemyPrefab; // The enemy to spawn
        public List<EnemyStat> enemiesInRoom = new List<EnemyStat>();
        public bool roomCleared = false;

        public void SpawnEnemiesInRoom(Room currentRoom)
        {
            enemiesInRoom.Clear();

            Transform spawnerParent = GameObject.Find(currentRoom.roomNumber.ToString()).transform.Find("Spawners");
            if (spawnerParent == null)
            {
                Debug.LogWarning("Spawner parent not found in room: " + currentRoom.roomNumber);
                return;
            }

            foreach (Transform spawnPoint in spawnerParent)
            {
                GameObject enemyGO = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
                EnemyStat enemyStat = enemyGO.GetComponent<EnemyStat>();

                if (enemyStat != null)
                {
                    enemiesInRoom.Add(enemyStat);
                }
                else
                {
                    Debug.LogWarning("Enemy prefab is missing EnemyStat component.");
                }

                Level.enemyCount++; // Track global enemy count if you're using it
            }
        }

        public void OnEnemyDefeated(EnemyStat defeatedEnemy)
        {
            enemiesInRoom.Remove(defeatedEnemy);
            Level.enemyCount--;

            if (enemiesInRoom.Count <= 0 && !roomCleared)
            {
                roomCleared = true;
                RoomCleared();
            }
        }

        private void RoomCleared()
        {
            Transform doors = GameObject.Find(PlayerManager.currentRoom.roomNumber.ToString()).transform.Find("Doors");
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
