using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathMechanic : MonoBehaviour, IDie, IMemory
{
    private PlayerInfo _playerInfo;
    public void Die()
    {
        Load();
    }

    public void Load()
    {
         //Application.LoadLevel(Application.loadedLevel);

        _playerInfo = PlayerPersistance.LoadInfo();

        transform.position = _playerInfo.position;
    
    }

    public void Save()
    {
        PlayerPersistance.SaveInfo(this);
    }
}
