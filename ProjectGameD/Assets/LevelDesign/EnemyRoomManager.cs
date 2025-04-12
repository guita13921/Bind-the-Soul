using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class EnemyRoomManager : MonoBehaviour
    {
        [Header("Room Enemy Manager")]
        public List<EnemyStat> enemiesInRoom = new List<EnemyStat>();
        public bool roomCleared = false;

        public void SpawnEnemiesInRoom(Room currentRoom)
        {
            enemiesInRoom.Clear();

            Transform spawnerParent = GameObject.Find(currentRoom.roomNumber.ToString()).transform.Find("Spawners");
            if (spawnerParent == null)
            {
                Debug.Log("Spawner parent not found in room: " + currentRoom.roomNumber);
                return;
            }

            GameObject enemiesParent = GameObject.Find("Enemies");
            if (enemiesParent == null)
            {
                enemiesParent = new GameObject("Enemies");
            }

            foreach (Transform spawnPoint in spawnerParent)
            {
                EnemySpawner spawner = spawnPoint.GetComponent<EnemySpawner>();
                if (spawner == null || spawner.enemyPrefab == null)
                {
                    Debug.LogWarning($"Spawner at {spawnPoint.name} is missing EnemySpawner component or has no enemyPrefab assigned.");
                    continue;
                }

                GameObject enemyGO = Instantiate(spawner.enemyPrefab, spawnPoint.position, Quaternion.identity, enemiesParent.transform);
                EnemyStat enemyStat = enemyGO.GetComponent<EnemyStat>();
                if (enemyStat != null)
                {
                    enemyStat.roomManager = this;
                    enemiesInRoom.Add(enemyStat);
                }
                else
                {
                    Debug.LogWarning("Enemy prefab is missing EnemyStat component.");
                }

                Level.enemyCount++; // optional global tracking
            }
        }

        public void OnEnemyDefeated(EnemyStat defeatedEnemy)
        {
            enemiesInRoom.Remove(defeatedEnemy);
            Level.enemyCount--;

            if (enemiesInRoom.Count <= 0)
            {
                roomCleared = true;
                PlayerManager.currentRoom.cleared = true;
                RoomCleared(PlayerManager.currentRoom);
            }
        }

        private void RoomCleared(Room currentRoom)
        {
            Transform doors = GameObject.Find(currentRoom.roomNumber.ToString()).transform.Find("Doors");

            if (doors != null)
            {
                OpenDoorIfExists(doors, "Left Door");
                OpenDoorIfExists(doors, "Right Door");
                OpenDoorIfExists(doors, "Top Door");
                OpenDoorIfExists(doors, "Bottom Door");
            }
            else
            {
                Debug.Log("Doors object not found in the new room.");
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
                        Debug.Log($"DoorManager not found on {doorName}.");
                    }
                }
                else
                {
                    Debug.Log($"{doorName} not found in Doors.");
                }
            }
        }

    }
}
