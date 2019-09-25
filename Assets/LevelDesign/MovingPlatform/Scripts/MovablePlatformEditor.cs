#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace Assets.Ground.MovingPlatform.Scripts
{
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
}
#endif