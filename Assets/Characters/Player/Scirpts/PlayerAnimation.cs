using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;

    private Rigidbody2D _rb;

    private Rays _crossDetection;

    private float _moveX, _moveY;

    private bool _isFirst = true, _fliped = false, isAnimFinished = false;

    private bool isGrouded, isWallGrabbed, isPushing;

    private Vector3 _localScale;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();

        _rb = GetComponent<Rigidbody2D>();

        _crossDetection = GetComponent<Rays>();
        _localScale = transform.localScale;
    }
    
    void Update()
    {
        _moveX = _rb.velocity.x;
        _moveY = _rb.velocity.y;

        isGrouded = _crossDetection.IsCrossed(1,2);
        isWallGrabbed = _crossDetection.IsCrossed(3);

        isPushing = _crossDetection.IsCrossed(4);

        if (isPushing)
        {
            _animator.SetBool("isPushing", true);
        }
        else
        {
            _animator.SetBool("isPushing", false);
        }

        //flip just when on ground or on wall
        if (_moveX < -4f && !_fliped)
        {
            if (isGrouded)
            {
                StartCoroutine(AnimatedFlip());
            }
            else
            {
                Flip();                    
            }

            _fliped = true;
        }
        else if (_moveX > 4f && _fliped)
        {
            if (isGrouded)
            {
                StartCoroutine(AnimatedFlip());
            }
            else
            {
                Flip();
            }
            _fliped = false;
        }


        //grab the wall
        if (isWallGrabbed && !isGrouded)
            _animator.SetBool("isWallGrabbed", true);
        else
            _animator.SetBool("isWallGrabbed", false);


        //jump animaiton
        if (isGrouded)
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

        //setting up jump animation.
        _animator.SetFloat("vertical_velocity", _moveY);

        //setting horizontal animation
        _animator.SetFloat("Speed", Mathf.Abs(_moveX));
    }

    private IEnumerator AnimatedFlip()
    {
        _animator.SetBool("isFlipTransition", true);
        yield return new WaitWhile(() => _animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1f && isGrouded);
        _animator.SetBool("isFlipTransition", false);
        Flip();
    }

    private void Flip()
    {
        transform.Rotate(Vector2.up * 180);
    }

}