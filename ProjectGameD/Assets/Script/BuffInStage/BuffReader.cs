using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BuffReader : MonoBehaviour
{
    public CharacterData characterData;
    public Transform buffUIPanel; // Parent object for UI buttons
    public GameObject buffButtonPrefab; // Prefab for buff buttons

    private List<Buff> buffs;

    public Animator animator;
    public EventSystem eventSystem;

    void Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();

        if (eventSystem != null)
        {
            Debug.Log("EventSystem found: " + eventSystem.name);
        }

        InitializeBuffs();
        ShowRandomAvailableBuffs();
    }

    public void ResetBuff()
    {
        InitializeBuffs();
        ShowRandomAvailableBuffs();
    }

    void InitializeBuffs()
    {
        buffs = new List<Buff>
        {
            new Buff(
                data => $"Vampirism (LV.{data.vampirism} -> LV.{data.vampirism + 1})",
                data => data.vampirism < 3,
                data => data.vampirism++,
                data => $"Restores player health by {2 + data.vampirism} for every attack",
                data => "Normal Attack"
            ),
            new Buff(
                data =>
                    $"Iron Body (LV.{data.reduceIncomeDamage} -> LV.{data.reduceIncomeDamage + 1})",
                data => data.reduceIncomeDamage < 3,
                data => data.reduceIncomeDamage++,
                data => $"Reduce incoming damage by {5 * (data.reduceIncomeDamage + 1)}%",
                data => "Passive"
            ),
            new Buff(
                data => $"Super Special (LV.{data.specialLV} -> LV.{data.specialLV + 1})",
                data => data.specialLV < 3,
                data => data.specialLV++,
                data => "Upgrade the special attack to a stronger version",
                data => "Special Attack"
            ),
            new Buff(
                data =>
                    $"Threshold Revival (LV.{data.healToThreshold} -> LV.{data.healToThreshold + 1})",
                data => data.healToThreshold < 3,
                data => data.healToThreshold++,
                data =>
                    $"Heal the player's health points to the health threshold of {20 * (data.healToThreshold + 1)}% after entering a new stage",
                data => "Passive"
            ),
            new Buff(
                data =>
                    $"Acceleration (LV.{data.QKReduceCooldown} -> LV.{data.QKReduceCooldown + 1})",
                data => data.QKReduceCooldown < 3,
                data => data.QKReduceCooldown++,
                data =>
                    $"Reduce special attack and skill cooldown by (data.QKReduceCooldown + 1) second",
                data => "Passive"
            ),
            new Buff(
                data => $"Overgrowth (LV.{data.maxHPIncrease} -> LV.{data.maxHPIncrease + 1})",
                data => data.maxHPIncrease < 3,
                data => data.maxHPIncrease++,
                data => $"Increase max health points by {1000 * (data.maxHPIncrease + 1)}",
                data => "Passive"
            ),
            new Buff(
                data =>
                    $"Lethal Strike (LV.{data.normalAttackCrit} -> LV.{data.normalAttackCrit + 1})",
                data => data.normalAttackCrit < 3,
                data => data.normalAttackCrit++,
                data =>
                    $"Normal attack now has a {10 * (data.normalAttackCrit + 1)}% chance to triple the damage",
                data => "Normal Attack"
            ),
            new Buff(
                data => $"Superspeed (LV.{data.moveFaster} -> LV.{data.moveFaster + 1})",
                data => data.moveFaster < 3,
                data => data.moveFaster++,
                data => $"Move faster",
                data => "Passive"
            ),
            new Buff(
                data =>
                    $"Swift Step (LV.{data.ReduceDashCooldown} -> LV.{data.ReduceDashCooldown + 1})",
                data => data.ReduceDashCooldown < 3,
                data => data.ReduceDashCooldown++,
                data => $"Reduce dash cooldown by {15 * (data.ReduceDashCooldown + 1)}%",
                data => "Passive"
            ),
            new Buff(
                data =>
                    $"Swift Step (LV.{data.reduceIncomeDamageDependOnHP} -> LV.{data.reduceIncomeDamageDependOnHP + 1})",
                data => data.reduceIncomeDamageDependOnHP < 3,
                data => data.reduceIncomeDamageDependOnHP++,
                data =>
                    $"If HP is less than 25%, reduce incoming damage by {15 * (data.reduceIncomeDamageDependOnHP + 1)}%",
                data => "Passive"
            ),
            new Buff(
                data =>
                    $"Last Stand (LV.{data.addDamageDependOnHP} -> LV.{data.addDamageDependOnHP + 1})",
                data => data.addDamageDependOnHP < 3,
                data => data.addDamageDependOnHP++,
                data =>
                    $"If HP is less than 25%, increase damage by {15 * (data.addDamageDependOnHP + 1)}%",
                data => "Passive"
            ),
            new Buff(
                data => $"Electric Trail (LV.{data.dashExplode} -> LV.{data.dashExplode + 1})",
                data => data.dashExplode < 3,
                data => data.dashExplode++,
                data =>
                    data.dashExplode == 0
                        ? $"When Dash left an electric bomb behind"
                        : "Increase electric bomb damage",
                data => "Passive"
            ),
            new Buff(
                data => $"Evolution: Red Blade",
                data => data.QSkillType == 0,
                data => data.QSkillType = 1,
                data => $"Change skill to Red Blade",
                data => "Skill"
            ),
            new Buff(
                data => $"Evolution: Homing Bullet",
                data => data.QSkillType == 0,
                data => data.QSkillType = 2,
                data => $"Change skill to a bullet that follows the enemy",
                data => "Skill"
            ),
            new Buff(
                data => $"Evolution: Aura",
                data => data.QSkillType == 0,
                data => data.QSkillType = 3,
                data => $"Change skill to an area of effect damage",
                data => "Skill"
            ),
            new Buff(
                data => $"Killing Strike",
                data => data.QSkillType == 1,
                data => data.Q1_QKDamageUp = true,
                data => $"Increase skill and special attack damage by 25%",
                data => "Evo:Skill"
            ),
            new Buff(
                data => $"Maximum Output",
                data => data.QSkillType == 1,
                data => data.Q1_QKFasterWider = true,
                data => $"Increase the size of special attack and skill",
                data => "Evo:Skill"
            ),
            /*
            new Buff(
                data => $"Bloodlust",
                data => data.QSkillType == 1,
                data => data.Q1_QKKillEnemyDamageUp = true,
                data => $"Increase the size of special attack and skill",
                data => "Evo:Skill"
            ),*/

            new Buff(
                data => $"Super Lethal",
                data => data.QSkillType == 2,
                data => data.Q2_QKCrit = true,
                data => $"Special attack and skill have a 20% chance to deal triple damage",
                data => "Evo:Skill"
            ),
            new Buff(
                data => $"Stackable",
                data => data.QSkillType == 2,
                data => data.Q2_QKStackable = true,
                data => $"Special skill and skill can now be stored with a maximum of 2",
                data => "Evo:Skill"
            ),
            new Buff(
                data => $"Little Bee",
                data => data.QSkillType == 2,
                data => data.Q2_SmallBullet = true,
                data => $"There is a small bullet after the attack",
                data => "Evo:Skill"
            ),

            /*
                         new Buff(
                            data => $"Hand broker",
                            data => data.QSkillType == 3,
                            data => data.Q3_QKWeak = true,
                            data => $"Enemy attack reduce",
                            data => "Evo:Skill"
                        ),
                         new Buff(
                            data => $"Little bee",
                            data => data.QSkillType == 3,
                            data => data.Q3_QKExplode = true,
                            data => $"There a small bullet after atttack",
                            data => "Evo:Skill"
                        ),
                         new Buff(
                            data => $"Little bee",
                            data => data.QSkillType == 3,
                            data => data.Q3_Barrier = true,
                            data => $"There a small bullet after atttack",
                            data => "Evo:Skill"
                        ),*/
        };
    }

    void ShowRandomAvailableBuffs()
    {
        // Clear existing buttons
        foreach (Transform child in buffUIPanel)
        {
            if (child.transform.name != "Reset")
                Destroy(child.gameObject);
        }

        // Filter available buffs
        List<Buff> availableBuffs = buffs.FindAll(buff => buff.isAvailable(characterData));

        RectTransform panelRect = buffUIPanel.GetComponent<RectTransform>();
        float panelHeight = panelRect.rect.height; // Get the height of the panel
        float buttonHeight = panelHeight / 4; // Divide space for consistent spacing

        float[] yOffsets = { 0, buttonHeight, -buttonHeight };

        for (int i = 0; i < 3 && availableBuffs.Count > 0; i++)
        {
            int index = UnityEngine.Random.Range(0, availableBuffs.Count);
            Buff selectedBuff = availableBuffs[index];
            availableBuffs.RemoveAt(index);

            // Create a button for the buff
            GameObject buttonObj = Instantiate(buffButtonPrefab, buffUIPanel);
            Transform childd = buttonObj.transform.GetChild(0);

            // Adjust position relative to the panel
            RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();

            buttonRect.anchoredPosition = new Vector2(0, yOffsets[i]);

            Button button = childd.GetComponent<Button>();

            Transform child1 = childd.transform.GetChild(0);
            TMPro.TextMeshProUGUI MainText = child1.GetComponent<TMPro.TextMeshProUGUI>();
            Transform child2 = childd.transform.GetChild(1);
            TMPro.TextMeshProUGUI DescriptionText = child2.GetComponent<TMPro.TextMeshProUGUI>();
            Transform child3 = childd.transform.GetChild(2);
            TMPro.TextMeshProUGUI TypeText = child3.GetComponent<TMPro.TextMeshProUGUI>();

            if (MainText != null)
            {
                MainText.text = selectedBuff.name(characterData);
                DescriptionText.text = selectedBuff.description(characterData);
                TypeText.text = selectedBuff.type(characterData);
            }
            else
            {
                Debug.LogWarning("Text component not found on the buff button prefab.");
            }

            // Add click event to apply the buff
            button.onClick.AddListener(() =>
            {
                selectedBuff.applyEffect(characterData);
                animator.Play("Out");

                Debug.Log($"{selectedBuff.name} applied!");
                eventSystem.enabled = false;
            });
        }
    }

    void Update()
    {
        // Check if the "R" key is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                characterData.ResetToDefault();
            }
            // Reload the current active scene
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
