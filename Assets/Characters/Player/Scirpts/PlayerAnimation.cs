using System.Collections;
using UnityEngine;
using PlayerRays;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rb;

    private float velocityX;
    private float velocityY;
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
        velocityX = _rb.velocity.x;
        velocityY = _rb.velocity.y;

        FlipAnimation(inputData.isGrounded);     
        GrabWallAnimation(mechanicsData.isWallGrabbed, inputData.isGrounded);
        JumpAnimation(inputData.isGrounded);

        _animator.SetFloat("vertical_velocity", velocityY);
        _animator.SetFloat("Speed", Mathf.Abs(velocityX));
    }

    private void GrabWallAnimation(bool isWallGrabbed, bool isGrounded)
    {
        _animator.SetBool("isWallGrabbed", isWallGrabbed && !isGrounded);
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
        _animator.SetBool("isPushing", isPushing);
    }

    private IEnumerator AnimatedFlip(bool isGrounded)
    {
        _animator.SetBool("isFlipTransition", true);
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f && isGrounded);
        _animator.SetBool("isFlipTransition", false);
        Flip();
    }

    private void Flip()
    {
        transform.Rotate(Vector2.up * 180);
    }
}