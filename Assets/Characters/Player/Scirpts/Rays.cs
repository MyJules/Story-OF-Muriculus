using UnityEngine;

public class Rays : MonoBehaviour
{
    [SerializeField]
    private RaysInfo[] _rayInfo;

    public bool IsCrossed(int fromRay, int toRay)
    {
        for (int i = fromRay; i <= toRay; i++)
        {
            RaycastHit2D raycast = Physics2D.Raycast(_rayInfo[i].rayTransform.position,
                Quaternion.Euler(_rayInfo[i].rayTransform.rotation.eulerAngles) * Vector3.forward,
                 _rayInfo[i].rayLength,
                 _rayInfo[i].rayCrossLayer);
            if (raycast == true)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsCrossed(int i)
    {
            RaycastHit2D raycast = Physics2D.Raycast(_rayInfo[i].rayTransform.position,
                Quaternion.Euler(_rayInfo[i].rayTransform.rotation.eulerAngles) * Vector3.forward, _rayInfo[i].rayLength,
                _rayInfo[i].rayCrossLayer);

            if (raycast == true)
            {
                return true;
            }
        return false;
    }
    public RaycastHit2D GetCrossInformation(int i)
    {
        return Physics2D.Raycast(_rayInfo[i].rayTransform.position,
            Quaternion.Euler(_rayInfo[i].rayTransform.rotation.eulerAngles) * Vector3.forward,
            _rayInfo[i].rayLength, _rayInfo[i].rayCrossLayer);
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < _rayInfo.Length; i++)
        {
            Debug.DrawRay(_rayInfo[i].rayTransform.position,
                Quaternion.Euler(_rayInfo[i].rayTransform.rotation.eulerAngles) * Vector3.forward * _rayInfo[i].rayLength,
                color: Color.red);
        }
    }
}
