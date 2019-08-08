using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator _animator;

    private Rigidbody2D _rigidbody;

    private Ground _groundDetection;

    private SpriteRenderer _spriteRender;

    private float _moveX, _moveY;

    private bool _isFirst = true, _fliped;

    private Vector3 _localScale;


    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();

        _rigidbody = GetComponent<Rigidbody2D>();

        _groundDetection = GetComponent<Ground>();

        _spriteRender = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _moveX = _rigidbody.velocity.x;
        _moveY = _rigidbody.velocity.y;

        _localScale = transform.localScale;

        // flip X
        if (_moveX < -0.1 && !_fliped)
        {
            _localScale.x *= -1;
            _fliped = true;
        }
        else if (_moveX > 0.1 && _fliped)
        {
            _localScale.x *= -1;
            _fliped = false;
        }

        transform.localScale = _localScale;

        //jump animaiton
        if (!_groundDetection.IsGrounded())
            {
                if (_isFirst)
                    _animator.SetTrigger("takeOff");

                _isFirst = false;
            }
            else if(_moveY < 0.2 && _moveY > -0.2)
            {
                if (!_isFirst)
                    _animator.SetTrigger("landing");

                _isFirst = true;
            }
        //setting up jump animation.
        _animator.SetFloat("vertical_velocity", _moveY);
        
        //setting horizontal animation
        _animator.SetFloat("Speed", Mathf.Abs(_moveX));
    }
}