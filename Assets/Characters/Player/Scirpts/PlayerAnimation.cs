using System.Collections;
using UnityEngine;
using PlayerRays;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;

    private Rigidbody2D _rb;

    private float velocityX, velocityY;

    private bool _isFirst = true, _fliped = false, isAnimFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }
    
    public void Animate(PlayerMoveData inputData, PlayerMechanicsData mechanicsData)
    {
        velocityX = _rb.velocity.x;
        velocityY = _rb.velocity.y;

        PushAnimation(false);

        FlipAnimation(inputData.isGrounded);
        
        GrabWallAnimation(mechanicsData.isWallGrabbed, inputData.isGrounded);

        JumpAnimation(inputData.isGrounded);

        //setting up jump animation.
        _animator.SetFloat("vertical_velocity", velocityY);

        //setting horizontal animation
        _animator.SetFloat("Speed", Mathf.Abs(velocityX));
    }

    private void GrabWallAnimation(bool isWallGrabbed, bool isGrounded)
    {
        //grab the wall
        if (isWallGrabbed && !isGrounded)
            _animator.SetBool("isWallGrabbed", true);
        else
            _animator.SetBool("isWallGrabbed", false);
    }

    private void JumpAnimation(bool isGrounded)
    {
        //jump animaiton
        if (isGrounded)
        {
            if (!_isFirst)
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
        //flip just when on ground or on wall
        if (velocityX < -1f && !_fliped)
        {

                StartCoroutine(AnimatedFlip(isGrounded));

            _fliped = true;
        }
        else if (velocityX > 1f && _fliped)
        {
                StartCoroutine(AnimatedFlip(isGrounded));

            _fliped = false;    
        }
    }

    private void PushAnimation(bool isPushing)
    {

        if (isPushing)
        {   
            _animator.SetBool("isPushing", true);
        }
        else
        {
            _animator.SetBool("isPushing", false);
        }
    }

    private IEnumerator AnimatedFlip(bool isGrounded)
    {
        _animator.SetBool("isFlipTransition", true);
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f && isGrounded);
        _animator.SetBool("isFlipTransition", false);
        Flip();
    }

    private void Flip()
    {
        transform.Rotate(Vector2.up * 180);
    }
}