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
        var vec = center;
        vec.x = center.x - x / 2;
        vec.y = center.y + y / 2;
        vec.z = center.z - z / 2;
        return vec;
    }

    public Vector3 GetRightTopFront()
    {
        var vec = center;
        vec.x = center.x + x / 2;
        vec.y = center.y + y / 2;
        vec.z = center.z - z / 2;
        return vec;
    }

    public Vector3 GetLeftBottomFront()
    {
        var vec = center;
        vec.x = center.x - x / 2;
        vec.y = center.y - y / 2;
        vec.z = center.z - z / 2;
        return vec;
    }

    public Vector3 GetRightBottomFront()
    {
        var vec = center;
        vec.x = center.x + x / 2;
        vec.y = center.y - y / 2;
        vec.z = center.z - z / 2;
        return vec;
    }

    //Back corners
    public Vector3 GetLeftTopBack()
    {
        var vec = center;
        vec.x = center.x - x / 2;
        vec.y = center.y + y / 2;
        vec.z = center.z + z / 2;
        return vec;
    }

    public Vector3 GetRightTopBack()
    {
        var vec = center;
        vec.x = center.x + x / 2;
        vec.y = center.y + y / 2;
        vec.z = center.z + z / 2;
        return vec;
    }

    public Vector3 GetLeftBottomBack()
    {
        var vec = center;
        vec.x = center.x - x / 2;
        vec.y = center.y - y / 2;
        vec.z = center.z + z / 2;
        return vec;
    }

    public Vector3 GetRightBottomBack()
    {
        var vec = center;
        vec.x = center.x + x / 2;
        vec.y = center.y - y / 2;
        vec.z = center.z + z / 2;
        return vec;
    }
}
