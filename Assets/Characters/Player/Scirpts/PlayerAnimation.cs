using System.Collections;
using UnityEngine;
using PlayerRays;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rb;

    private float _velocityX;
    private float _velocityY;
    private bool _isFirst = true;
    private bool _fliped = false;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Animate(PlayerMoveData inputData, PlayerMechanicsData mechanicsData)
    {
        _velocityX = _rb.velocity.x;
        _velocityY = _rb.velocity.y;

        FlipAnimation(inputData.isGrounded);
        GrabWallAnimation(mechanicsData.isWallGrabbed, inputData.isGrounded);
        JumpAnimation(inputData.isGrounded);
        GrabObjAnimation(mechanicsData);

        _animator.SetFloat("vertical_velocity", _velocityY);
        _animator.SetFloat("Speed", Mathf.Abs(_velocityX));
    }

    private void GrabWallAnimation(bool isWallGrabbed, bool isGrounded)
    {
        _animator.SetBool("isWallGrabbed", isWallGrabbed && !isGrounded);
    }
    private void GrabObjAnimation(PlayerMechanicsData mechanicsData)
    {
        if (mechanicsData.grabbableObject == null)
        {
            _animator.SetLayerWeight(1, 0);
        }
        else
        {
            _animator.SetLayerWeight(1, 1);
        }
    }
    private void JumpAnimation(bool isGrounded)
    {
        //jump animaiton
        if (isGrounded)
        {
            if (!_isFirst && Mathf.Abs(_rb.velocity.y) < 3f)
            {
                _animator.SetBool("isJumping", false);
                _isFirst = true;
            }
        }
        else
        {
            if (_isFirst)
            {
                _animator.SetBool("isJumping", true);
                _isFirst = false;
            }
        }
    }

    private void FlipAnimation(bool isGrounded)
    {
        if (_velocityX < -0.5f && !_fliped)
        {
            if (isGrounded)
            {
                StartCoroutine(AnimatedFlip(isGrounded));
            }
            else
            {
                Flip();
            }
            _fliped = true;
        }
        else if (_velocityX > 0.5f && _fliped)
        {
            if (isGrounded)
            {
                StartCoroutine(AnimatedFlip(isGrounded));
            }
            else
            {
                Flip();
            }
            _fliped = false;
        }
    }

    private IEnumerator AnimatedFlip(bool isGrounded)
    {
        _animator.SetBool("isFlipTransition", true);
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f);
        _animator.SetBool("isFlipTransition", false);
        Flip();
    }

    private void Flip()
    {
        transform.Rotate(Vector2.up * 180);
    }
}