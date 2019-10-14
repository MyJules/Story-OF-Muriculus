using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{   
    public List<string> name;

    [TextArea(3,10)]
    public List<string> sentecnes;

    public Dialogue() 
    {
        name = new List<string>();
        sentecnes = new List<string>();
    }

}
