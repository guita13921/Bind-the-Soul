using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileAttack : MonoBehaviour
{
    public GameObject[] skills;
    public Transform parentObject;
    public CharacterData characterData;
    public bool faster = false;

    void Start()
    {
        gameObject.SetActive(characterData.specialAttack == 3);

        StartCoroutine(InstantiateSkillCoroutine());
    }

    // Update is called once per frame
    void Update() { }

    IEnumerator InstantiateSkillCoroutine()
    {
        while (true)
        {
            if (FindObjectsOfType<Targetset>().Length > 0)
            {
                if (characterData.specialAdd2 && characterData.specialAdd1)
                {
                    var randomt = Random.value <= 0.35f ? skills[2] : skills[1];
                    Instantiate(randomt, parentObject);
                }
                else if (characterData.specialAdd2)
                {
                    Instantiate(skills[1], parentObject);
                }
                else
                {
                    Instantiate(skills[0], parentObject);
                }
            }

            float randomWaitTime = Random.Range(0.8f, 1.0f);
            float randomWaitTimeFast = Random.Range(0.3f, 0.5f);
            yield return new WaitForSeconds(faster ? randomWaitTimeFast : randomWaitTime);
        }
    }

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

        return null;
    }
}
