using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMechanics : MonoBehaviour, IDie, IMemory
{
    private PlayerInfo _playerInfo;
	private Animator _animator;
	private Rigidbody rigidbody;

	public void Die()
    {
		_animator = GetComponentInChildren<Animator>();
		_animator.SetFloat("Speed", 0);
		StartCoroutine(AnimatedDeath());
    } 

	private IEnumerator AnimatedDeath()
    {
		_animator = GetComponentInChildren<Animator>();
		_animator.SetTrigger("Death");
        yield return new WaitForSeconds(1);
		Load();
    }

    public void Load()
    {
		//StopAllCoroutines();
        _playerInfo = PlayerPersistance.LoadInfo();
        transform.position = _playerInfo.position;
		
    }

    public void Save()
    {
        PlayerPersistance.SaveInfo(this);
    }
}
    /*public void Die()
    {
		StartCoroutine(AnimatedDeath());
    }

	private IEnumerator AnimatedDeath()
    {
		float duration = 1.0f;
		_animator = GetComponentInChildren<Animator>();
		_animator.SetTrigger("Death");
		while (duration > 0)
		{
			duration -= Time.deltaTime;
			yield return null;
		}
        //yield return new WaitForSeconds(1);
        Load();
		//StopAllCoroutines();
		
        
    }
    public void Load()
    {
         //Application.LoadLevel(Application.loadedLevel);
        _playerInfo = PlayerPersistance.LoadInfo();
        transform.position = _playerInfo.position;
		//_animator.SetBool("isGrounded", false);
    }
    public void Save()
    {
        PlayerPersistance.SaveInfo(this);
    }
}*/
