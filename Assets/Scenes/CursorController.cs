using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    private void Start()
    {
        setVisible(false);
    }
    public void setVisible(bool isVisible)
    {
        Cursor.visible = isVisible;
    }
}
