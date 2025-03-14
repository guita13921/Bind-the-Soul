using System.Collections.Generic;
using UnityEngine;

public class TestCreateRoom : MonoBehaviour
{
    [SerializeField] private GameObject roomPrefab; // Assign your Room Prefab in the Inspector
    [SerializeField] private Vector3 spawnPosition;// Default spawn position
    [SerializeField] private float axisX;
    [SerializeField] private float axisZ;
    [SerializeField] private int tileSize;
    [SerializeField] private bool CanSpawn = true;

    private RoomInfo roomInfo;
    private float tolerance = 0.1f;


    void Start()
    {
        spawnPosition = this.transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && CanSpawn == true)
        {
            CreateRoom();
            Debug.Log("CreateRoom");
        }
    }

    void CreateRoom()
    {
        if (roomPrefab != null)
        {
            CanSpawn = false;
            roomInfo = roomPrefab.GetComponent<RoomInfo>();
            CalculateAxis();
            Vector3 spawnPosition = transform.position + new Vector3(axisX, 0, axisZ);
            Instantiate(roomPrefab, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Room Prefab is not assigned!");
        }
    }

    void CalculateAxis()
    {
        float rotationY = this.transform.rotation.eulerAngles.y;
        Debug.Log(rotationY);

        if (Mathf.Abs(rotationY - 0f) < tolerance || Mathf.Abs(rotationY - 360f) < tolerance)
        {
            // Facing Forward (Z+)
            axisZ = roomInfo.Get_sizeZ() * roomInfo.Get_tileSize() / 2;
            axisX = 0;
        }
        else if (Mathf.Abs(rotationY - 90f) < tolerance)
        {
            // Facing Right (X+)
            axisX = roomInfo.Get_sizeX() * roomInfo.Get_tileSize() / 2;
            axisZ = 0;
        }
        else if (Mathf.Abs(rotationY - 180f) < tolerance)
        {
            // Facing Backward (Z-)
            axisZ = roomInfo.Get_sizeZ() * -roomInfo.Get_tileSize() / 2;
            axisX = 0;
        }
        else if (Mathf.Abs(rotationY - 270f) < tolerance)
        {
            // Facing Left (X-)
            axisX = roomInfo.Get_sizeX() * -roomInfo.Get_tileSize() / 2;
            axisZ = 0;
        }
        else
        {
            Debug.Log("FUCK IT ALL WRONG");
        }
        //Debug.Log("CalculateAxis" + axisX + " : " + axisZ);
    }


}
