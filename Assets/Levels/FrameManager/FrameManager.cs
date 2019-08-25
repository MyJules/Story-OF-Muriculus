using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FrameManager : MonoBehaviour
{
    [SerializeField]
    private CinemachineConfiner frameConfiner;

    private GameObject[] _frames;

    private void Start()
    {
        _frames = GameObject.FindGameObjectsWithTag("Frame");

        //setting id for every frame.
        for (int i = 0; i < _frames.Length; i++)
        {
            FrameControl currentFrame = _frames[i].GetComponent<FrameControl>();

            currentFrame.id = i;
        }
    }
   
    public void ChangeToFrame(Collider2D collider)
    {
        frameConfiner.InvalidatePathCache();
        frameConfiner.m_BoundingShape2D = collider;



        //disabling all other frames.
        for (int i = 0; i < _frames.Length; i++)
        {
            FrameControl currentFrame = collider.GetComponent<FrameControl>();

            if (currentFrame.id == i)
            { 

                for (int j = 0; j < _frames[i].transform.childCount; j++)
                {
                    _frames[i].transform.GetChild(j).gameObject.active = true;
                }

            }
            else if(_frames[i].transform.GetChild(0).gameObject.active)
            {

                for (int j = 0; j < _frames[i].transform.childCount; j++)
                {
                         _frames[i].transform.GetChild(j).gameObject.active = false;
                }   
                
            }

        }

    }

}
