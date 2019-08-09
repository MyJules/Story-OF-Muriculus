using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickThePlayer : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            collision.collider.transform.SetParent(transform);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 9)
            collision.collider.transform.SetParent(null);
    }
}
