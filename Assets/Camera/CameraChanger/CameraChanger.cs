using UnityEngine;
using Cinemachine;

public class CameraChanger : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _changeToCamera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _changeToCamera.Priority = 100;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        _changeToCamera.Priority = -100;
    }
}
