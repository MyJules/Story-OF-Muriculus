using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class NPColision : MonoBehaviour
{
    [SerializeField]
    private GameObject dialogueIndicator;

    [SerializeField]
    private Transform dialogueIndicatorPosition;

    private GameObject _diallogueIndicatorInstance;

    private NPCDialogueTriger _dialogueTriger;

    private DialogueManager _dialogueManager;

    private Animator _indicatorAnimator;

    private bool _isFirstTrigger = false;

    private bool _isStayInTrigger = false;

    private void Start()
    {
        _dialogueTriger = GetComponent<NPCDialogueTriger>();
        _dialogueManager = FindObjectOfType<DialogueManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            _diallogueIndicatorInstance = Instantiate(dialogueIndicator, dialogueIndicatorPosition.transform.position, dialogueIndicator.transform.rotation);
            _indicatorAnimator = _diallogueIndicatorInstance.GetComponent<Animator>();
            // _dialogueTriger.TriggerDialogue();
            _isFirstTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (_indicatorAnimator != null)
            {
                _indicatorAnimator.SetTrigger("Disapear");
                Destroy(_diallogueIndicatorInstance, _indicatorAnimator.GetCurrentAnimatorStateInfo(0).length);
            }

            _dialogueManager.EndDialogue();
            _isFirstTrigger = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
            _isStayInTrigger = true;
        else
            _isStayInTrigger = false;
    }

    //using because need to handle input
    private void Update()
    {
        if (_isStayInTrigger && CrossPlatformInputManager.GetButtonDown("NextDialogue") )
        {
            //disapear dialogue indicator and trigger dialogue one time.
            if (_isFirstTrigger)
            {
                _indicatorAnimator.SetTrigger("Disapear");
                Destroy(_diallogueIndicatorInstance, _indicatorAnimator.GetCurrentAnimatorStateInfo(0).length);

                _dialogueTriger.TriggerDialogue();
                _isFirstTrigger = false;
            }

            _dialogueManager.DisplayNextSentence();
        }
    }
}
