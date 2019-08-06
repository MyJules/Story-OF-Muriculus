using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDialogueTriger : MonoBehaviour
{

    public Dialogue dialogue;

    private BoxCollider2D _boxColider;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

}
