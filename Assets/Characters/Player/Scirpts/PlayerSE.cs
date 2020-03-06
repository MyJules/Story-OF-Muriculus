using UnityEngine;
using PlayerRays;

public class PlayerSE : MonoBehaviour
{

    [SerializeField]
    private AudioSource audioSorce;

    [SerializeField]
    private ParticleSystem _playerParticleSystem;

    private GroundSpecialEffects groundCrossInfo;


    private Rays _rays;

    private bool isGroundCrossed;

    private void Start()
    {
        _rays = GetComponentInParent<Rays>();
    }

    public void PlayGroundParticle()
    {
        _playerParticleSystem.Play();
    }

    public void PlayStepSound()
    {
        isGroundCrossed = _rays.IsCrossed((int) PlayerRaysEnum.GroundParticleCheck);

        if (isGroundCrossed)
        {
            groundCrossInfo = _rays.GetCrossInformaiton((int)PlayerRaysEnum.GroundParticleCheck).collider.GetComponent<GroundSpecialEffects>();

            if (groundCrossInfo == true &&  groundCrossInfo.stepSound != null)
            {
                audioSorce.volume = 1;
                audioSorce.pitch = Random.Range(0.7f, 1.2f);
                audioSorce.PlayOneShot(groundCrossInfo.stepSound);
                //audioSorce.pitch = 1;
            }
        }
    }

    public void PlayLandSound()
    {
        isGroundCrossed = _rays.IsCrossed((int)PlayerRaysEnum.GroundParticleCheck);

        if (isGroundCrossed)
        {
            groundCrossInfo = _rays.GetCrossInformaiton((int)PlayerRaysEnum.GroundParticleCheck).collider.GetComponent<GroundSpecialEffects>();

            if (groundCrossInfo == true && groundCrossInfo.landSound != null)
            {
                audioSorce.volume = 0.6f;
                audioSorce.pitch = Random.Range(0.7f, 1.2f);
                audioSorce.PlayOneShot(groundCrossInfo.landSound);
            }
        }
    }

    public void PlayJumpSound()
    {
        isGroundCrossed = _rays.IsCrossed((int)PlayerRaysEnum.GroundParticleCheck);

        if (isGroundCrossed)
        {
            groundCrossInfo = _rays.GetCrossInformaiton((int)PlayerRaysEnum.GroundParticleCheck).collider.GetComponent<GroundSpecialEffects>();

            if (groundCrossInfo == true && groundCrossInfo.jumpSound != null)
            {
                audioSorce.volume = 1;
                audioSorce.pitch = 0.9f;
                audioSorce.PlayOneShot(groundCrossInfo.jumpSound);
            }
        }
    }
}
