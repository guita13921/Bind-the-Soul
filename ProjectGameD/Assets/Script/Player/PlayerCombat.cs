using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
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

    public float timeSinceLastSpecialAttack = 0f;
    public bool isSpecialAttackReady = true;
    public bool isSpecialAttackReady2 = true;

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
    float additionaldamage = 0;

    public bool isShield1 = false;
    public bool isShield2 = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    // Update is called once per frame
    {
        Cast2();
        SpecialAttack2();

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

    [SerializeField]
    Image Kimage;

    [SerializeField]
    Image Qimage;

    public GameObject KCooldown2;
    public GameObject QCooldown2;

    [SerializeField]
    Image Kimage2;

    [SerializeField]
    Image Qimage2;
    public bool isCastReady = true;
    public bool isCastReady2 = true;

    public float timeSinceLastCast = 0f;
    public float timeSinceLastCast2 = 0f;
    public float castCooldown = 5f;

    void Cast()
    {
        if (!isCastReady)
        {
            timeSinceLastCast += Time.deltaTime;
            if (timeSinceLastCast > castCooldown)
            {
                timeSinceLastCast = 0;

                isCastReady = true;
            }
        }
        if (isCastReady && Input.GetKeyDown(KeyCode.Q))
        {
            QCooldown.SetActive(true);
            Qimage.color = new Color(0f, 0f, 0f);

            animator.Play("CAST", 0, 0);
            GameObject vfxPrefab = qSkill[0];
            Instantiate(vfxPrefab, parentObject);
            isCastReady = false;
        }
        //Debug.Log("First attack: " + timeSinceLastCast);
    }

    void Cast2()
    {
        if (!isCastReady2)
        {
            timeSinceLastCast2 += Time.deltaTime;
            if (timeSinceLastCast2 > castCooldown)
            {
                timeSinceLastCast2 = 0;

                isCastReady2 = true;
            }
        }
        if (
            !isCastReady
            && isCastReady2
            && Input.GetKeyDown(KeyCode.Q)
            && characterData.Q2_QKStackable
        )
        {
            QCooldown2.SetActive(true);
            Qimage2.color = new Color(0, 0, 0f);

            animator.Play("CAST", 0, 0);
            GameObject vfxPrefab = qSkill[0];
            Instantiate(vfxPrefab, parentObject);
            isCastReady2 = false;
        }

        //Debug.Log("second attack: " + timeSinceLastCast2);
    }

    [SerializeField]
    float ksize = 1f;

    [SerializeField]
    GameObject barrier1;

    [SerializeField]
    GameObject barrier2;

    void SpecialAttack()
    {
        if (!isSpecialAttackReady)
        {
            timeSinceLastSpecialAttack += Time.deltaTime;
            if (timeSinceLastSpecialAttack >= specialAttackCooldown)
            {
                isSpecialAttackReady = true;
                timeSinceLastSpecialAttack = 0f;
            }
        }
        if (isSpecialAttackReady && Input.GetKeyDown(KeyCode.K))
        {
            KCooldown.SetActive(true);
            Kimage.color = new Color(0f, 0f, 0f);

            GameObject vfxPrefab = speicalVFX[0];
            if (characterData.Q1_QKFasterWider)
                vfxPrefab.transform.localScale = new Vector3(ksize, ksize, ksize); // Set scale to (1, 1, 1)
            Instantiate(vfxPrefab, parentObject);
            sfx.SkillSlash();
            animator.Play("SPAttack", 0, 0);
            isSpecialAttackReady = false;

            if (characterData.Q3_Barrier)
            {
                Instantiate(barrier1, parentObject);
                isShield1 = true;
            }
        }
        //Debug.Log("first attack: " + timeSinceLastSpecialAttack);
    }

    public float timeSinceLastSpecialAttack2 = 0f;

    void SpecialAttack2()
    {
        if (!isSpecialAttackReady2)
        {
            timeSinceLastSpecialAttack2 += Time.deltaTime;
            if (timeSinceLastSpecialAttack2 >= specialAttackCooldown)
            {
                isSpecialAttackReady2 = true;
                timeSinceLastSpecialAttack2 = 0f;
            }
        }
        if (
            !isSpecialAttackReady
            && isSpecialAttackReady2
            && Input.GetKeyDown(KeyCode.K)
            && characterData.Q2_QKStackable
        )
        {
            KCooldown2.SetActive(true);
            Kimage2.color = new Color(0f, 0f, 0f);

            GameObject vfxPrefab = speicalVFX[0];
            if (characterData.Q1_QKFasterWider)
                vfxPrefab.transform.localScale = new Vector3(ksize, ksize, ksize); // Set scale to (1, 1, 1)
            Instantiate(vfxPrefab, parentObject);
            sfx.SkillSlash();
            animator.Play("SPAttack", 0, 0);
            isSpecialAttackReady2 = false;
            if (characterData.Q3_Barrier)
            {
                Instantiate(barrier2, parentObject);
                isShield2 = true;
            }
        }
        //Debug.Log("second attack: " + timeSinceLastSpecialAttack2);
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
