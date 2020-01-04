using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor.U2D.Path.GUIFramework;

namespace UnityEditor.U2D.Path.Tests
{
    public class TestUndo : IUndoObject
    {
        public void RegisterUndo(string name) { }
    }
}
