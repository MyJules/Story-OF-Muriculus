using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSE : MonoBehaviour
{
    [SerializeField]
    private Transform particlePosition;

    private SpecialEffects _specialEffects;

    private Rays _rays;

    private bool isFirst = true, isGroundWithParticle;

    private void Start()
    {
        _rays = GetComponent<Rays>();

    }

    void Update()
    {
        StartCoroutine("SpawnParticle");
    }

    IEnumerator SpawnParticle()
    {
        isGroundWithParticle = _rays.IsCrossedWith(0);

        if (isGroundWithParticle && isFirst)
        {
            _specialEffects = _rays.GetCrossInformaiton(0).collider.GetComponent<SpecialEffects>();

            if (_specialEffects == true)
            {
                Instantiate(_specialEffects.landParticles, particlePosition);
            }

            isFirst = false;
        }
        else if (!isGroundWithParticle)
        {
            isFirst = true;
        }

        yield return new WaitForSeconds(5f);
    }
}
