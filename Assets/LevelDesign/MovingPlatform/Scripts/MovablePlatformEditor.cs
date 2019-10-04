#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Assets.Ground.MovingPlatform.Scripts
{
    [CustomEditor(typeof(MovableObject))]
    public class MovablePlatformEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();


            MovableObject movableObject = (MovableObject) target;

            if (GUILayout.Button("Add Point"))
            {
                movableObject.AddPoint();
                
            }
        
            if (GUILayout.Button("Remove Point"))
            {
                movableObject.RemovePoint();
            }
        }
    }
}
#endif