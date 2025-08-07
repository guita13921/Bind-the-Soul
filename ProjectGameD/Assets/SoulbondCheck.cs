using UnityEngine;

public class SoulbondCheck : MonoBehaviour
{
    [SerializeField] private CharacterData characterData;

    private void Start()
    {
        if (characterData != null && characterData.deathCount > 5)
        {
            gameObject.SetActive(false);
        }
    }
}
