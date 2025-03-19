using System.Collections.Generic;
using UnityEngine;

public class TestCreateRoom : MonoBehaviour
{
    [SerializeField] private GameObject roomPrefab; // Assign your Room Prefab in the Inspector
    [SerializeField] private Vector3 spawnPosition;// Default spawn position

    [SerializeField] private float axisX;
    [SerializeField] private float axisZ;
    [SerializeField] private float temp_rotation;// transform.rotation.eulerAngles.y;

    [SerializeField] private bool CanSpawn = true;

    private RoomInfo roomInfo;
    private float tolerance = 0.1f;
    public NavMeshBaker navMeshBaker;


    void Start()
    {
        CreateRoom();
        spawnPosition = this.transform.position;
        navMeshBaker.BakeAllFloors();
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
            CalculateRotation();
            CalculateShift();
            Quaternion rotation = Quaternion.Euler(0, temp_rotation, 0); // Example rotation
            Vector3 spawnPosition = transform.position + new Vector3(axisX, 0, axisZ);
            Instantiate(roomPrefab, spawnPosition, rotation);
        }
        else
        {
            Debug.LogError("Room Prefab is not assigned!");
        }
    }

    void CalculateAxis()
    {
        float rotationY = this.transform.rotation.eulerAngles.y;
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
            Debug.Log("IT ALL WRONG");
        }
    }

    void CalculateRotation()
    {
        float rotationY = this.transform.rotation.eulerAngles.y;
        if (Mathf.Abs(rotationY - 0f) < tolerance || Mathf.Abs(rotationY - 360f) < tolerance)
        {
            // Facing Forward (Z+)
            temp_rotation = 180f;

        }
        else if (Mathf.Abs(rotationY - 90f) < tolerance)
        {
            // Facing Right (X+)
            temp_rotation = -90f;

        }
        else if (Mathf.Abs(rotationY - 180f) < tolerance)
        {
            // Facing Backward (Z-)
            temp_rotation = 0;

        }
        else if (Mathf.Abs(rotationY - 270f) < tolerance)
        {
            // Facing Left (X-)
            temp_rotation = 90f;

        }
        else
        {
            Debug.Log("IT ALL WRONG");
        }
    }

    void CalculateShift()
    {
        float rotationY = this.transform.rotation.eulerAngles.y;
        if (Mathf.Abs(rotationY - 0f) < tolerance || Mathf.Abs(rotationY - 360f) < tolerance)
        {
            // Facing Forward (Z+)
            axisX += roomInfo.Get_shift();

        }
        else if (Mathf.Abs(rotationY - 90f) < tolerance)
        {
            // Facing Right (X+)
            axisZ -= roomInfo.Get_shift();
        }
        else if (Mathf.Abs(rotationY - 180f) < tolerance)
        {
            // Facing Backward (Z-)
            axisX -= roomInfo.Get_shift();
        }
        else if (Mathf.Abs(rotationY - 270f) < tolerance)
        {
            // Facing Left (X-)
            axisZ += roomInfo.Get_shift();
        }
        else
        {
            Debug.Log("IT ALL WRONG");
        }
    }
}
