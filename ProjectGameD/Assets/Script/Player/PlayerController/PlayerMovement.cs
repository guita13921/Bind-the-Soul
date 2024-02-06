using UnityEngine;

public partial class PlayerControl
{
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private float _turnSpeed = 500;
    private void Move()
    {
   
        _rb.MovePosition(
            transform.position + transform.forward * _input.normalized.magnitude * _speed * Time.deltaTime
        );
    }

    private void Look()
    {
        if (_input.magnitude > 0.01f)
        {
            var dot = Vector3.Dot(transform.forward, _input.normalized);
            var rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);

            if (dot < -0.2f)
            {
                transform.rotation = rot;
            }
            else
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    rot,
                    _turnSpeed * Time.deltaTime
                );
            }
        }
    }
}
