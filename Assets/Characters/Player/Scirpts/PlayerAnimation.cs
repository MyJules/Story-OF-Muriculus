using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;

    private Rigidbody2D _rigidbody;

    private Rays _crossDetection;

    private float _moveX, _moveY;

    private bool _isFirst = true, _fliped = false, isAnimFinished = false;

    private bool isGrouded, isWallGrabbed;

    private Vector3 _localScale;

    [SerializeField]
    private float allowFlipTimerMax = 0.1f;

    private float _allowflipTimer;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();

        _rigidbody = GetComponent<Rigidbody2D>();

        _crossDetection = GetComponent<Rays>();
    }

    // Update is called once per frame
    void Update()
    {
        _moveX = _rigidbody.velocity.x;
        _moveY = _rigidbody.velocity.y;

        isGrouded = _crossDetection.IsCrossed(1, 2);
        isWallGrabbed = _crossDetection.IsCrossed(3);

        _localScale = transform.localScale;

        //reseting flip Timer
        if (isGrouded || isWallGrabbed)
        {
            _allowflipTimer = allowFlipTimerMax;
        }
        else
        {
            _allowflipTimer -= Time.deltaTime;
        }

        //flip just when on ground or on wall
        if (_allowflipTimer >= 0)
        {
               if (_moveX < -0.1 && !_fliped)
               {
                   Flip(true);
                   _fliped = true;
               }
               else if (_moveX > 0.1 && _fliped)
               {
                   Flip(true);
                   _fliped = false;
               }

        }



        transform.localScale = _localScale;

        //grab the wall
        if (isWallGrabbed && !isGrouded)
        {
            _animator.SetBool("isWallGrabbed", true);
        }
        else
            _animator.SetBool("isWallGrabbed", false);


        //jump animaiton
        if (!isGrouded)
        {
            if (_isFirst)
                _animator.SetTrigger("takeOff");

            _isFirst = false;
        }
        else
        {
            if (_moveY < 0.2 && _moveY > -0.2)
            {
                if (!_isFirst)
                    _animator.SetTrigger("landing");

                _isFirst = true;
            }
        }

        //setting up jump animation.
        _animator.SetFloat("vertical_velocity", _moveY);

        //setting horizontal animation
        _animator.SetFloat("Speed", Mathf.Abs(_moveX));
    }

    private void Flip(bool isflip)
    {
        if(isflip)
        _localScale.x *= -1;
    }
}