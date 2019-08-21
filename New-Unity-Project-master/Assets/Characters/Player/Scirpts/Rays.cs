using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rays : MonoBehaviour
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
    public LayerMask crossedLayer;

    public bool IsCrossed()
    {

        for (int i = 0; i < rays.Length; i++)
        {
            RaycastHit2D raycast = Physics2D.Raycast(rays[i].position, raycastDirection, rayLength, crossedLayer);

            if (raycast == true)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsCrossed(int fromRay, int toRay)
    {

        for (int i = fromRay; i <= toRay; i++)
        {
            RaycastHit2D raycast = Physics2D.Raycast(rays[i].position, raycastDirection, rayLength, crossedLayer);

            if (raycast == true)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsCrossedWith(int i)
    {
            RaycastHit2D raycast = Physics2D.Raycast(rays[i].position, raycastDirection, rayLength, crossedLayer);

            if (raycast == true)
            {
                return true;
            }

        return false;
    }

    public RaycastHit2D GetCrossInformaiton(int i)
    {
        return Physics2D.Raycast(rays[i].position, raycastDirection, rayLength, crossedLayer);
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < rays.Length; i++)
        {
            Debug.DrawRay(rays[i].position, raycastDirection * rayLength, Color.red);
        }
    }
}
