using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    public GameObject _dialogueUI;

    private Animator _dialogueAnimator;

    private Queue<string> _sentences;


    void Start()
    {
        _sentences = new Queue<string>();

        _dialogueUI.SetActive(false);

        _dialogueAnimator = _dialogueUI.GetComponentInChildren<Animator>();

    }

    public void StartDialogue(Dialogue dialogue)
    {

        _dialogueUI.SetActive(true);

        nameText.text = dialogue.name;

        _sentences.Clear();

        foreach (string sentence in dialogue.sentecnes)
        {
            _sentences.Enqueue(sentence);
        }

        DisplayNextSentence();

    }

    public void DisplayNextSentence()
    {
        if (_sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence =_sentences.Dequeue();

        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

    }

    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }


    public void EndDialogue()
    {
        if (_dialogueUI.active)
        { 
            _dialogueAnimator.SetTrigger("Disapear");

            StartCoroutine(DeactivateWithTime(_dialogueAnimator.GetCurrentAnimatorStateInfo(0).length, _dialogueUI));
            //_dialogueUI.SetActive(false);
        }
    }

    private IEnumerator DeactivateWithTime(float duration, GameObject deactivateObject)
    {
        deactivateObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        deactivateObject.SetActive(false);
    }
}
