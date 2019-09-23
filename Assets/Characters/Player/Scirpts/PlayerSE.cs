using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSE : MonoBehaviour
{
    [SerializeField]
    private Transform groundParticlePosition;

    [SerializeField]
    private AudioSource audioSorce;

    private GroundSpecialEffects _groundSpecialEffects;

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
            _groundSpecialEffects = _rays.GetCrossInformaiton(0).collider.GetComponent<GroundSpecialEffects>();

            if (_groundSpecialEffects == true)
            {
                Instantiate(_groundSpecialEffects.landParticles, groundParticlePosition);
            }
        }
    }

    public void PlayStepSound()
    {
        isGroundWithParticle = _rays.IsCrossed(0);

        if (isGroundWithParticle)
        {
            _groundSpecialEffects = _rays.GetCrossInformaiton(0).collider.GetComponent<GroundSpecialEffects>();

            if (_groundSpecialEffects == true)
            {
                audioSorce.pitch = Random.Range(0.7f, 1.2f);
                audioSorce.PlayOneShot(_groundSpecialEffects.stepSound);
                //audioSorce.pitch = 1;
            }
        }
    }

    public void PlayLandSound()
    {
        isGroundWithParticle = _rays.IsCrossed(0);

        if (isGroundWithParticle)
        {
            _groundSpecialEffects = _rays.GetCrossInformaiton(0).collider.GetComponent<GroundSpecialEffects>();

            if (_groundSpecialEffects == true)
            {
                audioSorce.pitch = Random.Range(0.7f, 1.4f);
                audioSorce.PlayOneShot(_groundSpecialEffects.landSound);
            }
        }
    }

    public void PlayJumpSound()
    {
        isGroundWithParticle = _rays.IsCrossed(0);

        if (isGroundWithParticle)
        {
            _groundSpecialEffects = _rays.GetCrossInformaiton(0).collider.GetComponent<GroundSpecialEffects>();

            if (_groundSpecialEffects == true)
            {
                audioSorce.pitch = 0.9f;
                audioSorce.PlayOneShot(_groundSpecialEffects.jumpSound);
            }
        }
    }
}
