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
    [SerializeField]
    private string grabButton = "NextDialogue";

    private PlayerController _controller;
    private PlayerAnimation _animation;
    private PlayerMechanics _mechanics;

    private PlayerMoveData _moveData;
    private PlayerMechanicsData _mechanicsData;

    private Rays _rays;

    private bool _isJumpDown;
    private bool _isJumpUp;
    private bool _isGrabDown;

    private void Start()
    {
        _controller = GetComponent<PlayerController>();
        _animation = GetComponent<PlayerAnimation>();
        _mechanics = GetComponent<PlayerMechanics>();
        _rays = GetComponent<Rays>();

        _moveData = new PlayerMoveData();
        _mechanicsData = new PlayerMechanicsData();
    }

    private void Update()
    {
        _mechanicsData.isWallGrabbed = _rays.IsCrossed((int)PlayerRaysEnum.IsWallJumpCollide);
        _moveData.isGrounded = _rays.IsCrossed((int)PlayerRaysEnum.IsLeftLegGrounded, (int)PlayerRaysEnum.IsRightLegGrounded);
        _animation.Animate(_moveData, _mechanicsData);
        setMovement();
    }

    private void FixedUpdate()
    {
        _controller.Move(_moveData, _mechanicsData);
    }

    private void setMovement()
    {
        _moveData.horizontalInput = CrossPlatformInputManager.GetAxis(horizontalButton);
        _isJumpDown = CrossPlatformInputManager.GetButtonDown(jumpButton);
        _isJumpUp = CrossPlatformInputManager.GetButtonUp(jumpButton);
        _isGrabDown = CrossPlatformInputManager.GetButtonDown(grabButton);
        setJumping();
        setMechanics();
    }

    private void setJumping()
    {
        if (_isJumpDown)
        {
            _moveData.isJumping = true;
        }
        else
        if (_isJumpUp)
        {
            _moveData.isJumping = false;
        }
        if (_mechanicsData.isWallGrabbed && _isJumpDown)
        {
            _mechanicsData.wallNormal = _rays.GetCrossInformation((int)PlayerRaysEnum.IsWallJumpCollide).normal.normalized;
        }
    }

    private void setMechanics()
    {
        if (_isGrabDown)
        {
            _mechanicsData.isMovableObjGrabbed = _mechanicsData.isMovableObjGrabbed == false ||
                                                  _mechanicsData.grabbableObject == null;
            if (_mechanicsData.isMovableObjGrabbed == true)
            {
                _mechanicsData.grabbableObject =
                _rays.GetCrossInformation((int)PlayerRaysEnum.IsMovableObjeGrabbed).collider.gameObject.GetComponent<IGrabbable>();
            }
            _mechanics.Grabbing(_mechanicsData, ref _mechanicsData.grabbableObject);
        }
    }
}