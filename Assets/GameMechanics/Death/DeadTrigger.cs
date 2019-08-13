using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDie dead = collision.GetComponent<IDie>();

        if (dead)
        {
            dead.Die();
        }

    }
}
