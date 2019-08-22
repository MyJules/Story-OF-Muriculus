using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMechanics : MonoBehaviour, IDie, IMemory
{
    private PlayerInfo _playerInfo;

    private Animator _anim;

    private void Start()
    {
        _anim = GetComponentInChildren<Animator>();
    }

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
