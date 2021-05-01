using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DnCDebugPoint : MonoBehaviour
{
    public float x; //Along x-axis. Total span
    public float y; //Along y-axis. Total span 
    public float z; //Along z-axis. Total span 
    public Vector3 center;

    public void DebugDraw(Color color)
    {
        //Front plane
        Debug.DrawLine(GetLeftBottomFront(), GetRightBottomFront(), color);
        Debug.DrawLine(GetLeftBottomFront(), GetLeftTopFront(), color);
        Debug.DrawLine(GetLeftTopFront(), GetRightTopFront(), color);
        Debug.DrawLine(GetRightBottomFront(), GetRightTopFront(), color);

        //Back plane
        Debug.DrawLine(GetLeftBottomBack(), GetRightBottomBack(), color);
        Debug.DrawLine(GetLeftBottomBack(), GetLeftTopBack(), color);
        Debug.DrawLine(GetLeftTopBack(), GetRightTopBack(), color);
        Debug.DrawLine(GetRightBottomBack(), GetRightTopBack(), color);

        //Plane connectors
        Debug.DrawLine(GetLeftBottomFront(), GetLeftBottomBack(), color);
        Debug.DrawLine(GetLeftTopFront(), GetLeftTopBack(), color);
        Debug.DrawLine(GetRightBottomFront(), GetRightBottomBack(), color);
        Debug.DrawLine(GetRightTopFront(), GetRightTopBack(), color);

        //Diagonal inner lines
        Debug.DrawLine(GetLeftBottomFront(), GetRightTopBack(), color);
        Debug.DrawLine(GetLeftTopFront(), GetRightBottomBack(), color);
        Debug.DrawLine(GetRightBottomFront(), GetLeftTopBack(), color);
        Debug.DrawLine(GetRightTopFront(), GetLeftBottomBack(), color);

    }

    //Maybe need a new variable, but shouldn't as they should both be references, so I think this approach should work, maybe it does actually copy it instead of a reference, at least if it works, that is probably the explanation
    //Front corners
    public Vector3 GetLeftTopFront()
    {
        center.x = center.x - x / 2;
        center.y = center.y + y / 2;
        center.z = center.z - z / 2;
        return center;
    }

    public Vector3 GetRightTopFront()
    {
        center.x = center.x + x / 2;
        center.y = center.y + y / 2;
        center.z = center.z - z / 2;
        return center;
    }

    public Vector3 GetLeftBottomFront()
    {
        center.x = center.x - x / 2;
        center.y = center.y - y / 2;
        center.z = center.z - z / 2;
        return center;
    }

    public Vector3 GetRightBottomFront()
    {
        center.x = center.x + x / 2;
        center.y = center.y - y / 2;
        center.z = center.z - z / 2;
        return center;
    }

    //Back corners
    public Vector3 GetLeftTopBack()
    {
        center.x = center.x - x / 2;
        center.y = center.y + y / 2;
        center.z = center.z + z / 2;
        return center;
    }

    public Vector3 GetRightTopBack()
    {
        center.x = center.x + x / 2;
        center.y = center.y + y / 2;
        center.z = center.z + z / 2;
        return center;
    }

    public Vector3 GetLeftBottomBack()
    {
        center.x = center.x - x / 2;
        center.y = center.y - y / 2;
        center.z = center.z + z / 2;
        return center;
    }

    public Vector3 GetRightBottomBack()
    {
        center.x = center.x + x / 2;
        center.y = center.y - y / 2;
        center.z = center.z + z / 2;
        return center;
    }
}
