using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace UnityEditor.U2D.Path.Tests
{

    public class MultipleEditablePathControllerTests
    {
        private class Vector3Compare : IEqualityComparer<Vector3>
        {
            public bool Equals(Vector3 a, Vector3 b)
            {
                return Vector3.Distance(a, b) < Epsilon;
            }

            public int GetHashCode(Vector3 v)
            {
                return v.GetHashCode();
            }

            private static readonly float Epsilon = 0.001f;
        }

        private Vector3Compare vec3Compare = new Vector3Compare();

        private EditablePath m_EditablePath1;
        private EditablePath m_EditablePath2;
        private MultipleEditablePathController m_Controller;
        private IUndoObject m_UndoObject;

        [SetUp]
        public void Setup()
        {
            m_UndoObject = new TestUndo();
            m_EditablePath1 = new EditablePath();
            m_EditablePath2 = new EditablePath();
            m_Controller = new MultipleEditablePathController();
            m_Controller.AddPath(m_EditablePath1);
            m_Controller.AddPath(m_EditablePath2);
            m_Controller.editablePath = m_EditablePath1;
            m_EditablePath1.undoObject = m_UndoObject;
            m_EditablePath2.undoObject = m_UndoObject;
        }

        private void MakeClosedPolygon(IEditablePath editablePath)
        {
            var points = new Vector3[]
            {
                new Vector2(100f, 100f),
                new Vector2(100f, 200f),
                new Vector2(200f, 200f),
                new Vector2(200f, 100f),
            };

            IShape shape = points.ToPolygon(false);
            var controlPoints = shape.ToControlPoints();

            editablePath.shapeType = shape.type;
            editablePath.isOpenEnded = shape.isOpenEnded;

            foreach (var controlPoint in controlPoints)
                editablePath.AddPoint(controlPoint);
        }

        private void MakeClosedSpline(IEditablePath editablePath)
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

            editablePath.shapeType = shape.type;
            editablePath.isOpenEnded = shape.isOpenEnded;

            foreach (var controlPoint in controlPoints)
                editablePath.AddPoint(controlPoint);
        }

        [Test]
        public void MakeSpline_InitializesPath()
        {
            MakeClosedSpline(m_EditablePath1);

            Assert.AreEqual(4, m_EditablePath1.pointCount, "Incorrect point count");
            Assert.AreEqual(ShapeType.Spline, m_EditablePath1.shapeType, "Incorrect shape type");
            Assert.IsFalse(m_EditablePath1.isOpenEnded, "Incorrect open-ended");
        }

        [Test]
        public void MakePolygon_InitializesPath()
        {
            MakeClosedPolygon(m_EditablePath1);

            Assert.AreEqual(4, m_EditablePath1.pointCount, "Incorrect point count");
            Assert.AreEqual(ShapeType.Polygon, m_EditablePath1.shapeType, "Incorrect shape type");
            Assert.IsFalse(m_EditablePath1.isOpenEnded, "Incorrect open-ended");
        }

        [Test]
        public void ClearSelection_ClearsSelectionForEachPath()
        {
            m_EditablePath1.selection.Select(0, true);
            m_EditablePath2.selection.Select(0, true);
            Assert.AreEqual(1, m_EditablePath1.selection.Count, "Incorrect selection");
            Assert.AreEqual(1, m_EditablePath2.selection.Count, "Incorrect selection");
            m_Controller.ClearSelection();
            Assert.AreEqual(0, m_EditablePath1.selection.Count, "Incorrect selection");
            Assert.AreEqual(0, m_EditablePath2.selection.Count, "Incorrect selection");
        }

        [Test]
        public void RemoveSelectedPoints_WithOpenEndedPath_KeepsMinimumTwoPoints_ForEachPath()
        {
            MakeClosedPolygon(m_EditablePath1);
            MakeClosedPolygon(m_EditablePath2);

            m_EditablePath1.isOpenEnded = true;
            m_EditablePath1.selection.Select(0, true);
            m_EditablePath1.selection.Select(1, true);
            m_EditablePath1.selection.Select(2, true);
            m_EditablePath1.selection.Select(3, true);

            m_EditablePath2.isOpenEnded = true;
            m_EditablePath2.selection.Select(0, true);
            m_EditablePath2.selection.Select(1, true);
            m_EditablePath2.selection.Select(2, true);
            m_EditablePath2.selection.Select(3, true);

            m_Controller.RemoveSelectedPoints();
            
            Assert.AreEqual(2, m_EditablePath1.pointCount, "Incorrect point count");
            Assert.AreEqual(2, m_EditablePath2.pointCount, "Incorrect point count");
            Assert.IsTrue(m_EditablePath1.isOpenEnded, "Incorrect open-ended state");
            Assert.IsTrue(m_EditablePath2.isOpenEnded, "Incorrect open-ended state");
        }

        [Test]
        public void RemoveSelectedPoints_WithClosedPath_KeepsMinimumThreePoints_ForEachPath()
        {
            MakeClosedPolygon(m_EditablePath1);
            MakeClosedPolygon(m_EditablePath2);

            m_EditablePath1.selection.Select(0, true);
            m_EditablePath1.selection.Select(1, true);
            m_EditablePath1.selection.Select(2, true);
            m_EditablePath1.selection.Select(3, true);

            m_EditablePath2.selection.Select(0, true);
            m_EditablePath2.selection.Select(1, true);
            m_EditablePath2.selection.Select(2, true);
            m_EditablePath2.selection.Select(3, true);

            m_Controller.RemoveSelectedPoints();
            
            Assert.AreEqual(3, m_EditablePath1.pointCount, "Incorrect point count");
            Assert.AreEqual(3, m_EditablePath2.pointCount, "Incorrect point count");
            Assert.IsFalse(m_EditablePath1.isOpenEnded, "Incorrect open-ended state");
            Assert.IsFalse(m_EditablePath2.isOpenEnded, "Incorrect open-ended state");
        }

        [Test]
        public void MoveSelectedPoints_ForEachPath()
        {
            MakeClosedPolygon(m_EditablePath1);
            MakeClosedPolygon(m_EditablePath2);

            m_EditablePath1.selection.Select(1, true);
            m_EditablePath1.selection.Select(2, true);

            m_EditablePath2.selection.Select(1, true);
            m_EditablePath2.selection.Select(2, true);

            m_Controller.MoveSelectedPoints(Vector3.up);
            
            Assert.AreEqual(new Vector3(100f, 200f, 0f) + Vector3.up, m_EditablePath1.GetPoint(1).position, "Incorrect point position");
            Assert.AreEqual(new Vector3(200f, 200f, 0f) + Vector3.up, m_EditablePath1.GetPoint(2).position, "Incorrect point position");
            Assert.AreEqual(new Vector3(100f, 200f, 0f) + Vector3.up, m_EditablePath2.GetPoint(1).position, "Incorrect point position");
            Assert.AreEqual(new Vector3(200f, 200f, 0f) + Vector3.up, m_EditablePath2.GetPoint(2).position, "Incorrect point position");
            Assert.AreEqual(2, m_EditablePath1.selection.Count, "Incorrect selection");
            Assert.AreEqual(2, m_EditablePath2.selection.Count, "Incorrect selection");
        }

        [Test]
        public void MoveSelectedPoints_WithRotatedPaths_ForEachPath()
        {
            MakeClosedPolygon(m_EditablePath1);
            MakeClosedPolygon(m_EditablePath2);

            var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);
            m_EditablePath1.localToWorldMatrix = matrix;
            m_EditablePath2.localToWorldMatrix = matrix;
            m_EditablePath1.right = matrix.MultiplyVector(Vector3.right);
            m_EditablePath2.right = matrix.MultiplyVector(Vector3.right);
            m_EditablePath1.up = matrix.MultiplyVector(Vector3.up);
            m_EditablePath2.up = matrix.MultiplyVector(Vector3.up);

            Assert.That(m_EditablePath1.GetPoint(1).position, Is.EqualTo(new Vector3(-200f, 100f, 0f)).Using(vec3Compare));
            Assert.That(m_EditablePath2.GetPoint(1).position, Is.EqualTo(new Vector3(-200f, 100f, 0f)).Using(vec3Compare));

            m_EditablePath1.selection.Select(1, true);
            m_EditablePath2.selection.Select(1, true);

            Vector3 delta = Vector2.one;
            m_Controller.MoveSelectedPoints(delta);
            
            Assert.That(m_EditablePath1.GetPoint(1).position, Is.EqualTo(new Vector3(-200f, 100f, 0f) + delta).Using(vec3Compare));
            Assert.That(m_EditablePath2.GetPoint(1).position, Is.EqualTo(new Vector3(-200f, 100f, 0f) + delta).Using(vec3Compare));
        }
    }
}
