using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameControl : MonoBehaviour
{
    private FrameManager _frameManager;
    [HideInInspector]
    public int id;

    private void Start()
    {
        _frameManager = GameObject.FindObjectOfType<FrameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {    
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _frameManager.ChangeToFrame(GetComponent<Collider2D>());
        }
    }
}
