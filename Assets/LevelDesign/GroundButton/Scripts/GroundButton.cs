using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundButton : MonoBehaviour
{
    [SerializeField] public UnityEvent ButtonPressEvent;

    [SerializeField]
    public UnityEvent ButtonReleaseEvent;

    void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ButtonPressEvent.Invoke();   
        }
    }

    void OnTriggerExit2D(Collider2D collider2D)
    {
        if (collider2D.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            ButtonReleaseEvent.Invoke();
        }
    }

}

