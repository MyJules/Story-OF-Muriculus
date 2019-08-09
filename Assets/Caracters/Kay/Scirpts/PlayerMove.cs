using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DefaultExecutionOrder(-100)]
public class PlayerMove : MonoBehaviour
{
    [Header("Move")]
    [SerializeField]
    private float _speed = 25;

    [Header("Jump")]
    [SerializeField]
    private float _jumpHeight = 10;

    [SerializeField]
    private float _fallGravity = 5;

    [SerializeField]
    private float coyoteTimeMax;

    private Rigidbody2D _rb;

    private Ground _groundDetection;

    private float _rbGravity, _coyoteTime;

    private float _moveX, _moveY;

    private bool _isFirst;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _groundDetection = GetComponent<Ground>();

        _rbGravity = _rb.gravityScale;

        _coyoteTime = coyoteTimeMax;
    }


    private void Update()
    {
        CalculateMovement();
        _rb.velocity = new Vector2(_moveX * _speed, _moveY);
    }

    private void CalculateMovement()
    {
        //get input
        _moveX = Input.GetAxis("Horizontal");
        _moveY = _rb.velocity.y;

        bool isGrounded = _groundDetection.IsGrounded();

        //callculate coyote Time
        if (!isGrounded)
        {
            _coyoteTime -= Time.deltaTime;
        }
        else
        {
            _coyoteTime = coyoteTimeMax;
        }

        //jumping
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                Jump();
                _isFirst = true;
            }
            //coyote jump
            else if (_coyoteTime >= 0 && _moveY < 0)
            {
                Jump();
            }

            //relising button during jumping
        } else if (Input.GetButtonUp("Jump") && _moveY > 8 && _isFirst)
        {
            _moveY = _jumpHeight/2.5f;
            _isFirst = false;
        }

        //increase gravity if player is falling
        if (!_groundDetection.IsGrounded() && _moveY < 0)
        {
            _rb.gravityScale = _fallGravity;
        }
        else
        {
            _rb.gravityScale = _rbGravity;
        }
    }

    private void Jump()
    {
        _moveY = _jumpHeight;
    }

}