using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VCell
{
    public int id;
    public Vector3 seed;
    public List<GridPoint> points; 
    public Color color;
}
