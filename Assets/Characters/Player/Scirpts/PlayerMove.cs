using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[DefaultExecutionOrder(-100)]
public class PlayerMove : MonoBehaviour
{
    [Header("Move")]
    [SerializeField]
    private float _speed = 25;

    [Header("Jump")]
    [SerializeField]
    private float _jumpHeight = 25;

    [SerializeField]
    private float _fallGravityScale = 5;

    [Header("Air Control")]

    [SerializeField]
    private float coyoteTimeMax = 0.1f;

    [SerializeField]
    private float verticalMultipl = 0.9f;

    [SerializeField]
    [Tooltip("Deccelerate when in air you change velocity X direction.")]
    private float verticalSwithMultipl = 0.8f;

    [Space]

    [Header("Wall Jump Control")]

    [SerializeField]
    private float wallJumpCoyoteTimeMax = 1f;

    [SerializeField]
    private float wallMaxVelocity = 30f;

    [SerializeField]
    private float wallJumpDelayTimerMax = 0.8f;


    [Space]

    private Rigidbody2D _rb;

    private Rays _crossDetection;

    private float _rbStartGravity, _coyoteTime, _wallCoyoteTime;

    private float _moveX, _moveY;

    private bool _isFirstV = true, _isFirstH = true, _isFirsWallJump = true, _isFirstCoyoteJump = true;

    private bool isGrounded, isWallGrabbed;

    float startVelocity = 0;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _crossDetection = GetComponent<Rays>();

        _rbStartGravity = _rb.gravityScale;

        _coyoteTime = coyoteTimeMax;
        _wallCoyoteTime = wallJumpCoyoteTimeMax;   

    }

    private void Update()
    {
        _moveX = CrossPlatformInputManager.GetAxis("Horizontal");
        _moveY = _rb.velocity.y;

        isGrounded = _crossDetection.IsCrossed(1, 2);
        isWallGrabbed = _crossDetection.IsCrossed(3);

        CalculateMovement();
        _rb.velocity = new Vector2(_moveX * _speed, _moveY);
    }

    private void CalculateMovement()
    {
        CalculateHorizontalMovement();
        CalculateVertialMovement();     
    }


    private void CalculateHorizontalMovement()
    {

        if (!isGrounded)
        {
            if(_wallCoyoteTime < 0)
            //decrease X velocity while jumping
            _moveX *= verticalMultipl;

            //get begin X velocity
            if (_isFirstH || isWallGrabbed)
            {
                startVelocity = _rb.velocity.x;
                _isFirstH = false;
            }

            //decrease X velocity if jump direction changeds
            if (startVelocity < 0)
            {
                if (_rb.velocity.x > 0)
                    _moveX *= verticalSwithMultipl;
            }
            else if (startVelocity > 0)
            {
                if (_rb.velocity.x < 0)
                    _moveX *= verticalSwithMultipl;
            }
            
        }
        else
        _isFirstH = true;
    }


    private void CalculateVertialMovement()
    {

        //callculate coyote Time
        if (!isGrounded)
        {
            _coyoteTime -= Time.deltaTime;

        }
        else
        {
            _coyoteTime = coyoteTimeMax;
        }

        //callculate wall coyote Time
        if (!isWallGrabbed)
        {
            _wallCoyoteTime -= Time.deltaTime;
        }
        else if(isWallGrabbed)
        {
            _wallCoyoteTime = wallJumpCoyoteTimeMax;

        }


        //jumping
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {

            //jumping
            if (isGrounded)
            {
                Jump();

                _isFirstV = true;

                _isFirstCoyoteJump = true;
            }
            
            //coyote jump
            if (_coyoteTime > 0 && _moveY < 0)
            {
                if (_isFirstCoyoteJump)
                    Jump();
                _isFirstCoyoteJump = false;
            }
            
            //wall Jump
            if (_wallCoyoteTime > 0)
            {
                if (_isFirsWallJump)
                {
                    Jump();
                    _isFirsWallJump = false;
                }
            }

            if(!isWallGrabbed)
                _isFirsWallJump = true;

            //releasing button during jumping
        }
        else if (CrossPlatformInputManager.GetButtonUp("Jump") && _moveY > 10 && _isFirstV)
        {
            _moveY = _jumpHeight / 2.5f;
            _isFirstV = false;
        }

        //increase gravity if player is falling
        if (!isGrounded && _moveY < 0 && !isWallGrabbed)
        {
            if (_isFirstV)
                _rb.gravityScale *= _fallGravityScale;

            _isFirstV = false;

        } 
        else
        {
            _rb.gravityScale = _rbStartGravity;
            _isFirstV = true;
        }

        //setring maximum fall velocity for wall
        if (isWallGrabbed)
        {
            if (_isFirstV)
                _moveY = Mathf.Max(_moveY, wallMaxVelocity);
        }

    }


    private void Jump()
    {
        _moveY = _jumpHeight;
    }


}