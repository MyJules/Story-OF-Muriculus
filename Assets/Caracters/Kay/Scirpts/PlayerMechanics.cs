using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMechanics : IDie
{
    public override void Die()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
