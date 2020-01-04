using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEditor.U2D.Path.GUIFramework;

namespace UnityEditor.U2D.Path.Tests
{

    public class EditablePathViewTests
    {
        private TestGUIState m_GUIState;
        private GUISystem m_GUISystem;
        private IDrawer m_Drawer;
        private EditablePath m_EditablePath;
        private EditablePathController m_Controller;
        private PathEditor m_PathEditor;

        [SetUp]
        public void Setup()
        {
            m_GUIState = new TestGUIState();
            m_GUISystem = new GUISystem(m_GUIState);
            m_Drawer = new TestDrawer();
            m_EditablePath = new EditablePath();
            m_Controller = new EditablePathController();
            m_Controller.editablePath = m_EditablePath;
            m_PathEditor = new PathEditor(m_GUISystem);
            m_PathEditor.drawerOverride = m_Drawer;
            m_PathEditor.controller = m_Controller;
        }

        private void MakeSpline()
        {
            var points = new Vector3[]
            {
                new Vector2(100f, 100f),
                new Vector2(100f, 200f),
                new Vector2(200f, 200f),
                new Vector2(200f, 100f),
            };

            IShape shape = points.ToPolygon(false).ToSpline();
            var controlPoints = shape.ToControlPoints();

            m_EditablePath.shapeType = shape.type;
            m_EditablePath.isOpenEnded = shape.isOpenEnded;

            foreach (var controlPoint in controlPoints)
                m_EditablePath.AddPoint(controlPoint);
        }

        private void SendPointClick(int pointIndex, int button)
        {
            SendClick(m_EditablePath.GetPoint(pointIndex).position, button);
        }

        private void SendClick(Vector2 position, int button)
        {
            m_GUIState.mousePosition = position;
            m_GUIState.eventType = EventType.Layout;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();
            
            m_GUIState.eventType = EventType.MouseDown;
            m_GUIState.mouseButton = button;
            m_GUIState.clickCount = 1;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();

            m_GUIState.eventType = EventType.Layout;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();

            m_GUIState.eventType = EventType.MouseUp;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();
        }

        private void SendPointDrag(int pointIndex, Vector3 endPosition)
        {
            SendDrag(m_EditablePath.GetPoint(pointIndex).position, endPosition, 0);
        }

        private void SendLeftTangentDrag(int pointIndex, Vector3 endPosition)
        {
            SendDrag(m_EditablePath.GetPoint(pointIndex).leftTangent, endPosition, 0);
        }

        private void SendRightTangentDrag(int pointIndex, Vector3 endPosition)
        {
            SendDrag(m_EditablePath.GetPoint(pointIndex).rightTangent, endPosition, 0);
        }

        private void SendDrag(Vector2 start, Vector2 end, int button)
        {
            m_GUIState.sliderDelta = Vector3.zero;
            m_GUIState.mousePosition = start;
            m_GUIState.eventType = EventType.Layout;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();
            
            m_GUIState.eventType = EventType.MouseDown;
            m_GUIState.mouseButton = button;
            m_GUIState.clickCount = 1;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();

            m_GUIState.mousePosition = end;
            m_GUIState.eventType = EventType.Layout;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();

            m_GUIState.sliderDelta = end - start;
            m_GUIState.eventType = EventType.MouseDrag;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();

            m_GUIState.eventType = EventType.Layout;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();

            m_GUIState.eventType = EventType.MouseUp;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();
        }

        private void SendCommand(string name)
        {
            m_GUIState.eventType = EventType.Layout;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();
            
            m_GUIState.commandName = name;
            m_GUIState.eventType = EventType.ValidateCommand;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();

            if (m_GUIState.usedEvent)
            {
                m_GUIState.eventType = EventType.ExecuteCommand;
                m_GUIState.BeginEventPass();
                m_GUISystem.OnGUI();
            }
        }

        [Test]
        public void MakeSpline_InitializesPath()
        {
            MakeSpline();

            Assert.AreEqual(4, m_EditablePath.pointCount, "Incorrect point count");
            Assert.AreEqual(ShapeType.Spline, m_EditablePath.shapeType, "Incorrect shape type");
            Assert.IsFalse(m_EditablePath.isOpenEnded, "Incorrect open-ended");
        }

        [Test]
        public void PointClick_SelectsPoint()
        {
            MakeSpline();

            Assert.AreEqual(0, m_EditablePath.selection.Count, "Incorrect selection state");

            SendPointClick(0, 0);

            Assert.AreEqual(1, m_EditablePath.selection.Count, "Incorrect selection state");
            Assert.IsTrue(m_EditablePath.selection.Contains(0), "Incorrect selection state");

            SendPointClick(1, 0);

            Assert.AreEqual(1, m_EditablePath.selection.Count, "Incorrect selection state");
            Assert.IsTrue(m_EditablePath.selection.Contains(1), "Incorrect selection state");
        }

        [Test]
        public void PointDrag_SelectsAndMovesPoint()
        {
            MakeSpline();

            SendPointDrag(0, Vector3.zero);

            Assert.AreEqual(1, m_EditablePath.selection.Count, "Incorrect selection state");
            Assert.AreEqual(Vector3.zero, m_EditablePath.GetPoint(0).position, "Incorrect point position");

            SendPointDrag(1, Vector3.zero);

            Assert.AreEqual(1, m_EditablePath.selection.Count, "Incorrect selection state");
            Assert.AreEqual(Vector3.zero, m_EditablePath.GetPoint(1).position, "Incorrect point position");
        }

        [Test]
        public void EdgeDrag_ActionKeyDown_MovesEdgePoints()
        {
            MakeSpline();

            m_GUIState.isActionKeyDown = true;
            SendDrag(new Vector3(150f, 100f, 0f), Vector3.zero, 0);

            Assert.AreEqual(0, m_EditablePath.selection.Count, "Incorrect selection state");
            Assert.AreEqual(new Vector3(-50f, 0f, 0f), m_EditablePath.GetPoint(0).position, "Incorrect point position");
            Assert.AreEqual(new Vector3(50f, 0f, 0f), m_EditablePath.GetPoint(3).position, "Incorrect point position");
        }

        [Test]
        public void TangentDrag_MovesTangent()
        {
            MakeSpline();

            var pointPosition = m_EditablePath.GetPoint(0).position;

            m_EditablePath.SetTangentMode(0, TangentMode.Broken);

            Assert.AreNotEqual(Vector3.zero, m_EditablePath.GetPoint(0).localLeftTangent, "Incorrect left tangent");
            Assert.AreNotEqual(Vector3.zero, m_EditablePath.GetPoint(0).localRightTangent, "Incorrect right tangent");

            SendPointClick(0, 0);

            Assert.AreEqual(1, m_EditablePath.selection.Count, "Incorrect selection state");

            SendLeftTangentDrag(0, Vector3.zero);
            SendRightTangentDrag(0, Vector3.zero);

            Assert.AreEqual(pointPosition, m_EditablePath.GetPoint(0).position, "Incorrect point position");
            Assert.AreEqual(Vector3.zero, m_EditablePath.GetPoint(0).leftTangent, "Incorrect left tangent position");
            Assert.AreEqual(Vector3.zero, m_EditablePath.GetPoint(0).rightTangent, "Incorrect right tangent position");
        }

        [Test]
        public void DragBothTangentsToPoint_SetsLinearTangentMode()
        {
            MakeSpline();

            var pointPosition = m_EditablePath.GetPoint(0).position;

            m_EditablePath.SetTangentMode(0, TangentMode.Broken);

            Assert.AreNotEqual(Vector3.zero, m_EditablePath.GetPoint(0).localLeftTangent, "Incorrect left tangent");
            Assert.AreNotEqual(Vector3.zero, m_EditablePath.GetPoint(0).localRightTangent, "Incorrect right tangent");
            Assert.AreEqual(TangentMode.Broken, m_EditablePath.GetPoint(0).tangentMode, "Incorrect tangent mode");

            SendPointClick(0, 0);

            Assert.AreEqual(1, m_EditablePath.selection.Count, "Incorrect selection state");

            SendLeftTangentDrag(0, m_EditablePath.GetPoint(0).position);
            SendRightTangentDrag(0, m_EditablePath.GetPoint(0).position);

            Assert.AreEqual(pointPosition, m_EditablePath.GetPoint(0).position, "Incorrect point position");
            Assert.AreEqual(Vector3.zero, m_EditablePath.GetPoint(0).localLeftTangent, "Incorrect left tangent position");
            Assert.AreEqual(Vector3.zero, m_EditablePath.GetPoint(0).localRightTangent, "Incorrect right tangent position");
            Assert.AreEqual(TangentMode.Linear, m_EditablePath.GetPoint(0).tangentMode, "Incorrect tangent mode");
        }

        [Test]
        public void DragLeftTangentToPoint_WhileTangentModeContinuous_SetsTangentModeBroken()
        {
            MakeSpline();

            var pointPosition = m_EditablePath.GetPoint(0).position;

            m_EditablePath.SetTangentMode(0, TangentMode.Continuous);

            Assert.AreNotEqual(Vector3.zero, m_EditablePath.GetPoint(0).localLeftTangent, "Incorrect left tangent");
            Assert.AreNotEqual(Vector3.zero, m_EditablePath.GetPoint(0).localRightTangent, "Incorrect right tangent");
            Assert.AreEqual(TangentMode.Continuous, m_EditablePath.GetPoint(0).tangentMode, "Incorrect tangent mode");

            SendPointClick(0, 0);

            Assert.AreEqual(1, m_EditablePath.selection.Count, "Incorrect selection state");

            SendLeftTangentDrag(0, m_EditablePath.GetPoint(0).position);

            Assert.AreEqual(pointPosition, m_EditablePath.GetPoint(0).position, "Incorrect point position");
            Assert.AreEqual(Vector3.zero, m_EditablePath.GetPoint(0).localLeftTangent, "Incorrect left tangent position");
            Assert.AreNotEqual(Vector3.zero, m_EditablePath.GetPoint(0).localRightTangent, "Incorrect right tangent");
            Assert.AreEqual(TangentMode.Broken, m_EditablePath.GetPoint(0).tangentMode, "Incorrect tangent mode");
        }

        [Test]
        public void DragRightTangentToPoint_WhileTangentModeContinuous_SetsTangentModeBroken()
        {
            MakeSpline();

            var pointPosition = m_EditablePath.GetPoint(0).position;

            m_EditablePath.SetTangentMode(0, TangentMode.Continuous);

            Assert.AreNotEqual(Vector3.zero, m_EditablePath.GetPoint(0).localLeftTangent, "Incorrect left tangent");
            Assert.AreNotEqual(Vector3.zero, m_EditablePath.GetPoint(0).localRightTangent, "Incorrect right tangent");
            Assert.AreEqual(TangentMode.Continuous, m_EditablePath.GetPoint(0).tangentMode, "Incorrect tangent mode");

            SendPointClick(0, 0);

            Assert.AreEqual(1, m_EditablePath.selection.Count, "Incorrect selection state");

            SendRightTangentDrag(0, m_EditablePath.GetPoint(0).position);

            Assert.AreEqual(pointPosition, m_EditablePath.GetPoint(0).position, "Incorrect point position");
            Assert.AreNotEqual(Vector3.zero, m_EditablePath.GetPoint(0).localLeftTangent, "Incorrect left tangent");
            Assert.AreEqual(Vector3.zero, m_EditablePath.GetPoint(0).localRightTangent, "Incorrect right tangent position");
            Assert.AreEqual(TangentMode.Broken, m_EditablePath.GetPoint(0).tangentMode, "Incorrect tangent mode");
        }

        [Test]
        public void LeftTangentDrag_MovesTangent_MultipleSelectedPoints()
        {
            MakeSpline();

            m_EditablePath.SetTangentMode(0, TangentMode.Broken);
            m_EditablePath.SetTangentMode(1, TangentMode.Broken);

            m_EditablePath.selection.Select(0, true);
            m_EditablePath.selection.Select(1, true);

            Assert.IsTrue(m_EditablePath.selection.Contains(0), "Incorrect selection state");
            Assert.IsTrue(m_EditablePath.selection.Contains(1), "Incorrect selection state");

            SendLeftTangentDrag(0, Vector3.zero);
            SendLeftTangentDrag(1, Vector3.zero);

            Assert.AreEqual(Vector3.zero, m_EditablePath.GetPoint(0).leftTangent, "Incorrect left tangent position");
            Assert.AreEqual(Vector3.zero, m_EditablePath.GetPoint(1).leftTangent, "Incorrect left tangent position");
        }

        [Test]
        public void RightTangentDrag_MovesTangent_MultipleSelectedPoints()
        {
            MakeSpline();

            m_EditablePath.SetTangentMode(0, TangentMode.Broken);
            m_EditablePath.SetTangentMode(1, TangentMode.Broken);

            m_EditablePath.selection.Select(0, true);
            m_EditablePath.selection.Select(1, true);

            Assert.IsTrue(m_EditablePath.selection.Contains(0), "Incorrect selection state");
            Assert.IsTrue(m_EditablePath.selection.Contains(1), "Incorrect selection state");

            SendRightTangentDrag(0, Vector3.zero);
            SendRightTangentDrag(1, Vector3.zero);

            Assert.AreEqual(Vector3.zero, m_EditablePath.GetPoint(0).rightTangent, "Incorrect right tangent position");
            Assert.AreEqual(Vector3.zero, m_EditablePath.GetPoint(1).rightTangent, "Incorrect right tangent position");
        }

        [Test]
        public void ClickOnEdge_CreatesAndSelectsNewPoint()
        {
            MakeSpline();

            SendClick(new Vector3(150f, 100f, 0f), 0);

            Assert.AreEqual(5, m_EditablePath.pointCount, "Incorrect point count");
            Assert.IsTrue(m_EditablePath.selection.Contains(4), "Incorrect selection state");
            Assert.AreEqual(new Vector3(150f, 100f, 0f), m_EditablePath.GetPoint(4).position, "Incorrect point position");
        }

        private static IEnumerable<string> RemovePointCommands()
        {
            yield return "Delete";
            yield return "SoftDelete";
        }

        [Test]
        public void DeleteCommandEvent_RemovesSelectedPoints([ValueSource("RemovePointCommands")] string commandName)
        {
            MakeSpline();

            SendPointClick(0, 0);
            SendCommand(commandName);

            Assert.AreEqual(3, m_EditablePath.pointCount, "Incorrect point count");
            Assert.AreEqual(0, m_EditablePath.selection.Count, "Incorrect selection state");
            Assert.AreNotEqual(new Vector3(100f, 100f, 0f), m_EditablePath.GetPoint(0).position, "Incorrect point position");
        }
    }
}
