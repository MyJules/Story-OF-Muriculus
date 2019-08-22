using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyParticle : MonoBehaviour
{
    private ParticleSystem _thisParticle;

    private float _particleLifeTime;

    void Start()
    {
        _thisParticle = GetComponent<ParticleSystem>();

        _particleLifeTime = _thisParticle.duration + _thisParticle.startLifetime;

        Destroy(gameObject, _particleLifeTime);
    }
}
