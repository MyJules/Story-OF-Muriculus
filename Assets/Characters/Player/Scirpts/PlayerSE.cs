using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSE : MonoBehaviour
{
    [SerializeField]
    private Transform particlePosition;

    [SerializeField]
    private AudioSource audioSorce;

    private SpecialEffects _specialEffects;

    private Rays _rays;

    private bool isGroundWithParticle;

    private void Start()
    {
        _rays = GetComponentInParent<Rays>();

    }

    public void SpawnGroundParticle()
    {
        isGroundWithParticle = _rays.IsCrossed(0);

        if (isGroundWithParticle)
        {
            _specialEffects = _rays.GetCrossInformaiton(0).collider.GetComponent<SpecialEffects>();

            if (_specialEffects == true)
            {
                Instantiate(_specialEffects.landParticles, particlePosition);
            }
        }
    }

    public void PlayStepSound()
    {
        isGroundWithParticle = _rays.IsCrossed(0);

        if (isGroundWithParticle)
        {
            _specialEffects = _rays.GetCrossInformaiton(0).collider.GetComponent<SpecialEffects>();

            if (_specialEffects == true)
            {
                audioSorce.pitch = Random.Range(0.7f, 1.2f);
                audioSorce.PlayOneShot(_specialEffects.stepSound);
                //audioSorce.pitch = 1;
            }
        }
    }

    public void PlayLendSound()
    {
        isGroundWithParticle = _rays.IsCrossed(0);

        if (isGroundWithParticle)
        {
            _specialEffects = _rays.GetCrossInformaiton(0).collider.GetComponent<SpecialEffects>();

            if (_specialEffects == true)
            {
                audioSorce.pitch = Random.Range(0.7f, 1.4f);
                audioSorce.PlayOneShot(_specialEffects.landSound);
            }
        }
    }

    public void PlayJumpSound()
    {
        isGroundWithParticle = _rays.IsCrossed(0);

        if (isGroundWithParticle)
        {
            _specialEffects = _rays.GetCrossInformaiton(0).collider.GetComponent<SpecialEffects>();

            if (_specialEffects == true)
            {
                audioSorce.pitch = 0.9f;
                audioSorce.PlayOneShot(_specialEffects.jumpSound);
            }
        }
    }
}
