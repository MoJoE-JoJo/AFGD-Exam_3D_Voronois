using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellPlane : MonoBehaviour
{
    public List<GraphVertex> vertices;

    public void DebugDrawCenter()
    {
        var center = CenterPoint();
        Debug.DrawLine(center - new Vector3(0.1f, 0, 0), center + new Vector3(0.1f, 0, 0), Color.green);
        Debug.DrawLine(center - new Vector3(0, 0.1f, 0), center + new Vector3(0, 0.1f, 0), Color.green);
        Debug.DrawLine(center - new Vector3(0, 0, 0.1f), center + new Vector3(0, 0, 0.1f), Color.green);

    }

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
        if (other.vertices.Count != vertices.Count) return false;
        else if (other.CenterPoint() != CenterPoint()) return false;
        else
        {
            for (int i = 0; i < other.vertices.Count; i++)
            {
                if(other.vertices[i] == vertices[0])
                {
                    var right = false;
                    var left = false;
                    if (other.vertices[Mod((i+1), other.vertices.Count)] == vertices[1])
                    {
                        right = true;
                    }
                    if (other.vertices[Mod((i - 1), other.vertices.Count)] == vertices[1])
                    {
                        left = true;
                    }
                    //Check right direction
                    /*
                    if(i == other.vertices.Count-1)
                    {
                        if (other.vertices[0] == vertices[1])
                        {
                            right = true;
                        }
                    }
                    else
                    {
                        if (other.vertices[i + 1] == vertices[1])
                        {
                            right = true;
                        }

                    }

                    if(right == false)
                    {
                        if (i == 0)
                        {
                            if (other.vertices[other.vertices.Count-1] == vertices[1])
                            {
                                left = true;
                            }
                        }
                        else
                        {
                            if (other.vertices[i - 1] == vertices[1])
                            {
                                left = true;
                            }

                        }
                    }
                    */

                    if (!right && !left) return false;
                    else if (right)
                    {
                        var numberOfChecks = vertices.Count;
                        var alreadyChecked = 2;
                        while(alreadyChecked < numberOfChecks)
                        {
                            if (other.vertices[Mod((i + alreadyChecked), other.vertices.Count)] != vertices[alreadyChecked])
                            {
                                return false;
                            }
                            alreadyChecked++;
                        }
                        return true;
                    }
                    else if (left)
                    {
                        var numberOfChecks = vertices.Count;
                        var alreadyChecked = 2;
                        while (alreadyChecked < numberOfChecks)
                        {
                            if (other.vertices[Mod((i - alreadyChecked), other.vertices.Count)] != vertices[alreadyChecked])
                            {
                                return false;
                            }
                            alreadyChecked++;
                        }
                        return true;
                    }
                }
            }
            return false;

        }
        /*
        if (vertices.SequenceEqual(other.vertices)) return true;
        else return false;
        */
    }

    public bool CheckValidPlane()
    {
        for(int i = 0; i <vertices.Count-1; i++)
        {
            for(int j = i+1; j < vertices.Count; j++)
            {
                if (vertices[i] == vertices[j]) return false;
            }
        }
        return true;
    }

    private int Mod(int x, int m)
    {
        int r = x % m;
        return r < 0 ? r + m : r;
    }
}
