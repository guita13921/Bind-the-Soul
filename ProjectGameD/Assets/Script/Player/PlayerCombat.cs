using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;

public class PlayerCombat : MonoBehaviour
{
    public ProjectileAttack projectileAttack;
    public List<AttackSO> combo;
    public GameObject[] vfxPrefabs; // Array to hold references to VFX prefabs

    public GameObject[] speicalVFX;
    public Health health;
    public GameObject[] qSkill;
    public CharacterData characterData;
    float lastClickedTime; //time betweeen attack in combo
    float lastComboEnd; //amount of time before player can do the next combo
    public int comboCounter;

    public float specialAttackCooldown = 10f;
    public float castCooldown = 5f; // Set the cooldown duration for cast

    public float timeSinceLastSpecialAttack = 0f;
    public bool isSpecialAttackReady = true;
    Animator animator;

    [SerializeField]
    PlayerWeapon weapon;
    public SFX sfx;

    [SerializeField]
    BoxCollider boxCollider;
    public Transform parentObject; // The object inside which you want to spawn the new object

    public bool normalmode = true;
    public PlayerCD playerCD;

    bool forthAttack = false;
    bool check4thattack = false;
    public ControlPower controlPower;
    public GameObject[] heaven;
    float additionaldamage = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
        forthAttack = characterData.forthNormalAttack;

        heaven[0].SetActive(characterData.specialAttack == 3);
    }

    void Update()
    // Update is called once per frame
    {
        Cast();

        SpecialAttack();
        if (
            !animator.GetCurrentAnimatorStateInfo(0).IsName("GotHit")
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("Dash")
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("CAST") // Add check for CAST state
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("SPAttack") // Add check for SPAttack state
        )
        {
            if (Input.GetKeyDown(KeyCode.J) && !check4thattack)
            {
                Attack();
            }
        }

        ExitAttack();
    }

    void Attack()
    {
        CancelInvoke("EndCombo");

        if (
            Time.time - lastClickedTime >= 0.3f
            && !animator.GetCurrentAnimatorStateInfo(0).IsName("SPAttack")
        )
        {
            if (!forthAttack && comboCounter == 3)
            {
                comboCounter = 1;
            }
            animator.runtimeAnimatorController = combo[comboCounter].animatorOV;
            animator.Play("Attack", 0, 0);

            sfx.Slash();

            if (vfxPrefabs != null && vfxPrefabs.Length > 0)
            {
                GameObject vfxPrefab = vfxPrefabs[comboCounter];
                Instantiate(vfxPrefab, parentObject);
            }

            controlPower.StartVFX();
            int randomValue = UnityEngine.Random.Range(0, 10); // Generate a random integer between 0 and 9
            if (health.currentHealth < (health.maxHealth * 0.25f))
            {
                float add = characterData.addDamageDependOnHP * 0.15f;
                additionaldamage = combo[comboCounter].damage * add;
            }
            weapon.damage = combo[comboCounter].damage + 1000;

            if (randomValue < characterData.normalAttackCrit)
            {
                weapon.damage = Mathf.CeilToInt(combo[comboCounter].damage + additionaldamage);
                weapon.damage *= 3; //crit
            }
            else
            {
                weapon.damage = Mathf.CeilToInt(combo[comboCounter].damage + additionaldamage);
            }

            comboCounter++;

            if (comboCounter == 4)
            {
                check4thattack = true;
                StartCoroutine(WaitForAnimationToFinish());
            }
            lastClickedTime = Time.time;

            if (comboCounter >= combo.Count)
            {
                comboCounter = 0;
            }
        }
    }

    IEnumerator WaitForAnimationToFinish()
    {
        while (
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f
            && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")
        )
        {
            yield return null; // Wait for the next frame
        }
        check4thattack = false;

        comboCounter = 0;
        lastComboEnd = Time.time;
    }

    public GameObject KCooldown;
    public GameObject QCooldown;

    public bool isCastReady = true;
    public float timeSinceLastCast = 0f;

    void Cast()
    {
        if (!isCastReady)
        {
            timeSinceLastCast += Time.deltaTime;
            if (timeSinceLastCast > castCooldown)
            {
                isCastReady = true;
            }
        }
        if (isCastReady && Input.GetKeyDown(KeyCode.Q))
        {
            QCooldown.SetActive(true);

            timeSinceLastCast = 0;
            animator.Play("CAST", 0, 0);
            GameObject vfxPrefab = qSkill[0];
            Instantiate(vfxPrefab, parentObject);
            isCastReady = false;
        }
    }

    [SerializeField]
    float ksize = 1f;

    void SpecialAttack()
    {
        if (!isSpecialAttackReady)
        {
            timeSinceLastSpecialAttack += Time.deltaTime;
            if (timeSinceLastSpecialAttack > (0.6 * specialAttackCooldown))
            {
                projectileAttack.faster = false;
                heaven[1].SetActive(false);
            }

            if (timeSinceLastSpecialAttack >= specialAttackCooldown)
            {
                isSpecialAttackReady = true;
                timeSinceLastSpecialAttack = 0f;
            }
        }
        if (isSpecialAttackReady && Input.GetKeyDown(KeyCode.K))
        {
            KCooldown.SetActive(true);
            GameObject vfxPrefab = speicalVFX[0];
            if (characterData.Q1_QKFasterWider)
                vfxPrefab.transform.localScale = new Vector3(ksize, ksize, ksize); // Set scale to (1, 1, 1)

            Instantiate(vfxPrefab, parentObject);

            sfx.SkillSlash();
            if (characterData.specialAttack == 1 || characterData.specialAttack == 3)
            {
                animator.Play("CAST", 0, 0);
                heaven[1].SetActive(characterData.specialAttack == 3);

                projectileAttack.faster = true;
            }
            else
            {
                animator.Play("SPAttack", 0, 0);
            }
            isSpecialAttackReady = false;
        }
    }

    void ExitAttack()
    {
        if (
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f
            && animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack")
        )
        {
            Invoke("EndCombo", 0.5f);
        }
    }

    void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
    }

    void EnableAttack()
    {
        boxCollider.enabled = true;
    }

    void DisableAttack()
    {
        boxCollider.enabled = false;
    }
}
