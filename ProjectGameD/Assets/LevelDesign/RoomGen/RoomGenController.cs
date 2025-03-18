using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenController : MonoBehaviour
{

    [SerializeField] private int currentStage;
    private int costCurrentStage;
    private int remainCost;
    private List<GameObject> roomPrefab = new List<GameObject>(); // Assign your Room Prefab in the Inspector

    void Start()
    {
        SetCost(currentStage);
    }

    void Update()
    {

    }

    void SetCost(int thisStage)
    {
        switch (thisStage)
        {
            case 1:
                costCurrentStage = 5;
                remainCost = costCurrentStage;
                break;
            default:
                break;
        }
    }
}
