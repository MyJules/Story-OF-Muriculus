using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[DefaultExecutionOrder(-100)]
public class PlayerInput : MonoBehaviour
{

    [SerializeField]
    private string jumpButton = "Jump";

    [SerializeField]
    private string horizontalButton = "Horizontal";

    private PlayerController _controller;

    private bool _isJumping;
    
    private bool _isJumpDown, _isJumpUp;

    private float _hMove;

    private void Start()
    {
        _controller = GetComponent<PlayerController>();
    }

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        _controller.Move(_hMove, _isJumping);
    }
    
    private void GetInput()
    {
        _hMove = CrossPlatformInputManager.GetAxis(horizontalButton);
        
        _isJumpDown = CrossPlatformInputManager.GetButtonDown(jumpButton);
        _isJumpUp = CrossPlatformInputManager.GetButtonUp(jumpButton);

        CalculateInput();
    }
    
    private void CalculateInput()
    {
        if (_isJumpDown)
        {
            _isJumping = true;
        }

        if (_isJumpUp)
        {
            _isJumping = false;
        }

    }
}