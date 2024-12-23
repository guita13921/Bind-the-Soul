using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightRay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Targetset closestTargetSet = FindRandomClosestTargetSet();

        // Move the LightRay object to the closest Targetset's position
        if (closestTargetSet != null)
        {
            transform.position = closestTargetSet.transform.position;
        }
    }

    // Update is called once per frame
    void Update() { }

    Targetset FindRandomClosestTargetSet()
    {
        Targetset[] targetSets = FindObjectsOfType<Targetset>();

        var sortedTargetSets = targetSets
            .OrderBy(targetSet =>
                Vector3.Distance(transform.position, targetSet.transform.position)
            )
            .ToArray();

        var closestThree = sortedTargetSets.Take(3).ToArray();

        if (closestThree.Length > 0)
        {
            int randomIndex = Random.Range(0, closestThree.Length);
            return closestThree[randomIndex];
        }

        return null; // No Targetset found
    }
}
