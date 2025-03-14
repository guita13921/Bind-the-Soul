using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    [SerializeField] private string type;
    [SerializeField] private int cost;
    [SerializeField] private int sizeX;
    [SerializeField] private int sizeZ;
    [SerializeField] private int tileSize;

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
}
