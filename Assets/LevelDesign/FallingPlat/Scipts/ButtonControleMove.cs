using System.Collections;
using UnityEngine;

public class ButtonControleMove : MonoBehaviour
{
    
    private MovableObject _movable;

    private Transform _nextPoint;
    
    [SerializeField]
    private float goDownDelay = 0.5f;
    
    void Start()
    {
        _movable = GetComponent<MovableObject>();
        _nextPoint = _movable.GetPoint(1);
    }
    
    public void GoUp()
    {
        _nextPoint = _movable.GetPoint(0);
    }

    public void GoDown()
    {
        StartCoroutine(WaitAndSetPoint(goDownDelay, 1));
    }
    
    void FixedUpdate()
    {
        _movable.MoveTowardsPoint(_nextPoint);
    }

    private IEnumerator WaitAndSetPoint(float t, int point)
    {
        yield return new WaitForSeconds(t);
        _nextPoint = _movable.GetPoint(point);
    }
}
