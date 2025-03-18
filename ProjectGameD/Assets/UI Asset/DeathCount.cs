using UnityEngine;
using UnityEngine.UI;

public class DeathCount : MonoBehaviour
{
    public CharacterData characterData;
    private Image imageComponent;
    int death = 5;
    [SerializeField]bool first = true;
    void Start()
    {
        imageComponent = GetComponent<Image>();

        int deathcountt =characterData.deathCount;
        if (first) deathcountt--;
        imageComponent.color = Color.Lerp(Color.red, Color.white, (float)(death - deathcountt)
 / 5f);




    }
}
