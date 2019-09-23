using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

[CustomEditor(typeof(MovablePlatform))]
public class MovablePlatformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();


        MovablePlatform movablePlatform = (MovablePlatform) target;

        if (GUILayout.Button("Add Point"))
        {
            movablePlatform.AddPoint();
        }
        
        if (GUILayout.Button("Remove Point"))
        {
            movablePlatform.RemovePoint();
        }
    }
}
