using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField]
    float _rayLength = 1f;

    [SerializeField]
    private Transform _rayPosition;

    [SerializeField]
    private LayerMask _groundLayer;

    // Raycast parameters
    // using in IsGrounded and
    // GetGroundAngle.
    Vector2 _position;
    Vector2 _direction = Vector2.down;

    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(_position, _direction, _rayLength, _groundLayer);

        if (hit.collider != null)
        {
            return true;
        }
        return false;
    }

    public Vector2 GetGroundNormal()
    {
        RaycastHit2D hit = Physics2D.Raycast(_position, _direction, _rayLength, _groundLayer);

        Vector2 groundNormal;

        if (hit.collider != null)
        {
            groundNormal = hit.normal;

            return groundNormal;
        }

        return Vector2.zero;
    }

    void Update()
    {
        _position = _rayPosition.position;
        Debug.DrawRay(_position, _direction * _rayLength, Color.red); 
    }
}
