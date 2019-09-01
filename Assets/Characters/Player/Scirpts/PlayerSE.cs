using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSE : MonoBehaviour
{
    [SerializeField]
    private Transform particlePosition;

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
}
