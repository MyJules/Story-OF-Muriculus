using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    private void Start()
    {
        Cursor.visible = true;
    }
    public void setVisible(bool isVisible)
    {
        Cursor.visible = isVisible;
    }
}
