using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMechanics : MonoBehaviour, IDie, IMemory
{
    private PlayerInfo _playerInfo;

    //just for test. delete this!!!
    private void OnEnable()
    {
        Load();
    }//---

    public void Die()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void Load()
    {
        _playerInfo = PlayerPersistance.LoadInfo();

        transform.position = _playerInfo.position;
    }

    public void Save()
    {
        PlayerPersistance.SaveInfo(this);
    }
}
