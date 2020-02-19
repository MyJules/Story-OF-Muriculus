using System;
using UnityEngine;

[RequireComponent(typeof(Rays), typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Run")]
    [SerializeField]
    private float _maxSpeed = 100;
    
    [SerializeField]
    private  float _acceleration = 80;

    [SerializeField] private float _decceleration = 80;
    

    [Header("Jump")]
    [SerializeField]
    private float _jumpHeight = 2000;

    [SerializeField]
    private float _fallGravityScale = 20;

    [Header("Air Control")]

    [SerializeField]
    [Range(0, 3)]
    private float coyoteTimeMax = 0.1f;

    [SerializeField] private float _airAcceleration = 300;
    
    [SerializeField] private float _airDecceeleration = 200;
    
        [Space]

    [Header("Wall Jump Control")]

        [SerializeField]
    private float wallJumpHeight = 500f;

    [SerializeField] private float jumpOffForce = 30f;

    [SerializeField] private float maxFallSpeed = 10f;

    [SerializeField] private float wallDeccelerForce = 80f;

    [Space]

    private Rigidbody2D _rb;

    private Rays _crossDetection;

    private bool _isGrounded, _isWallGrabbed, _isFirstJump = true;

    private float _startGravityScale;

    private float _coyoteTime;

    private Vector2 _wallNormal;

    private bool _jumpWasRelesed = false, _allowJumping = false;

    private void Start()
    {
        _crossDetection = GetComponent<Rays>();
        _rb = GetComponent<Rigidbody2D>();

        _startGravityScale = _rb.gravityScale;
        
    }

    private void Update()
    {
        _isGrounded = _crossDetection.IsCrossed(1, 2);
        _isWallGrabbed = _crossDetection.IsCrossed(3);

        GetWallNormal();
        CalculateCoyoteTime();

    }

    public void Move(float horizontalInput, bool isJumping)
    {
        Running(horizontalInput, isJumping);
        Jumping(isJumping);
        WallJump(isJumping, _isWallGrabbed);
        AirControl(horizontalInput);
    }

    private void Running(float horizontalInput, bool isJumping)
    {
        if (_isGrounded)
        {
            if (Math.Abs(horizontalInput) > 0.01f)
            {
                // acceleration when X velocity is smaller then max speed.    
                if (Mathf.Abs(_rb.velocity.x) < _maxSpeed)
                {
                    //setting up constant speed if player reached max speed.
                    if ( Mathf.Abs(Mathf.Abs(_rb.velocity.x) - _maxSpeed) > 0.3f || Math.Abs(_rb.velocity.normalized.x - horizontalInput) > 0.3f)
                    {
                        _rb.AddForce(new Vector2(_acceleration * horizontalInput, 0));
                    }
                    else
                    {
                        _rb.velocity = new Vector2(horizontalInput * _maxSpeed, 0);
                    }
                }
                else
                {
                    _rb.AddForce(new Vector2(-_decceleration * _rb.velocity.normalized.x, 0));
                }
            }
            //deccelerate when input X is 0
            else
            {
                if (Mathf.Abs(_rb.velocity.x) > 3f)
                {
                    _rb.AddForce(new Vector2(-_decceleration * _rb.velocity.normalized.x, 0));
                }
                else
                {

                    _rb.velocity = Vector2.down;    
                }
            }
            
        }

    }

    private void Jumping(bool isJumping)
    {
        if (isJumping && _isFirstJump && _allowJumping)
        {
            if (_coyoteTime > 0)
            {
                Jump();
                _isFirstJump = false;
                _jumpWasRelesed = false;
            }
        }
        else //realising button when jumping
        if (!isJumping && _rb.velocity.y > 0.1f)
        {
            _rb.AddForce(Vector2.down * _rb.velocity.y * 5);
        }

        if (!isJumping)
        {
            _jumpWasRelesed = true;
        }

        //is jump button pressed again
        if (_jumpWasRelesed && isJumping)
        {
            _allowJumping = true;
        }
        else
        {
            _allowJumping = false;
        }
  
    }
    
    private void WallJump(bool isJumping, bool isWallGrabbed)
    {
        if (isWallGrabbed && !_isGrounded)
        {
            //velocity control
            
            //wall jump
            if (isJumping && _jumpWasRelesed)
            {
                _rb.velocity = Vector2.zero;
                _jumpWasRelesed = false;
                WallJump();
            }

            WallSlideControl();
        }

        if (!isJumping)
        {
            _jumpWasRelesed = true;
        }
    }

    private void WallSlideControl()
    {
        if (Mathf.Abs(_rb.velocity.y) > maxFallSpeed)
        {
            _rb.AddForce(new Vector2(0, -_rb.velocity.y).normalized * wallDeccelerForce);
        }
    }

    private void AirControl(float horizontalInput)
    {
        IncreaseGravityScale();
        AirMove(horizontalInput);
    }

    private void AirMove(float horizontalInput)
    {
        if (!_isGrounded && !_isWallGrabbed)
        {
            if (Math.Abs(horizontalInput) > 0.01f)
            {
                // acceleration when X velocity is smaller then max speed.    
                if (Mathf.Abs(_rb.velocity.x) < _maxSpeed)
                {
                    _rb.AddForce(new Vector2(_airAcceleration * horizontalInput, 0));
                }
                else
                {
                    _rb.AddForce(new Vector2(-_airAcceleration * _rb.velocity.normalized.x, 0));
                }
            }
            //deccelerate when input X is 0
            else
            {
                if (Mathf.Abs(_rb.velocity.x) > 3f)
                {
                    _rb.AddForce(new Vector2(-_airDecceeleration * _rb.velocity.normalized.x, 0));
                }

            }
        }
    }

    private void IncreaseGravityScale()
    {
        if (!_isGrounded && !_isWallGrabbed)
        {
            //increasing gravity scale when falling
            if (_rb.velocity.y < -1f)
            {
                _rb.gravityScale = _fallGravityScale;
            }
            else
            {
                _rb.gravityScale = _startGravityScale;
            }
        }
        else
            _rb.gravityScale = _startGravityScale;
    }

    private void Jump()
    {
        _rb.AddForce(Vector2.up * (_jumpHeight + Mathf.Abs(_rb.velocity.y)), ForceMode2D.Impulse);
    }
    
    private void WallJump()
    {
        _rb.AddForce(_wallNormal * jumpOffForce, ForceMode2D.Impulse);
        _rb.AddForce(Vector2.up * wallJumpHeight, ForceMode2D.Impulse);
    }
    
    private void CalculateCoyoteTime()
    {
        if (_isGrounded)
        {
            _coyoteTime = coyoteTimeMax;
            _isFirstJump = true;
        }
        else
        {
            _coyoteTime -= Time.deltaTime;
        }
    }

    private void GetWallNormal()
    {
        if (_isWallGrabbed)
        {
            _wallNormal = _crossDetection.GetCrossInformaiton(3).normal;
        }
    }
}
