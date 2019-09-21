using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Run")]
    [SerializeField]
    private float _maxSpeed = 100;
    
    [SerializeField]
    private  float _acceleration = 80;

    [SerializeField] private float _decceleration = 80;
    
    [SerializeField] private float _simpleDeccelerationDiv = 2;

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

    [SerializeField] private float _simpleAirDeccelerationDiv = 2;

        [Space]

    [Header("Wall Jump Control")]

        [SerializeField]
        [Range(0, 3)] private float wallJumpCoyoteTimeMax = 0.5f;
        
    [SerializeField]
    private float wallJumpHeight = 500f;

    [SerializeField] private float jumpOffForce = 30f;

    [Space]

    private Rigidbody2D _rb;

    private Rays _crossDetection;

    private bool _isGrounded, _isWallGrabbed, _isFirstJump = true, _isWallJump = true;

    private float _startGravityScale;

    private float _coyoteTime, _wallCoyoteTime;

    private Vector2 _wallNormal;

    private bool jumpWasRealised = false;

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
        CalculateWallCoyoteTime();
    }

    public void Move(float horizontalInput, bool isJumping)
    {
        Running(horizontalInput);
        Jumping(isJumping);
        WallJump(isJumping, _isWallGrabbed);
        AirControl(horizontalInput);
    }

    private void Running(float horizontalInput)
    {
        if(_isGrounded)
        {
            //setting up max speed + acceleration and decceleration
            if (Mathf.Abs(_rb.velocity.x) < _maxSpeed && horizontalInput != 0)
            {
                _rb.AddForce(new Vector2(horizontalInput * _acceleration, y: 0));
            }else if (_rb.velocity.x < 0 && horizontalInput == 1) 
            {    //deccelerate if we pushing the opposite direction
                _rb.AddForce(new Vector2(horizontalInput * _decceleration , y: 0));
            }else if (_rb.velocity.x > 0 && horizontalInput == -1)
            {    //deccelerate if we pushing the opposite direction
                _rb.AddForce(new Vector2(horizontalInput * _decceleration , y: 0));
            }else if (Mathf.Abs(_rb.velocity.x) > 0.2f && horizontalInput == 0)
            {
                //deccelerete if don't puch any button
                if (_rb.velocity.x < 0.2f)
                {
                    _rb.AddForce(new Vector2(_decceleration / _simpleDeccelerationDiv, 0));
                }
                else if (_rb.velocity.x > 0.2f)
                {
                    _rb.AddForce(new Vector2(-_decceleration / _simpleDeccelerationDiv, 0));
                }
            }
        }
        
        //setting velocity to 0 if x velocity is small
        if (Mathf.Abs(_rb.velocity.x) < 1f && horizontalInput == 0 && _isGrounded)
        {
            _rb.velocity = Vector2.zero;
        }
    }

    private void Jumping(bool isJumping)
    {
        //main
            if (isJumping && _isFirstJump && jumpWasRealised)
            {
                if (_coyoteTime > 0)
                {
                    Jump();
                    _isFirstJump = false;
                    jumpWasRealised = false;
                }
            }
            else //realising button when jumping
            if (!isJumping && _rb.velocity.y > 0.1f)
            {
                _rb.AddForce(Vector2.down * _rb.velocity.y * 5);
                   // jumpWasRealised = true;
            }

            if (!isJumping)
            {
                jumpWasRealised = true;
            }
    }
    
    private void WallJump(bool isJumping, bool isWallGrabbed)
    {
        if (isWallGrabbed || _wallCoyoteTime > 0)
        {
            if (isJumping && _isWallJump && jumpWasRealised)
            {
                _rb.velocity = Vector2.zero;
                jumpWasRealised = false;
                _isWallJump = false;
                WallJump();
            }
        }

        if (!isJumping)
        {
            jumpWasRealised = true;
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
            //setting up max speed + acceleration and decceleration
            if (Mathf.Abs(_rb.velocity.x) < _maxSpeed && horizontalInput != 0)
            {
                _rb.AddForce(new Vector2(horizontalInput * _airAcceleration, y: 0));
            }else if (_rb.velocity.x < 0 && horizontalInput == 1) 
            {    //deccelerate if we pushing the opposite direction
                _rb.AddForce(new Vector2(horizontalInput * _airDecceeleration , y: 0));
            }else if (_rb.velocity.x > 0 && horizontalInput == -1)
            {    //deccelerate if we pushing the opposite direction
                _rb.AddForce(new Vector2(horizontalInput * _airDecceeleration , y: 0));
            }else if (Mathf.Abs(_rb.velocity.x) > 0.2f && horizontalInput == 0)
            {
                //deccelerete if don't puch any button
                if (_rb.velocity.x < 0.2f)
                {
                    _rb.AddForce(new Vector2(_airDecceeleration / _simpleAirDeccelerationDiv, 0));
                }
                else if (_rb.velocity.x > 0.2f)
                {
                    _rb.AddForce(new Vector2(-_airDecceeleration / _simpleAirDeccelerationDiv, 0));
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
        _rb.AddForce(Vector2.up * _jumpHeight);
    }
    
    private void WallJump()
    {
        _rb.AddForce(_wallNormal * jumpOffForce);
        _rb.AddForce(Vector2.up * wallJumpHeight);
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
    
    private void CalculateWallCoyoteTime()
    {
        if (_isWallGrabbed)
        {
            _isWallJump = true;
            _wallCoyoteTime = wallJumpCoyoteTimeMax;
        }
        else if(!_isGrounded)
        {
            _wallCoyoteTime -= Time.deltaTime;
        }
    }
    
    private void GetWallNormal()
    {
        if (_isWallGrabbed && _crossDetection.IsCrossed(3))
        {
            _wallNormal = _crossDetection.GetCrossInformaiton(3).normal;
        }
    }
}
