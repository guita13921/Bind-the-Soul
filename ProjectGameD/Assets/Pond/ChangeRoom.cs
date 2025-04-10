using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SG
{

    public class ChangeRoom : MonoBehaviour
    {
        public Transform rooms;
        public float roomSpawnOffSet = 0;

        void Awake()
        {
        }


        private void OnTriggerEnter(Collider hit)
        {
            Debug.Log(hit.gameObject.name);
            if (hit.gameObject.name == "Left Door")
            {
                Debug.Log("OnTriggerEnter");
                //Where are we?
                Vector2 location = PlayerManager.currentRoom.location;

                //Where are we going?
                location = location + new Vector2(-1, 0);

                if (Level.rooms.Exists(x => x.location == location))
                {
                    Room r = Level.rooms.First(x => x.location == location);

                    //disable the room that you are in
                    rooms.Find(PlayerManager.currentRoom.roomNumber.ToString()).gameObject.SetActive(false);

                    //Find the new Room and Active it
                    GameObject newRoom = rooms.Find(r.roomNumber.ToString()).gameObject;
                    newRoom.SetActive(true);

                    //Move the player to the door area where he would be coming to
                    PlayerManager.transform.position = newRoom.transform.Find("Doors").transform.Find("Right Door").position;
                }
            }
        }
    }
}
