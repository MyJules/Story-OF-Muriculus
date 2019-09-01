using UnityEngine;

[System.Serializable]
public struct RaysInfo
{
    public string rayName;

    public float rayLength;

    public Transform rayTransform;

    public LayerMask rayCrossLayer;
}
