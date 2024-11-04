using UnityEngine;

[CreateAssetMenu(fileName = "DashCheck", menuName = "ScriptableObjects/DashCheck", order = 1)]
public class DashCheck : ScriptableObject
{
    public bool willCollide = false;

    // Method to set collision state
    public void SetCollisionState(bool state)
    {
        willCollide = state;
    }

    // Method to toggle collision state
    public void ToggleCollisionState()
    {
        willCollide = !willCollide;
    }
}
