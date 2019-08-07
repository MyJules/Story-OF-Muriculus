using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField]
    float _rayLength = 1f;

    [SerializeField]
    private Transform _firstRayPosition;

    [SerializeField]
    private Transform _secondRayPosition;

    [SerializeField]
    private LayerMask _groundLayer;


    Vector2 _direction = Vector2.down;

    public bool IsGrounded()
    {
        RaycastHit2D hit1 = Physics2D.Raycast(_firstRayPosition.position, _direction, _rayLength, _groundLayer);
        RaycastHit2D hit2 = Physics2D.Raycast(_secondRayPosition.position, _direction, _rayLength, _groundLayer);

        if (hit1.collider != null || hit2.collider != null)
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        Debug.DrawRay(_firstRayPosition.position, _direction * _rayLength, Color.red);
        Debug.DrawRay(_secondRayPosition.position, _direction * _rayLength, Color.red);
    }
}
