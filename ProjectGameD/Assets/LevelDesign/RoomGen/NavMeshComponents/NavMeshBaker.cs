using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class RuntimeNavMeshBaker : MonoBehaviour
{
    public string floorTag = "Floor";
    [SerializeField] private List<NavMeshSurface> navMeshSurfaces = new List<NavMeshSurface>();

    private void Start()
    {
        FindAndBakeFloors();
    }

    public void FindAndBakeFloors()
    {
        Debug.Log("Finding all floors...");

        // Clear old surfaces
        foreach (var surface in navMeshSurfaces)
        {
            Destroy(surface);
        }
        navMeshSurfaces.Clear();

        // Find all objects tagged as "Floor"
        GameObject[] floors = GameObject.FindGameObjectsWithTag(floorTag);

        foreach (GameObject floor in floors)
        {
            NavMeshSurface navMeshSurface = floor.GetComponent<NavMeshSurface>();

            Debug.Log(navMeshSurface);

            // If no NavMeshSurface is attached, add one
            if (navMeshSurface == null)
            {
                navMeshSurface = floor.AddComponent<NavMeshSurface>();
            }

            navMeshSurface.layerMask = LayerMask.GetMask(floor.layer.ToString()); // Uses floor's layer
            navMeshSurfaces.Add(navMeshSurface);
        }

        // Bake all floor surfaces
        BakeAllFloors();
    }

    public void BakeAllFloors()
    {
        Debug.Log("Baking NavMesh for all floors...");
        foreach (var surface in navMeshSurfaces)
        {
            surface.BuildNavMesh();
        }
        Debug.Log("All floors baked!");
    }
}
