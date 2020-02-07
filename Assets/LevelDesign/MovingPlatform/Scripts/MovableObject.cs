using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    [SerializeField]
    private Transform platformObject;

    [SerializeField]
    private float speed = 4;

    [Space]

    [SerializeField]
    private bool isLooped;

    [Space]

    [SerializeField]

    private Color gizmoColor;

    [Space]

    [SerializeField]
    private GameObject examplePoint;
    
    [SerializeField]
    private List<GameObject> points;

    private Transform _nextPoint;
    
    private int _currentPoint;

    private bool _isReturning = false;

    public void AddPoint()
    {
        Vector2 pointPosition;

        GameObject newPoint;

        newPoint = examplePoint;
        
        if (points.Count > 0)
        {
            pointPosition = points.ElementAt(points.Count - 1).transform.position;

            newPoint = Instantiate(newPoint, pointPosition + Random.insideUnitCircle * 20,
                Quaternion.identity, this.transform);
        }
        else
        {
            pointPosition = transform.position;
            
            newPoint = Instantiate(newPoint, pointPosition + Random.insideUnitCircle * 20,
                Quaternion.identity, this.transform);
        }
        newPoint.SetActive(true);

        points.Add(newPoint);
    }

    public void RemovePoint()
    {
        if (points.Count > 0)
        {
            GameObject removePoint = points.ElementAt(points.Count - 1);

            points.Remove(removePoint);
            DestroyImmediate(removePoint);
        }
    }

    public void MoveTowardsPoint(Transform goToPosition)
    {
        platformObject.transform.position = Vector2.MoveTowards(platformObject.transform.position, goToPosition.transform.position, speed * Time.deltaTime); 
    }

    public void ChangeDirection()
    {
        _isReturning = !_isReturning;
    }

    public Transform GetPoint(int i)
    {
        return points[i].transform;
    }

    public Transform CalculateNextPoint()
    {
        Transform nextPoint;
            
        if (Vector2.Distance(platformObject.position, points[_currentPoint].transform.position) < 0.1f)
        {
            if (_isReturning)
            {
                _currentPoint--;

                if (_currentPoint < 0)
                {
                    _currentPoint = 1;
                    _isReturning = false;
                }
            }
            else
            {
                _currentPoint++;

                if (_currentPoint >= points.Count)
                {
                    if (isLooped)
                    {
                        _currentPoint = 0;
                        _isReturning = false;
                    }
                    else
                    {
                        _currentPoint--;
                        _isReturning = true;
                    }
                }
            }
        }
        nextPoint = points[_currentPoint].transform;
        
        return nextPoint;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        if (isLooped)
        {
            for (int i = 0; i < points.Count-1; i++)
            {
                Gizmos.DrawLine(points[i].transform.position, points[i+1].transform.position);
                Gizmos.DrawLine(points[points.Count - 1].transform.position, points[0].transform.position);
            }
        }
        else
        {
            for (int i = 0; i < points.Count-1; i++)
            {
                Gizmos.DrawLine(points[i].transform.position, points[i + 1].transform.position);
            }
        }
    }
}
