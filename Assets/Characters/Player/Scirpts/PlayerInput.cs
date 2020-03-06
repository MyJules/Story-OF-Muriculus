using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using PlayerRays;

[DefaultExecutionOrder(-100)]
public class PlayerInput : MonoBehaviour
{

    [SerializeField]
    private string jumpButton = "Jump";

    [SerializeField]
    private string horizontalButton = "Horizontal";

    private PlayerController _controller;

    private PlayerAnimation _animation;

    private PlayerInputData _inputData;

    private Rays _rays;

    private bool _isJumpDown, _isJumpUp;

    private void Start()
    {
        _controller = GetComponent<PlayerController>();
        _rays = GetComponent<Rays>();
        _animation = GetComponent<PlayerAnimation>();

        _inputData = new PlayerInputData();
    }

    private void Update()
    {
        _inputData.isWallGrabbed = _rays.IsCrossed((int) PlayerRaysEnum.IsWallJumpCollide);
        _inputData.isGrounded = _rays.IsCrossed((int) PlayerRaysEnum.IsLeftLegGrounded, (int) PlayerRaysEnum.IsRightLegGrounded);

        setMovement();
    }

    private void FixedUpdate()
    {
        _controller.Move(_inputData);
        _animation.Animate(_inputData);
    }
    
    private void setMovement()
    {
        _inputData.horizontalInput = CrossPlatformInputManager.GetAxis(horizontalButton);
        
        _isJumpDown = CrossPlatformInputManager.GetButtonDown(jumpButton);
        _isJumpUp = CrossPlatformInputManager.GetButtonUp(jumpButton);

        setJumping();
    }
    
    private void setJumping()
    {
        if (_isJumpDown)
        {
            _inputData.isJumping = true;
        }

        if (_isJumpUp)
        {
            _inputData.isJumping = false;
        }

        if (_inputData.isWallGrabbed)
        {
            _inputData.wallNormal = _rays.GetCrossInformaiton((int) PlayerRaysEnum.IsWallJumpCollide).normal.normalized;
        }

    }
}