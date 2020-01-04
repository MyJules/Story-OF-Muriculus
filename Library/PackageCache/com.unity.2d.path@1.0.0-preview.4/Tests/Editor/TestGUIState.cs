using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.U2D.Path.GUIFramework;

namespace UnityEditor.U2D.Path.Tests
{
    public class TestGUIState : IGUIState
    {
        private readonly float kPickDistance = 5f;

        private int m_CurrentControlID = 0;
        private float m_NearestDistance = 0f;
        public Vector2 mousePosition { get; set; }
        public int mouseButton { get; set; }
        public int clickCount { get; set; }
        public bool isShiftDown { get; set; }
        public bool isAltDown { get; set; }
        public bool isActionKeyDown { get; set; }
        public KeyCode keyCode { get; set; }
        public EventType eventType { get; set; }
        public string commandName { get; set; }
        public int nearestControl { get; set; }
        public int hotControl { get; set; }
        public bool changed { get; set; }
        public bool repaintFlag { get; private set; }
        public bool usedEvent { get; private set; }
        public Vector3 sliderDelta;

        public void BeginEventPass()
        {
            m_CurrentControlID = 0;
            m_NearestDistance = float.MaxValue;
        }

        public int GetControlID(int hint, FocusType focusType)
        {
            return ++m_CurrentControlID;
        }

        public void AddControl(int controlID, float distance)
        {
            if (eventType == EventType.Layout)
            {
                if (distance <= m_NearestDistance && distance <= kPickDistance)
                {
                    m_NearestDistance = distance;
                    nearestControl = controlID;
                }
            }
        }

        public bool Slider(int id, SliderData sliderData, out Vector3 newPosition)
        {
            newPosition = sliderData.position;

            if (nearestControl == id && mouseButton == 0 && eventType == EventType.MouseDown)
            {
                hotControl = id;
            }

            if (hotControl == id && mouseButton == 0 && eventType == EventType.MouseUp)
            {
                hotControl = 0;
            }

            if (hotControl == id && eventType == EventType.MouseDrag)
            {
                newPosition += sliderDelta;
                return true;
            }

            return false;
        }

        public void UseEvent() { usedEvent = true; }
        public void Repaint() { repaintFlag = true; }
        public bool IsEventOutsideWindow() { return false; }
        public bool IsViewToolActive() { return false; }
        public bool HasCurrentCamera() { return false; }
        public float GetHandleSize(Vector3 position) { return 0.005f; }
        public float DistanceToSegment(Vector3 p1, Vector3 p2)
        {
            return UnityEditor.HandleUtility.DistancePointToLineSegment(mousePosition, p1, p2);
        }
        public float DistanceToCircle(Vector3 center, float radius)
        {
            float dist = ((Vector2)center - mousePosition).magnitude;
            if (dist < radius)
                return 0;
            return dist - radius;
        }
        public Vector3 GUIToWorld(Vector2 guiPosition, Vector3 planeNormal, Vector3 planePos) { return guiPosition; }
    }
}
