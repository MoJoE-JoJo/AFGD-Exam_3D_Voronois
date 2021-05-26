using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphVertex
{
    public List<int> cellIds;
    public List<GraphVertex> connectedVertices;
    public Vector3 Position { get; set; }
    public GridPoint Point { get; set; }
    //private MemDivideAndConquer3D memDnC;
    public int Priotity { get; set; }

    public GraphVertex(GridPoint p, int prio, int cellID)
    {
        Point = p;
        connectedVertices = new List<GraphVertex>();
        cellIds = new List<int> { cellID };
        Priotity = prio;
    }

    //// Start is called before the first frame update
    //void Start()
    //{
    //    Position = transform.position;
    //    memDnC = GameObject.FindGameObjectWithTag("DivideAndConquer").GetComponent<MemDivideAndConquer3D>();
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    // These function is to make the public lists work like a set
    public void AddCellID(int id)
    {
        if (!cellIds.Contains(id)) cellIds.Add(id);
    }

    public void AddConnection(GraphVertex v)
    {
        if (!connectedVertices.Contains(v)) connectedVertices.Add(v);
    }

    //public void DebugDraw()
    //{
    //    for (int vertex = 0; vertex < connectedVertices.Count; vertex++)
    //    {
    //        for (int cellIdIndex = 0; cellIdIndex < cellIds.Count; cellIdIndex++)
    //        {
    //            if (connectedVertices[vertex].cellIds.Contains(cellIds[cellIdIndex]))
    //            {
    //                Debug.DrawLine(Position, connectedVertices[vertex].Position, memDnC.cells[cellIds[cellIdIndex]].color);
    //            }
    //        }
    //    }
    //}

    public override string ToString()
    {
        return $"({Point.x}, {Point.y}, {Point.z}) CellCount: {cellIds.Count}";
    }

    public override bool Equals(object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            // compare the points in their gridpoint coordinates
            GraphVertex n = (GraphVertex)obj;
            return (n.Point == Point);
        }
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

}
