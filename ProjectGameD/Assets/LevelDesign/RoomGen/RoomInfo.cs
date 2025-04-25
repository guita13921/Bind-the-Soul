using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    [SerializeField] private string type;
    [SerializeField] private int cost;
    [SerializeField] public int sizeX;
    [SerializeField] public int sizeZ;
    [SerializeField] private int tileSize;
    [SerializeField] private float shift;

    public GameObject roomPrefabs;



    public string Get_type()
    {
        return type;
    }

    public int Get_cost()
    {
        return cost;
    }

    public int Get_sizeX()
    {
        return sizeX;
    }

    public int Get_sizeZ()
    {
        return sizeZ;
    }

    public int Get_tileSize()
    {
        return tileSize;
    }

    public float Get_shift()
    {
        return shift;
    }
}
