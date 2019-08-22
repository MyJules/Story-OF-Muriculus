using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class NPColision : MonoBehaviour
{
    private NPCDialogueTriger _dialogueTriger;

    private DialogueManager _dialogueManager;

    private void Start()
    {
        _dialogueTriger = GetComponent<NPCDialogueTriger>();
        _dialogueManager = FindObjectOfType<DialogueManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //9 - player layer
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _dialogueTriger.TriggerDialogue();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _dialogueManager.EndDialogue();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (CrossPlatformInputManager.GetButtonUp("NextDialogue") && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _dialogueManager.DisplayNextSentence();
        }
    }
}
