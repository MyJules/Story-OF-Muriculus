using UnityEngine;

public class ConstantMove : MonoBehaviour
{
    private MovableObject _movable;

    private Transform _nextPoint;
    
    void Start()
    {
        _movable = GetComponent<MovableObject>();
    }

    void FixedUpdate()
    {
        _nextPoint = _movable.CalculateNextPoint();
        
        _movable.MoveTowardsPoint(_nextPoint);
    }
}
