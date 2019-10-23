using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using System;

public class NPCDialogueTriger : MonoBehaviour
{
    [SerializeField] private string DialoguePath;
    [SerializeField] private int id;

    [NonSerialized]
    public Dialogue dialogues;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogues);
    }

    private void Start()
    {
        TextAsset jFile =  (TextAsset) Resources.Load(@DialoguePath);
        
        JObject dialogue = (JObject) JsonConvert.DeserializeObject(jFile.ToString());
        
        dialogues = new Dialogue();
        
        foreach (var dialogElem in dialogue["dialogues"].Children())
        {
            if ((int)dialogElem.SelectToken("id") == id)
            {
                foreach (var content in dialogElem.SelectToken("content"))
                { 
                    dialogues.name.Add((string)content.SelectToken("name"));
                    dialogues.sentecnes.Add((string)content.SelectToken("sentence"));
                }
                
            }
        }
        
    }
}
