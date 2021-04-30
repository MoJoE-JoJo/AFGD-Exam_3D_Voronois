using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DnCDebugPoint : MonoBehaviour
{
    public float x; //Along x-axis. Total span
    public float y; //Along y-axis. Total span 
    public Vector3 center;

    public void DebugDraw(Color color)
    {
        //Handles.Label(center, "" + cellId);
        Debug.DrawLine(GetTopLeft(), GetTopRight(), color);
        Debug.DrawLine(GetTopRight(), GetBottomRight(), color);
        Debug.DrawLine(GetBottomRight(), GetBottomLeft(), color);
        Debug.DrawLine(GetBottomLeft(), GetTopLeft(), color);
        Debug.DrawLine(GetTopLeft(), GetBottomRight(), color);
        Debug.DrawLine(GetTopRight(), GetBottomLeft(), color);
    }

    public Vector3 GetTopLeft()
    {
        var topLeft = center;
        topLeft.x = center.x - x / 2;
        topLeft.y = center.y + y / 2;
        return topLeft;
    }

    public Vector3 GetTopRight()
    {
        var topLeft = center;
        topLeft.x = center.x + x / 2;
        topLeft.y = center.y + y / 2;
        return topLeft;
    }

    public Vector3 GetBottomLeft()
    {
        var topLeft = center;
        topLeft.x = center.x - x / 2;
        topLeft.y = center.y - y / 2;
        return topLeft;
    }

    public Vector3 GetBottomRight()
    {
        var topLeft = center;
        topLeft.x = center.x + x / 2;
        topLeft.y = center.y - y / 2;
        return topLeft;
    }
}
