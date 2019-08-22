using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        IMemory memory = collision.GetComponent<IMemory>();

        if (memory != null)
        {
            memory.Save();
        }
    }
}
