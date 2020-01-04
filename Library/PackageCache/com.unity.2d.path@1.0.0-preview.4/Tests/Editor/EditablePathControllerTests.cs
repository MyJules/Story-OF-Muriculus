using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

namespace UnityEditor.U2D.Path.Tests
{

    public class EditablePathControllerTests
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

        private EditablePath m_EditablePath;
        private EditablePathController m_Controller;
        private IUndoObject m_UndoObject;

        [SetUp]
        public void Setup()
        {
            m_UndoObject = new TestUndo();
            m_EditablePath = new EditablePath();
            m_Controller = new EditablePathController();
            m_Controller.editablePath = m_EditablePath;
            m_EditablePath.undoObject = m_UndoObject;
        }

        private void MakeClosedPolygon()
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

            m_EditablePath.shapeType = shape.type;
            m_EditablePath.isOpenEnded = shape.isOpenEnded;

            foreach (var controlPoint in controlPoints)
                m_EditablePath.AddPoint(controlPoint);
        }

        private void MakeClosedSpline()
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

        [Test]
        public void MakeSpline_InitializesPath()
        {
            MakeClosedSpline();

            Assert.AreEqual(4, m_EditablePath.pointCount, "Incorrect point count");
            Assert.AreEqual(ShapeType.Spline, m_EditablePath.shapeType, "Incorrect shape type");
            Assert.IsFalse(m_EditablePath.isOpenEnded, "Incorrect open-ended");
        }

        [Test]
        public void MakePolygon_InitializesPath()
        {
            MakeClosedPolygon();

            Assert.AreEqual(4, m_EditablePath.pointCount, "Incorrect point count");
            Assert.AreEqual(ShapeType.Polygon, m_EditablePath.shapeType, "Incorrect shape type");
            Assert.IsFalse(m_EditablePath.isOpenEnded, "Incorrect open-ended");
        }

        [Test]
        public void ClearSelection()
        {
            m_EditablePath.selection.Select(0, true);
            Assert.AreEqual(1, m_EditablePath.selection.Count, "Incorrect selection");
            m_Controller.ClearSelection();
            Assert.AreEqual(0, m_EditablePath.selection.Count, "Incorrect selection");
        }

        [Test]
        public void SelectPoint()
        {
            m_Controller.SelectPoint(0, true);
            Assert.AreEqual(1, m_EditablePath.selection.Count, "Incorrect selection");
            Assert.IsTrue(m_EditablePath.selection.Contains(0), "Incorrect selection");
            m_Controller.SelectPoint(0, false);
            Assert.AreEqual(0, m_EditablePath.selection.Count, "Incorrect selection");
        }

        [Test]
        public void CreatePoint_InPolygon()
        {
            MakeClosedPolygon();

            m_Controller.CreatePoint(0, new Vector2(150f, 100f));
            Assert.AreEqual(5, m_EditablePath.pointCount, "Incorrect point count");
            Assert.AreEqual(new Vector3(150f, 100f, 0f), m_EditablePath.GetPoint(1).position, "Incorrect point position");
        }

        [Test]
        public void CreatePoint_InSplineCurve_ProducesContinuousPoint()
        {
            MakeClosedSpline();

            m_EditablePath.SetTangentMode(1, TangentMode.Continuous);
            m_EditablePath.SetTangentMode(2, TangentMode.Continuous);
            m_Controller.CreatePoint(1, new Vector2(150f, 250f));
            Assert.AreEqual(5, m_EditablePath.pointCount, "Incorrect point count");

            Assert.That(m_EditablePath.GetPoint(2).position, Is.EqualTo(new Vector3(150f, 217.6777f, 0f)).Using(vec3Compare));
            Assert.AreEqual(TangentMode.Continuous, m_EditablePath.GetPoint(2).tangentMode, "Incorrect tangent mode");
            Assert.AreEqual(TangentMode.Continuous, m_EditablePath.GetPoint(1).tangentMode, "Incorrect tangent mode");
            Assert.AreEqual(TangentMode.Continuous, m_EditablePath.GetPoint(3).tangentMode, "Incorrect tangent mode");
            Assert.That(m_EditablePath.GetPoint(2).localLeftTangent, Is.EqualTo(new Vector3(-19.10745f, 0f, 0f)).Using(vec3Compare));
            Assert.That(m_EditablePath.GetPoint(2).localRightTangent, Is.EqualTo(new Vector3(19.10745f, 0f, 0f)).Using(vec3Compare));
            Assert.That(m_EditablePath.GetPoint(1).localRightTangent, Is.EqualTo(new Vector3(11.78511f, 11.78511f, 0f)).Using(vec3Compare));
            Assert.That(m_EditablePath.GetPoint(3).localLeftTangent, Is.EqualTo(new Vector3(-11.78511f, 11.78511f, 0f)).Using(vec3Compare));
        }

        [Test]
        public void CreatePoint_InSplineLinearCurve_ProducesLinearPoint()
        {
            MakeClosedSpline();

            Assert.AreEqual(TangentMode.Linear, m_EditablePath.GetPoint(1).tangentMode, "Incorrect tangent mode");
            Assert.AreEqual(TangentMode.Linear, m_EditablePath.GetPoint(2).tangentMode, "Incorrect tangent mode");

            m_Controller.CreatePoint(1, new Vector2(150f, 200f));

            Assert.AreEqual(5, m_EditablePath.pointCount, "Incorrect point count");
            Assert.AreEqual(new Vector3(150f, 200f, 0f), m_EditablePath.GetPoint(2).position, "Incorrect point position");
            Assert.AreEqual(TangentMode.Linear, m_EditablePath.GetPoint(2).tangentMode, "Incorrect tangent mode");
            Assert.AreEqual(TangentMode.Linear, m_EditablePath.GetPoint(1).tangentMode, "Incorrect tangent mode");
            Assert.AreEqual(TangentMode.Linear, m_EditablePath.GetPoint(3).tangentMode, "Incorrect tangent mode");
            Assert.AreEqual(Vector3.zero, m_EditablePath.GetPoint(2).localLeftTangent, "Incorrect point position");
            Assert.AreEqual(Vector3.zero, m_EditablePath.GetPoint(2).localRightTangent, "Incorrect point position");
        }

        [Test]
        public void CreatePoint_InSplineCurve_WithOneLinearNeighbour_NeighbourBecomesBroken()
        {
            MakeClosedSpline();

            Assert.AreEqual(TangentMode.Linear, m_EditablePath.GetPoint(2).tangentMode, "Incorrect tangent mode");

            m_EditablePath.SetTangentMode(1, TangentMode.Continuous);
            m_Controller.CreatePoint(1, new Vector2(150f, 250f));

            Assert.AreEqual(5, m_EditablePath.pointCount, "Incorrect point count");
            Assert.AreEqual(TangentMode.Continuous, m_EditablePath.GetPoint(1).tangentMode, "Incorrect tangent mode");
            Assert.AreEqual(TangentMode.Broken, m_EditablePath.GetPoint(3).tangentMode, "Incorrect tangent mode");
        }

        [Test]
        public void CreatePoint_ClearsSelection()
        {
            MakeClosedSpline();

            m_EditablePath.selection.Select(0, true);
            m_EditablePath.selection.Select(1, true);
            m_EditablePath.selection.Select(2, true);
            m_EditablePath.selection.Select(3, true);

            Assert.AreEqual(4, m_EditablePath.selection.Count, "Incorrect selection");

            m_Controller.CreatePoint(1, new Vector2(150f, 250f));
            
            Assert.AreEqual(0, m_EditablePath.selection.Count, "Incorrect selection");
        }

        [Test]
        public void RemoveSelectedPoints_ClearsSelection()
        {
            MakeClosedPolygon();

            m_EditablePath.selection.Select(0, true);
            m_EditablePath.selection.Select(1, true);
            m_EditablePath.selection.Select(2, true);
            m_EditablePath.selection.Select(3, true);

            Assert.AreEqual(4, m_EditablePath.selection.Count, "Incorrect selection");

            m_Controller.RemoveSelectedPoints();
            
            Assert.AreEqual(0, m_EditablePath.selection.Count, "Incorrect selection");
        }

        [Test]
        public void RemoveSelectedPoints_WithOpenEndedPath_KeepsMinimumTwoPoints()
        {
            MakeClosedPolygon();

            m_EditablePath.isOpenEnded = true;
            m_EditablePath.selection.Select(0, true);
            m_EditablePath.selection.Select(1, true);
            m_EditablePath.selection.Select(2, true);
            m_EditablePath.selection.Select(3, true);

            m_Controller.RemoveSelectedPoints();
            
            Assert.AreEqual(2, m_EditablePath.pointCount, "Incorrect point count");
            Assert.IsTrue(m_EditablePath.isOpenEnded, "Incorrect open-ended state");
        }

        [Test]
        public void RemoveSelectedPoints_WithClosedPath_KeepsMinimumThreePoints()
        {
            MakeClosedPolygon();

            m_EditablePath.selection.Select(0, true);
            m_EditablePath.selection.Select(1, true);
            m_EditablePath.selection.Select(2, true);
            m_EditablePath.selection.Select(3, true);

            m_Controller.RemoveSelectedPoints();
            
            Assert.AreEqual(3, m_EditablePath.pointCount, "Incorrect point count");
            Assert.IsFalse(m_EditablePath.isOpenEnded, "Incorrect open-ended state");
        }

        [Test]
        public void MoveSelectedPoints()
        {
            MakeClosedPolygon();

            m_EditablePath.selection.Select(1, true);
            m_EditablePath.selection.Select(2, true);

            m_Controller.MoveSelectedPoints(Vector3.up);
            
            Assert.AreEqual(new Vector3(100f, 200f, 0f) + Vector3.up, m_EditablePath.GetPoint(1).position, "Incorrect point position");
            Assert.AreEqual(new Vector3(200f, 200f, 0f) + Vector3.up, m_EditablePath.GetPoint(2).position, "Incorrect point position");
            Assert.AreEqual(2, m_EditablePath.selection.Count, "Incorrect selection");
        }

        [Test]
        public void MoveSelectedPoints_WithRotatedPaths_ForEachPath()
        {
            MakeClosedPolygon();

            var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);
            m_EditablePath.localToWorldMatrix = matrix;
            m_EditablePath.right = matrix.MultiplyVector(Vector3.right);
            m_EditablePath.up = matrix.MultiplyVector(Vector3.up);

            Assert.That(m_EditablePath.GetPoint(1).position, Is.EqualTo(new Vector3(-200f, 100f, 0f)).Using(vec3Compare));

            m_EditablePath.selection.Select(1, true);

            Vector3 delta = Vector2.one;
            m_Controller.MoveSelectedPoints(delta);
            
            Assert.That(m_EditablePath.GetPoint(1).position, Is.EqualTo(new Vector3(-200f, 100f, 0f) + delta).Using(vec3Compare));
        }

        [Test]
        public void MoveEdge()
        {
            MakeClosedPolygon();

            m_Controller.MoveEdge(1, Vector3.up);
            
            Assert.AreEqual(new Vector3(100f, 200f, 0f) + Vector3.up, m_EditablePath.GetPoint(1).position, "Incorrect point position");
            Assert.AreEqual(new Vector3(200f, 200f, 0f) + Vector3.up, m_EditablePath.GetPoint(2).position, "Incorrect point position");
        }

        [Test]
        public void MoveEdge_WithLastPointIndex()
        {
            MakeClosedPolygon();

            m_Controller.MoveEdge(3, Vector3.up);
            
            Assert.AreEqual(new Vector3(200f, 100f, 0f) + Vector3.up, m_EditablePath.GetPoint(3).position, "Incorrect point position");
            Assert.AreEqual(new Vector3(100f, 100f, 0f) + Vector3.up, m_EditablePath.GetPoint(0).position, "Incorrect point position");
        }

        [Test]
        public void MoveEdge_WithLastPointIndex_WithOpenEnded_DoesNotMovePoints()
        {
            MakeClosedPolygon();

            m_EditablePath.isOpenEnded = true;
            m_Controller.MoveEdge(3, Vector3.down);
            
            Assert.AreEqual(new Vector3(200f, 100f, 0f), m_EditablePath.GetPoint(3).position, "Incorrect point position");
            Assert.AreEqual(new Vector3(100f, 100f, 0f), m_EditablePath.GetPoint(0).position, "Incorrect point position");
        }

        [Test]
        public void SetLeftTangent_SetToLinear_SetsCachedRightTangent()
        {
            MakeClosedPolygon();

            var cachedTangent = new Vector3(150f, 250f, 0f);

            m_EditablePath.SetTangentMode(1, TangentMode.Continuous);
            m_Controller.SetLeftTangent(1, Vector3.zero, true, false, cachedTangent, TangentMode.Continuous);
            
            Assert.AreEqual(TangentMode.Broken, m_EditablePath.GetPoint(1).tangentMode, "Incorrect tangent mode");
            Assert.AreEqual(m_EditablePath.GetPoint(1).position, m_EditablePath.GetPoint(1).leftTangent, "Incorrect tangent position");
            Assert.AreEqual(cachedTangent, m_EditablePath.GetPoint(1).rightTangent, "Incorrect tangent position");
        }

        [Test]
        public void SetLeftTangent_WithContinuousTangentMode()
        {
            MakeClosedPolygon();

            m_EditablePath.SetTangentMode(1, TangentMode.Continuous);
            m_Controller.SetLeftTangent(1, new Vector3(50f, 50f, 0f), false, false, Vector3.zero, TangentMode.Continuous);
            
            Assert.That(m_EditablePath.GetPoint(1).leftTangent, Is.EqualTo(new Vector3(50f, 50f, 0f)).Using(vec3Compare));
            Assert.That(m_EditablePath.GetPoint(1).rightTangent, Is.EqualTo(new Vector3(110.5409f, 231.6228f, 0f)).Using(vec3Compare));
        }

        [Test]
        public void SetLeftTangent_WithContinuousTangentMode_WithMirror()
        {
            MakeClosedPolygon();

            m_EditablePath.SetTangentMode(1, TangentMode.Continuous);
            m_Controller.SetLeftTangent(1, new Vector3(50f, 50f, 0f), false, true, Vector3.zero, TangentMode.Continuous);
            
            Assert.That(m_EditablePath.GetPoint(1).leftTangent, Is.EqualTo(new Vector3(50f, 50f, 0f)).Using(vec3Compare));
            Assert.That(m_EditablePath.GetPoint(1).rightTangent, Is.EqualTo(new Vector3(150f, 350f, 0f)).Using(vec3Compare));
        }

        [Test]
        public void SetLeftTangent_WithBrokenTangentMode_WithMirror()
        {
            MakeClosedPolygon();

            m_EditablePath.SetTangentMode(1, TangentMode.Continuous);
            m_EditablePath.SetTangentMode(1, TangentMode.Broken);
            m_Controller.SetLeftTangent(1, new Vector3(50f, 50f, 0f), false, true, Vector3.zero, TangentMode.Broken);
            
            Assert.That(m_EditablePath.GetPoint(1).leftTangent, Is.EqualTo(new Vector3(50f, 50f, 0f)).Using(vec3Compare));
            Assert.That(m_EditablePath.GetPoint(1).rightTangent, Is.EqualTo(new Vector3(150f, 350f, 0f)).Using(vec3Compare));
        }

        [Test]
        public void SetRightTangent_SetToLinear_SetsCachedLeftTangent()
        {
            MakeClosedPolygon();

            var cachedTangent = new Vector3(50f, 50f, 0f);

            m_EditablePath.SetTangentMode(1, TangentMode.Continuous);
            m_Controller.SetRightTangent(1, Vector3.zero, true, false, cachedTangent, TangentMode.Continuous);
            
            Assert.AreEqual(TangentMode.Broken, m_EditablePath.GetPoint(1).tangentMode, "Incorrect tangent mode");
            Assert.AreEqual(m_EditablePath.GetPoint(1).position, m_EditablePath.GetPoint(1).rightTangent, "Incorrect tangent position");
            Assert.AreEqual(cachedTangent, m_EditablePath.GetPoint(1).leftTangent, "Incorrect tangent position");
        }

        [Test]
        public void SetRightTangent_WithContinuousTangentMode()
        {
            MakeClosedPolygon();

            m_EditablePath.SetTangentMode(1, TangentMode.Continuous);
            m_Controller.SetRightTangent(1, new Vector3(150f, 250f, 0f), false, false, Vector3.zero, TangentMode.Continuous);
            
            Assert.That(m_EditablePath.GetPoint(1).rightTangent, Is.EqualTo(new Vector3(150f, 250f, 0f)).Using(vec3Compare));
            Assert.That(m_EditablePath.GetPoint(1).leftTangent, Is.EqualTo(new Vector3(76.42978f, 176.4298f, 0f)).Using(vec3Compare));
        }

        [Test]
        public void SetRightTangent_WithContinuousTangentMode_WithMirror()
        {
            MakeClosedPolygon();

            m_EditablePath.SetTangentMode(1, TangentMode.Continuous);
            m_Controller.SetRightTangent(1, new Vector3(150f, 250f, 0f), false, true, Vector3.zero, TangentMode.Continuous);
            
            Assert.That(m_EditablePath.GetPoint(1).rightTangent, Is.EqualTo(new Vector3(150f, 250f, 0f)).Using(vec3Compare));
            Assert.That(m_EditablePath.GetPoint(1).leftTangent, Is.EqualTo(new Vector3(50f, 150f, 0f)).Using(vec3Compare));
        }

        [Test]
        public void SetRightTangent_WithBrokenTangentMode_WithMirror()
        {
            MakeClosedPolygon();

            m_EditablePath.SetTangentMode(1, TangentMode.Continuous);
            m_EditablePath.SetTangentMode(1, TangentMode.Broken);
            m_Controller.SetRightTangent(1, new Vector3(150f, 250f, 0f), false, true, Vector3.zero, TangentMode.Broken);
            
            Assert.That(m_EditablePath.GetPoint(1).rightTangent, Is.EqualTo(new Vector3(150f, 250f, 0f)).Using(vec3Compare));
            Assert.That(m_EditablePath.GetPoint(1).leftTangent, Is.EqualTo(new Vector3(50f, 150f, 0f)).Using(vec3Compare));
        }
    }
}
