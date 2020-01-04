using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.U2D.Path.GUIFramework;

namespace UnityEditor.U2D.Path.Tests
{
    public class TestDrawer : IDrawer
    {
        public void DrawBezier(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float width, Color color) { }
        public void DrawCreatePointPreview(Vector3 position) { }
        public void DrawDottedLine(Vector3 p1, Vector3 p2, float width, Color color) { }
        public void DrawLine(Vector3 p1, Vector3 p2, float width, Color color) { }
        public void DrawPoint(Vector3 position) { }
        public void DrawPointHovered(Vector3 position) { }
        public void DrawPointSelected(Vector3 position) { }
        public void DrawRemovePointPreview(Vector3 position) { }
        public void DrawSelectionRect(Rect rect) { }
        public void DrawTangent(Vector3 position, Vector3 tangent) { }
    }
}
