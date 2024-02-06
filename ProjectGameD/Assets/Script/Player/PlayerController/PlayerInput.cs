using UnityEngine;

public partial class PlayerControl
{
    private Vector3 _input;

    private void GatherInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }
}
