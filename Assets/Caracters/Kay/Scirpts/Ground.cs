using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField]
    float rayLength = 1f;

    [Space]

    [SerializeField]
    private Transform[] rays;

    [Space]

    [SerializeField]
    Vector2 raycastDirection = Vector2.down;

    [Space]

    [SerializeField]
    private LayerMask _groundLayer;

    public bool IsGrounded()
    {
       
        for (int i = 0; i < rays.Length; i++)
        {
            RaycastHit2D raycast = Physics2D.Raycast(rays[i].position, raycastDirection, rayLength, _groundLayer);

            if (raycast == true)
            {
                return true;
            }
        }

        return false;
    }

    void Update()
    {
        for (int i = 0; i < rays.Length; i++)
        {
            Debug.DrawRay(rays[i].position, raycastDirection * rayLength, Color.red);
        }
    }
}
