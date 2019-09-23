using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using UnityEditor;
using UnityEngine;

public class MovablePlatform : MonoBehaviour
{
    [Tooltip("Platform will apear at first point")]
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
    
    private int _i;

    private bool _isReturning = false;
    

    void FixedUpdate()
    {
        _nextPoint = CalculateNextPoint(points);
        
        GoToPoint(_nextPoint);
    }

    public void AddPoint()
    {
        Vector2 pointPosition;

        GameObject newPoint;

        newPoint = examplePoint;
        
        if (points.Count > 0)
        {
            pointPosition = points.ElementAt(points.Count - 1).transform.position;

            newPoint = Instantiate(newPoint, pointPosition + Random.insideUnitCircle * 6,
                Quaternion.identity, this.transform);
        }
        else
        {
            pointPosition = transform.position;
            
            newPoint = Instantiate(newPoint, pointPosition + Random.insideUnitCircle * 6,
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

    private void GoToPoint(Transform goToPosition)
    {
        platformObject.transform.position = Vector2.MoveTowards(platformObject.transform.position, goToPosition.transform.position, speed * Time.deltaTime); 
    }
    
    private Transform CalculateNextPoint(List<GameObject> points)
    {
        Transform nextPoint;
            
        if (Vector2.Distance(platformObject.position, points[_i].transform.position) < 0.1f)
        {
            if (_isReturning)
            {
                _i--;

                if (_i < 0)
                {
                    _i = 1;
                    _isReturning = false;
                }
            }
            else
            {
                _i++;

                if (_i >= points.Count)
                {
                    if (isLooped)
                    {
                        _i = 0;
                        _isReturning = false;
                    }
                    else
                    {
                        _i--;
                        _isReturning = true;
                    }
                }
            }
        }
        nextPoint = points[_i].transform;
        
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
