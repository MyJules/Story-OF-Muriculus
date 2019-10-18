using UnityEngine;
using Cinemachine;

public class CameraChanger : MonoBehaviour
{
    [SerializeField]
        CinemachineVirtualCamera changeEnterCamera;

    [SerializeField]
        CinemachineVirtualCamera changeExitCamera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
        changeEnterCamera.Priority = 10;
        changeExitCamera.Priority = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
        changeEnterCamera.Priority = -10;
        changeExitCamera.Priority = 0;
        }
    }
}
