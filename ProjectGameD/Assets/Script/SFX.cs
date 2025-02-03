using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour
{
    public CharacterData characterData;

    [SerializeField]
    GameObject Sword_SFX;

    [SerializeField]
    GameObject SwordSpin_SFX;

    [SerializeField]
    GameObject SwordSpin_human;

    [SerializeField]
    GameObject BigSwordSFX;

    [SerializeField]
    GameObject LightBulletSFX;

    [SerializeField]
    GameObject Hit_SFX;

    [SerializeField]
    GameObject[] Normal_humanSFX;

    // Start is called before the first frame update

    public void Slash()
    {
        GameObject sfx = Instantiate(Sword_SFX);

        int randomIndex = Random.Range(0, Normal_humanSFX.Length);
        GameObject humansfx = Instantiate(Normal_humanSFX[randomIndex]);
    }

    public void SkillSlash()
    {
        {
            GameObject sfx = Instantiate(SwordSpin_SFX);
            GameObject sfx2 = Instantiate(SwordSpin_human);
        }
    }

    public void Hit()
    {
        GameObject sfx = Instantiate(Hit_SFX);
    }
}
