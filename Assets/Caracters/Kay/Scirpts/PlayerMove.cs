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

    private Rigidbody2D _rb;

    private Ground _groundDetection;

    private float _rbGravity, _coyoteTime = 0.5f;

    private float _moveX, _moveY;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _groundDetection = GetComponent<Ground>();
        _rbGravity = _rb.gravityScale;
    }

    private void FixedUpdate()
    {
        CalculateMovement();
        _rb.velocity = new Vector2(_moveX * _speed, _moveY);
    }

    private void CalculateMovement()
    {
        //get input.
        _moveX = Input.GetAxis("Horizontal");
        _moveY = _rb.velocity.y;

        //jumping
        if (Input.GetButton("Jump") && _groundDetection.IsGrounded())
        {
            _moveY = _jumpHeight;
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

}