using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct GridPoint
{
    public int x;
    public int y;
    public int z;


    public static bool operator ==(GridPoint lhs, GridPoint rhs)
    {
        return (lhs.x == rhs.x) && (lhs.y == rhs.y) && (lhs.z == rhs.z);
    }

    public static bool operator !=(GridPoint lhs, GridPoint rhs)
    {
        return (lhs.x != rhs.x) || (lhs.y != rhs.y) || (lhs.z != rhs.z);
    }


    //public override bool Equals(object obj)
    //{
    //    //Check for null and compare run-time types.
    //    if ((obj == null) || !this.GetType().Equals(obj.GetType()))
    //    {
    //        return false;
    //    }
    //    else
    //    {
    //        GridPoint p = (GridPoint)obj;
    //        return (x == p.x) && (y == p.y) && (z == p.z);
    //    }
    //}
}
