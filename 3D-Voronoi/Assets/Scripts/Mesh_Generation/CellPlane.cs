using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellPlane : MonoBehaviour
{
    public List<GraphVertex> vertices;

    public Vector3 CenterPoint()
    {
        Vector3 center = new Vector3();
        foreach(GraphVertex gv in vertices)
        {
            center += gv.position;
        }
        center /= vertices.Count;
        return center;
    }

    public bool PlaneEquals(CellPlane other)
    {
        if (vertices.SequenceEqual(other.vertices)) return true;
        else return false;
    }
}
