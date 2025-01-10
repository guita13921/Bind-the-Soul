using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    public Animator animator;

    public void GoUp()
    {
        animator.Play("UpgradeUIUp");
    }

    public void GoDown()
    {
        animator.Play("UpgradeUIDown");
    }
}
