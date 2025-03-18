using UnityEngine;
using Unity.AI.Navigation;
using System.Collections.Generic;

public class NavMeshBaker : MonoBehaviour
{
    public string floorTag = "Floor";
    [SerializeField] private NavMeshSurface[] navMeshSurfaces;


    public void BakeAllFloors()
    {
        Debug.Log("Baking NavMesh for all floors...");
        for (int i = 0; i < navMeshSurfaces.Length; i++)
        {
            navMeshSurfaces[i].BuildNavMesh();
        }
        Debug.Log("All floors baked!");
    }
}
