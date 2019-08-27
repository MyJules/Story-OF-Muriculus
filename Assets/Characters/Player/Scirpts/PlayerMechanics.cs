using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMechanics : IDie
{

    public override void Die()
    {
<<<<<<< Updated upstream
        Application.LoadLevel(Application.loadedLevel);
=======
        _anim.SetTrigger("Death");
		Load();
        //Application.LoadLevel(Application.loadedLevel);
>>>>>>> Stashed changes
    }
}
