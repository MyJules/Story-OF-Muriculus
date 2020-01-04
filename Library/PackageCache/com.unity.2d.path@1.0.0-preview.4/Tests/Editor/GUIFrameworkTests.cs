using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.U2D.Path.GUIFramework;

namespace UnityEditor.U2D.Path.Tests
{
    public class GUIFrameworkTests
    {
        private TestGUIState m_GUIState;
        private GUISystem m_GUISystem;
        
        [SetUp]
        public void Setup()
        {
            m_GUIState = new TestGUIState();
            m_GUISystem = new GUISystem(m_GUIState);
        }

        private void SendEvent(EventType eventType)
        {
            m_GUIState.eventType = EventType.Layout;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();
            
            m_GUIState.eventType = eventType;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();
        }

        private void SendMouseDown(int button, int clickCount = 1)
        {
            m_GUIState.eventType = EventType.Layout;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();
            
            m_GUIState.clickCount = clickCount;
            m_GUIState.eventType = EventType.MouseDown;
            m_GUIState.mouseButton = button;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();
        }

        private void SendDrag()
        {
            m_GUIState.eventType = EventType.Layout;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();
            
            m_GUIState.mouseButton = 0;
            m_GUIState.clickCount = 1;
            m_GUIState.eventType = EventType.MouseDown;
            m_GUIState.BeginEventPass();
            m_GUISystem.OnGUI();

            m_GUIState.eventType = EventType.MouseDrag;
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

        private DefaultControl AddDefaultControl(string name)
        {
            var control = new GenericDefaultControl(name);
            m_GUISystem.AddControl(control);
            return control;
        }

        private Control AddControl(string name, float distance)
        {
            return AddControl(name, distance, null);
        }

        private Control AddControl(string name, float distance, object userData)
        {
            return AddControl(name, 1, new float[] { distance }, new object[] { userData });
        }

        private Control AddControl(string name, int count, float[] distances)
        {
            return AddControl(name, count, distances, new object[distances.Length]);
        }

        private Control AddControl(string name, int count, float[] distances, object[] userData)
        {
            var control = new GenericControl(name)
            {
                count = () => { return count; },
                distance = (guiState, i) => { return distances[i]; },
                userData = (i) => { return userData[i]; }
            };

            m_GUISystem.AddControl(control);

            return control;
        }

        [Test]
        public void AddNullControl_Trows()
        {
            Assert.Throws<System.NullReferenceException>(() => { m_GUISystem.AddControl(null); }, "Adding null control should throw");
        }

        [Test]
        public void AddNullAction_Trows()
        {
            Assert.Throws<System.NullReferenceException>(() => { m_GUISystem.AddAction(null); }, "Adding null action should throw");
        }

        [Test]
        public void SingleControl_LayoutDataIsNearest()
        {
            var control = AddControl("TestControl", 0f);
            SendEvent(EventType.Repaint);

            Assert.AreEqual(control.ID, m_GUIState.nearestControl, "Control should be the nearest");
        }

        [Test]
        public void SingleControl_MultipleElements_LayoutDataIsNearest()
        {
            var control = AddControl("TestControl", 2, new float[] { 1f, 0f });
            SendEvent(EventType.Repaint);

            Assert.AreEqual(control.ID, m_GUIState.nearestControl, "Control should be the nearest");
            Assert.AreEqual(1, control.layoutData.index, "Nearest element index is not correct");
        }

        [Test]
        public void MultipleControl_LayoutDataIsNearest()
        {
            var control1 = AddControl("TestControl1", 1f);
            var control2 = AddControl("TestControl2", 0f);
            SendEvent(EventType.Repaint);
            SendEvent(EventType.Repaint);

            Assert.AreNotEqual(control1.ID, m_GUIState.nearestControl, "Control1 should not be the nearest");
            Assert.AreEqual(control2.ID, m_GUIState.nearestControl, "Control2 should be the nearest");
        }

        [Test]
        public void MultipleControl_MultipleElements_LayoutDataIsNearest()
        {
            var control1 = AddControl("TestControl1", 2, new float[] { 2f, 2f });
            var control2 = AddControl("TestControl2", 2, new float[] { 1f, 0f });
            SendEvent(EventType.Repaint);

            Assert.AreNotEqual(control1.ID, m_GUIState.nearestControl, "Control1 should not be the nearest");
            Assert.AreEqual(control2.ID, m_GUIState.nearestControl, "Control2 should be the nearest");
            Assert.AreEqual(1, control2.layoutData.index, "Nearest element index is not correct");
        }

        [Test]
        public void ChangingNearestControl_InvokesRepaint()
        {
            var control1 = AddControl("TestControl1", 0f);
            SendEvent(EventType.Repaint);

            Assert.AreEqual(control1.ID, m_GUIState.nearestControl, "Control1 should be the nearest");

            var control2 = AddControl("TestControl2", 0f);
            SendEvent(EventType.Repaint);
            
            Assert.AreEqual(control2.ID, m_GUIState.nearestControl, "Control2 should be the nearest");
            Assert.IsTrue(m_GUIState.repaintFlag, "Repaint did not trigger");
        }

        [Test]
        public void ChangingNearestElement_TriggersRepaint()
        {
            var distances = new float[] { 0f, 1f };
            var control = AddControl("TestControl1", 2, distances);
            SendEvent(EventType.Repaint);

            Assert.AreEqual(control.ID, m_GUIState.nearestControl, "Control should be the nearest");
            Assert.AreEqual(0, control.layoutData.index, "Nearest element index is not correct");

            distances[0] = 1f;
            distances[1] = 0f;

            SendEvent(EventType.Repaint);

            Assert.AreEqual(control.ID, m_GUIState.nearestControl, "Control should be the nearest");
            Assert.AreEqual(1, control.layoutData.index, "Nearest element index is not correct");
        }

        [Test]
        public void OnLayoutEvent_InvokesControlBeginEndLayout()
        {
            var beginLayout = false;
            var endLayout = false;

            var control = AddControl("TestControl", 0f) as GenericControl;
            control.onBeginLayout = (s) => { beginLayout = true; return LayoutData.zero; };
            control.onEndLayout = (s) => { endLayout = true; };

            SendEvent(EventType.Repaint);

            Assert.IsTrue(beginLayout, "BeginLayout was not invoked");
            Assert.IsTrue(endLayout, "EndLayout was not invoked");
        }

        [Test]
        public void OnRepaintEvent_InvokesControlRepaint()
        {
            var repaint = false;

            var control = AddControl("TestControl", 0f) as GenericControl;
            control.onRepaint = (s, c, i) => { repaint = true; };

            SendEvent(EventType.Repaint);

            Assert.IsTrue(repaint, "Repaint was not invoked");
        }

        [Test]
        public void DefaultControl_UsesFixedPickDistance()
        {
            var control = AddDefaultControl("TestDefaultControl");
            SendEvent(EventType.Repaint);

            Assert.AreEqual(control.ID, m_GUIState.nearestControl, "Control should be the nearest");
            Assert.AreEqual(control.layoutData.distance, DefaultControl.kPickDistance, "Incorrect layout distance");
        }

        [Test]
        public void DefaultControl_IsNearestWhenOtherControlsAreNotPickable()
        {
            var defaultControl = AddDefaultControl("TestDefaultControl");
            var control = AddControl("OtherControl", DefaultControl.kPickDistance + 0.1f);
            SendEvent(EventType.Repaint);

            Assert.AreEqual(defaultControl.ID, m_GUIState.nearestControl, "DefaultControl should be the nearest");
            Assert.AreEqual(defaultControl.layoutData.distance, DefaultControl.kPickDistance, "Incorrect layout distance");
        }

        [Test]
        public void ClickAction_InvokesOnClick_OnNearestControl()
        {
            var mouseButton = 0;
            var clicked = false;
            var control = AddControl("TestControl", 0f);
            var action = new ClickAction(control, mouseButton);
            action.onClick = (s, c) => { clicked = true; };
            m_GUISystem.AddAction(action);
            SendMouseDown(mouseButton);

            Assert.AreEqual(control.ID, m_GUIState.nearestControl, "Control should be the nearest");
            Assert.IsTrue(clicked, "OnClick was not invoked");
        }

        [Test]
        public void DoublicClickAction_InvokesOnClick_OnNearestControl()
        {
            var mouseButton = 0;
            var clicked = false;
            var control = AddControl("TestControl", 0f);
            var action = new ClickAction(control, mouseButton);
            action.clickCount = 2;
            action.onClick = (s, c) => { clicked = true; };
            m_GUISystem.AddAction(action);
            SendMouseDown(mouseButton, 1);
            SendMouseDown(mouseButton, 2);

            Assert.AreEqual(control.ID, m_GUIState.nearestControl, "Control should be the nearest");
            Assert.IsTrue(clicked, "OnClick was not invoked");
        }

        [Test]
        public void DoublicClickAction_SkippingClickCount1_DoesNotInvokeOnClick_OnNearestControl()
        {
            var mouseButton = 0;
            var clicked = false;
            var control = AddControl("TestControl", 0f);
            var action = new ClickAction(control, mouseButton);
            action.clickCount = 2;
            action.onClick = (s, c) => { clicked = true; };
            m_GUISystem.AddAction(action);
            SendMouseDown(mouseButton, 2);

            Assert.AreEqual(control.ID, m_GUIState.nearestControl, "Control should be the nearest");
            Assert.IsFalse(clicked, "OnClick was invoked");
        }

        [Test]
        public void ClickAction_DoesNotInvokeOnClick_OnNonPickableControl()
        {
            var mouseButton = 0;
            var clicked = false;
            var control = AddControl("TestControl", DefaultControl.kPickDistance + 0.1f);
            var action = new ClickAction(control, mouseButton);
            action.onClick = (s, c) => { clicked = true; };
            m_GUISystem.AddAction(action);
            SendMouseDown(mouseButton);

            Assert.AreEqual(0, m_GUIState.nearestControl, "Control should be the nearest");
            Assert.IsFalse(clicked, "OnClick was not invoked");
        }

        [Test]
        public void SliderAction_OnNearestControl_InvokesBeginChangedEnd_WantsRepaint()
        {
            var begin = false;
            var changed = false;
            var end = false;
            var control = AddControl("TestControl", 0f);
            var action = new SliderAction(control);
            action.onSliderBegin = (s, c, p) => { begin = true; };
            action.onSliderChanged = (s, c, p) => { changed = true; };
            action.onSliderEnd = (s, c, p) => { end = true; };
            m_GUISystem.AddAction(action);
            SendDrag();

            Assert.IsTrue(begin, "OnSliderBegin was not invoked");
            Assert.IsTrue(changed, "OnSliderChanged was not invoked");
            Assert.IsTrue(end, "OnSliderEnd was not invoked");
            Assert.IsTrue(m_GUIState.repaintFlag, "OnRepaint was not invoked");
        }

        [Test]
        public void CommandAction_InvokesOnCommand()
        {
            var commandName = "Command";
            var command = false;
            var action = new CommandAction(commandName);
            action.onCommand = (s) => { command = true; };
            m_GUISystem.AddAction(action);
            SendCommand(commandName);

            Assert.IsTrue(command, "OnCommand was not invoked");
        }
    }
}
