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
    GameObject BigSwordSFX;

    [SerializeField]
    GameObject LightBulletSFX;

    [SerializeField]
    GameObject Hit_SFX;

    // Start is called before the first frame update

    public void Slash()
    {
        GameObject sfx = Instantiate(Sword_SFX);
    }

    public void SkillSlash()
    {
        if (characterData.specialAttack == 1)
        {
            GameObject sfx = Instantiate(BigSwordSFX);
        }
        else if (characterData.specialAttack == 3)
        {
            GameObject sfx = Instantiate(LightBulletSFX);
        }
        else
        {
            GameObject sfx = Instantiate(SwordSpin_SFX);
        }
    }

    public void Hit()
    {
        GameObject sfx = Instantiate(Hit_SFX);
    }
}
