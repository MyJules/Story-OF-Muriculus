using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Run")]
    [SerializeField]
    private float _maxSpeed = 100;
    [SerializeField]
    private float _acceleration = 80;
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

    private Rigidbody2D _rigidbody;
    private bool _isFirstJump = true;
    private bool _allowJumping = false;
    private bool _jumpWasRelesed = false;
    private float _startGravityScale;
    private float _currentCoyoteTime;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _startGravityScale = _rigidbody.gravityScale;
    }

    public void Move(PlayerMoveData moveData, PlayerMechanicsData mechanicsData)
    {
        Running(moveData.horizontalInput, moveData.isGrounded);
        Jumping(moveData.isJumping);
        WallJump(moveData.isJumping, mechanicsData.isWallGrabbed, moveData.isGrounded, mechanicsData.wallNormal);
        AirControl(moveData.horizontalInput, moveData.isGrounded, mechanicsData.isWallGrabbed);
        CalculateCoyoteTime(moveData.isGrounded, coyoteTimeMax);
    }
    private void Running(float horizontalInput, bool isGrounded)
    {
        if (isGrounded)
        {
            if (Math.Abs(horizontalInput) > 0.01f)
            {
                // acceleration when X velocity is smaller then max speed.    
                if (Mathf.Abs(_rigidbody.velocity.x) < _maxSpeed)
                {
                    //setting up constant speed if player reached max speed.
                    if (Mathf.Abs(Mathf.Abs(_rigidbody.velocity.x) - _maxSpeed) > 0.3f ||
                        Math.Abs(_rigidbody.velocity.normalized.x - horizontalInput) > 0.3f)
                    {
                        _rigidbody.AddForce(new Vector2(_acceleration * horizontalInput, 0));
                    }
                    else
                    {
                        _rigidbody.velocity = new Vector2(horizontalInput * _maxSpeed, 0);
                    }
                }
                else
                {
                    _rigidbody.AddForce(new Vector2(-_decceleration * _rigidbody.velocity.normalized.x, 0));
                }
            }
            //deccelerate when input X is 0
            else
            {
                if (Mathf.Abs(_rigidbody.velocity.x) > 3f)
                {
                    _rigidbody.AddForce(new Vector2(-_decceleration * _rigidbody.velocity.normalized.x, 0));
                }
                else if(Mathf.Abs(_rigidbody.velocity.y) < 3f)
                {
                    _rigidbody.velocity = Vector2.down;
                }
            }

        }

    }

    private void Jumping(bool isJumping)
    {
        if (isJumping && _isFirstJump && _allowJumping)
        {
            if (_currentCoyoteTime > 0 && Mathf.Abs(_rigidbody.velocity.y) < 1.5f)
            {
                Jump();
                _isFirstJump = false;
                _jumpWasRelesed = false;
            }
        }
        else //realising button when jumping
        if (!isJumping && _rigidbody.velocity.y > 0.1f)
        {
            _rigidbody.AddForce(Vector2.down * _rigidbody.velocity.y * 5);
        }

        if (!isJumping)
        {
            _jumpWasRelesed = true;
        }

        _allowJumping = _jumpWasRelesed && isJumping;
    }

    private void WallJump(bool isJumping, bool isWallGrabbed, bool isGrounded, Vector2 wallNormal)
    {
        if (isWallGrabbed && !isGrounded)
        {
            //velocity control       
            //wall jump
            if (isJumping && _jumpWasRelesed)
            {
                _rigidbody.velocity = Vector2.zero;
                _jumpWasRelesed = false;
                WallJump(wallNormal);
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
        if (Mathf.Abs(_rigidbody.velocity.y) > maxFallSpeed)
        {
            _rigidbody.AddForce(new Vector2(0, -_rigidbody.velocity.y).normalized * wallDeccelerForce);
        }
    }

    private void AirControl(float horizontalInput, bool isGrounded, bool isWallGrabbed)
    {
        IncreaseGravityScale(isGrounded, isWallGrabbed);
        AirMove(horizontalInput, isGrounded, isWallGrabbed);
    }

    private void AirMove(float horizontalInput, bool isGrounded, bool isWallGrabbed)
    {
        if (!isGrounded && !isWallGrabbed)
        {
            if (Math.Abs(horizontalInput) > 0.01f)
            {
                // acceleration when X velocity is smaller then max speed.    
                if (Mathf.Abs(_rigidbody.velocity.x) < _maxSpeed)
                {
                    _rigidbody.AddForce(new Vector2(_airAcceleration * horizontalInput, 0));
                }
                else
                {
                    _rigidbody.AddForce(new Vector2(-_airAcceleration * _rigidbody.velocity.normalized.x, 0));
                }
            }
            //deccelerate when input X is 0
            else
            {
                if (Mathf.Abs(_rigidbody.velocity.x) > 3f)
                {
                    _rigidbody.AddForce(new Vector2(-_airDecceeleration * _rigidbody.velocity.normalized.x, 0));
                }

            }
        }
    }

    private void IncreaseGravityScale(bool isGrounded, bool isWallGrabbed)
    {
        if (!isGrounded && !isWallGrabbed)
        {
            //increasing gravity scale when falling
            if (_rigidbody.velocity.y < -1f)
            {
                _rigidbody.gravityScale = _fallGravityScale;
            }
            else
            {
                _rigidbody.gravityScale = _startGravityScale;
            }
        }
        else
        {
            _rigidbody.gravityScale = _startGravityScale;
        }
    }

    private void Jump()
    {
        _rigidbody.AddForce(Vector2.up * (_jumpHeight + Mathf.Abs(_rigidbody.velocity.y)), ForceMode2D.Impulse);
    }

    private void WallJump(Vector2 wallNormal)
    {
        _rigidbody.AddForce(wallNormal * jumpOffForce, ForceMode2D.Impulse);
        _rigidbody.AddForce(Vector2.up * wallJumpHeight, ForceMode2D.Impulse);
    }

    private void CalculateCoyoteTime(bool isGrounded, float coyoteTimeMax)
    {
        if (isGrounded)
        {
            _currentCoyoteTime = coyoteTimeMax;
            _isFirstJump = true;
        }
        else
        {
            _currentCoyoteTime -= Time.deltaTime;
        }
    }
}