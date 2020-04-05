using UnityEngine;
using PlayerRays;

public class PlayerSE : MonoBehaviour
{
    [Header("Default Sounds")]
    [SerializeField]
    private AudioSource _audioSorce;
    [SerializeField]
    private ParticleSystem _playerDust;
    [SerializeField]
    private AudioClip _stepSound;
    [SerializeField]
    private AudioClip _landSound;
    [SerializeField]
    private AudioClip _jumpSound;

    private GroundSpecialEffects groundCrossInfo;
    private Rays _rays;
    private void Start()
    {
        _rays = GetComponentInParent<Rays>();
    }

    public void PlayGroundParticle()
    {
        _playerDust.Play();
    }

    public void PlayStepSound()
    {
        _audioSorce.volume = 2f;
        _audioSorce.pitch = Random.Range(0.7f, 1.2f);
        _audioSorce.PlayOneShot(_stepSound);
    }

    public void PlayLandSound()
    {
        _audioSorce.volume = 1f;
        _audioSorce.pitch = Random.Range(0.7f, 1.2f);
        _audioSorce.PlayOneShot(_landSound);
    }

    public void PlayJumpSound()
    {
        _audioSorce.volume = 1;
        _audioSorce.pitch = 0.9f;
        _audioSorce.PlayOneShot(_jumpSound);
    }
}
