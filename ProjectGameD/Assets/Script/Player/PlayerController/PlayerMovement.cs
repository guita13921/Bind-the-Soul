using UnityEngine;

public partial class PlayerControl
{
    private bool isSprint = false;
    [SerializeField] private float _speed = 6.5f;
    [SerializeField] private float _turnSpeed = 500;
    private void Move()
    {
        float currentSpeed = _speed;

        if (isSprint)
        {
            currentSpeed = 10f;
        }

        _rb.MovePosition(
            transform.position + transform.forward * _input.normalized.magnitude * currentSpeed * Time.deltaTime
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
