using System.Collections;
using System.Collections.Generic;
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
    private Transform[] points;

    private Transform _nextPoint;

    private int _i;

    private bool _isReturning = false;
   
    // Start is called before the first frame update
    void Awake()
    {
        _i = 0;
        _nextPoint = points[0];

        platformObject.transform.position = points[0].transform.position; 
    }

    void FixedUpdate()
    {

        if (platformObject.position == points[_i].position)
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

                if (_i >= points.Length)
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

            _nextPoint = points[_i];
        }

        
        GoToPoint(_nextPoint);
    }

    void GoToPoint(Transform goToPosition)
    {
        platformObject.transform.position = Vector2.MoveTowards(platformObject.transform.position, goToPosition.transform.position, speed * Time.deltaTime); 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        if (isLooped)
        {
            for (int i = 0; i < points.Length-1; i++)
            {
                Gizmos.DrawLine(points[i].position, points[i+1].position);
                Gizmos.DrawLine(points[points.Length - 1].position, points[0].position);
            }
        }
        else
        {
            for (int i = 0; i < points.Length-1; i++)
            {
                Gizmos.DrawLine(points[i].position, points[i + 1].position);
            }
        }
    }
}
