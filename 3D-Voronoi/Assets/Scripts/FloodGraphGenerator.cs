using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloodGraphGenerator : MonoBehaviour
{

    [SerializeField] private MemDivideAndConquer3D DAC;
    [SerializeField] private int combineRange = 2;
    public PlaneGenerator planeGenerator;


    public bool debugDraw = true;
    [Header("Bool run button")]
    public bool run = false;


    private GridPoint _gridPoint;
    private HashSet<GraphVertex> vertices;
    private HashSet<GridPoint> visited;
    private Queue<GridPoint> queue;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void CombineNodes(GraphVertex best, GraphVertex worst)
    {
        // Take the Cell IDs of the other vertex
        foreach (int item in worst.cellIds)
        {
            best.AddCellID(item);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (run)
        {
            run = false;
            Run();
            planeGenerator.cellVertices = vertices.ToList();
        }
    }

    private void Run()
    {
        visited = new HashSet<GridPoint>();
        vertices = new HashSet<GraphVertex>();
        // for each cell created by Divide and conquer, start a flood
        foreach (VCell cell in DAC.cells)
        {
            // ===reset queue and visited when starting on a new cell

            queue = new Queue<GridPoint>();
            visited = new HashSet<GridPoint>();

            queue.Enqueue(cell.points[0]);
            visited.Add(cell.points[0]);

            VertexFlood(cell.id);
            Debug.Log($"Done with cell -> {cell.id}");
        }

        foreach (GraphVertex item in vertices)
        {
            ConnectionFlood(item);
            item.Position = DAC.GridPointCenter(item.Point.x, item.Point.y, item.Point.z);
        }

        // DEBUG: draw balls for the nodes and small balls for "edges"
        if (debugDraw)
        {
            DebugDraw();
        }
    }

    private void DebugDraw()
    {
        int count = 0;
        foreach (GraphVertex item in vertices)
        {
            var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.transform.position = DAC.GridPointCenter(item.Point.x, item.Point.y, item.Point.z);
            ball.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            ball.gameObject.name = count++ + ": " + item.ToString();

            foreach (GraphVertex vertex in item.connectedVertices)
            {
                var v = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                v.transform.position = DAC.GridPointCenter(vertex.Point.x, vertex.Point.y, vertex.Point.z);
                v.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                v.gameObject.name = vertex.ToString();
                v.transform.parent = ball.transform;
            }
        }
    }

    private void VertexFlood(int cellID)
    {
        while (queue.Count != 0)
        {
            GridPoint current = queue.Dequeue();

            CheckSurroundingGridPoints(current, cellID);
        }
    }

    private class QueueElement
    {
        public GridPoint GridPoint { get; set; }
        public bool FoundPoint { get; set; }
    }

    private void ConnectionFlood(GraphVertex item)
    {
        var ConnectionQueue = new Queue<QueueElement>();
        ConnectionQueue.Enqueue(new QueueElement { GridPoint = item.Point, FoundPoint = false });
        visited = new HashSet<GridPoint>();
        visited.Add(item.Point);

        int x, y, z, id;
        while (ConnectionQueue.Count != 0)
        {
            var element = ConnectionQueue.Dequeue();
            GridPoint gp = element.GridPoint;
            bool found = element.FoundPoint;

            if (found)
            {
                continue;
            }
            for (x = gp.x - 1; x <= gp.x + 1; x++)
            {
                for (y = gp.y - 1; y <= gp.y + 1; y++)
                {
                    for (z = gp.z - 1; z <= gp.z + 1; z++)
                    {
                        try
                        {
                            id = DAC.grid[x][y][z];
                        }
                        catch (IndexOutOfRangeException)
                        {
                            continue;
                        }
                        if (id == -1) //ignore this one
                        {
                            continue;
                        }
                        _gridPoint.x = x;
                        _gridPoint.y = y;
                        _gridPoint.z = z;
                        if (!visited.Contains(_gridPoint))
                        {
                            if (IsPointNode(_gridPoint))
                            {

                                GraphVertex n = vertices.Where(p => p.Point == _gridPoint).First();
                                found = true;
                                //add connection to origin
                                item.AddConnection(n);

                                foreach (var ele in ConnectionQueue)
                                {
                                    if (PointInRange(_gridPoint, ele.GridPoint, combineRange + 1))
                                    {
                                        ele.FoundPoint = true;
                                    }
                                }
                            }
                            else
                            {
                                visited.Add(_gridPoint);
                                ConnectionQueue.Enqueue(new QueueElement { GridPoint = _gridPoint, FoundPoint = found });
                            }
                        }
                    }
                }
            }
        }
    }

    private void CheckSurroundingGridPoints(GridPoint gp, int cellID)
    {
        List<GridPoint> pointsToQueue = new List<GridPoint>();
        List<GridPoint> pointsWithSameCellId = new List<GridPoint>();
        Dictionary<int, int> surroundingCells = new Dictionary<int, int>();

        // === Step 1: scan surround gridpoints for what their ID is 
        int x, y, z, id;
        for (x = gp.x - 1; x <= gp.x + 1; x++)
        {
            for (y = gp.y - 1; y <= gp.y + 1; y++)
            {
                for (z = gp.z - 1; z <= gp.z + 1; z++)
                {
                    try
                    {
                        id = DAC.grid[x][y][z];
                    }
                    catch (IndexOutOfRangeException)
                    {
                        continue;
                    }

                    if (id == -1)
                    {
                        //ignore this one
                        continue;
                    }
                    else if (id == cellID) // same cellID as currently flooding from
                    {
                        // if gridpoint not visited, add to queue and visited
                        _gridPoint.x = x;
                        _gridPoint.y = y;
                        _gridPoint.z = z;
                        if (!visited.Contains(_gridPoint))
                        {
                            visited.Add(_gridPoint);
                            pointsToQueue.Add(_gridPoint);
                        }
                        else
                        {
                            pointsWithSameCellId.Add(_gridPoint);
                        }
                    }
                    else
                    {
                        if (surroundingCells.ContainsKey(id))
                        {
                            surroundingCells[id] = surroundingCells[id] + 1;
                        }
                        else
                        {
                            surroundingCells.Add(id, 1);
                        }
                    }
                }
            }
        }

        // === Step 2: based on how many found surrounding cells, do stuff
        // this also depends on if the current point is a corner or sidepoint. 
        GraphVertex newNode = null;
        bool createNewNode = false;
        int prio = surroundingCells.Values.ToList().Sum();
        if (IsCornerPoint(gp))
        {
            //override prio, to ensure corner point is kept
            prio = int.MaxValue;
            // always create a new node in this case
            createNewNode = true;
        }
        else if (IsOnSideLine(gp))
        {
            // if on side line, give a boost to prio
            // create new node if 1 or more different cell types are near
            if (surroundingCells.Keys.Count >= 1)
            {
                prio += 5;
                createNewNode = true;
            }
        }
        else if (IsOnSidePlane(gp))
        {
            // if on side plane, give a small boost to prio
            // create new node if 2 or more cells are near
            if (surroundingCells.Keys.Count >= 2)
            {
                prio += 3;
                createNewNode = true;
            }
        }
        else
        {
            // need to be at least 3 ids around to be a Node
            if (surroundingCells.Keys.Count >= 3)
            {
                createNewNode = true;
            }
        }

        if (createNewNode)
        {
            newNode = new GraphVertex(gp, prio, cellID);
        }

        // If a new node is supposed to be created here, look if any nodes are nearby, then combine the nodes into one
        if (createNewNode)
        {
            List<GraphVertex> toDelete = new List<GraphVertex>();
            foreach (GraphVertex vertex in vertices)
            {
                GridPoint p = vertex.Point;
                if (p == gp)
                {
                    continue;
                }
                // if node is nearby
                if (PointInRange(gp, p, combineRange))
                {
                    if (newNode.Priotity > vertex.Priotity) // the new node is a better candidate, eat the nearby node n, and mark it for deletion
                    {
                        CombineNodes(newNode, vertex);
                        toDelete.Add(vertex);
                    }
                    else
                    {
                        // let the existing node, eat the new one, set bool to not create a new node
                        CombineNodes(vertex, newNode);

                        createNewNode = false;
                        newNode = vertex;
                    }
                }
            }
            if (createNewNode)
            {
                vertices.Add(newNode);
            }
            foreach (GraphVertex n in toDelete)
            {
                vertices.Remove(n);
            }
        }

        // add new points to the queue
        foreach (GridPoint item in pointsToQueue)
        {
            queue.Enqueue(item);
        }
    }

    /// <summary>
    /// Checks if the point p is within the searchrange of the point gp
    /// </summary>
    /// <param name="gp"></param>
    /// <param name="p"></param>
    /// <param name="searchRange"></param>
    /// <returns></returns>
    private bool PointInRange(GridPoint gp, GridPoint p, int searchRange)
    {
        return p.x >= gp.x - searchRange && p.x <= gp.x + searchRange &&
                    p.y >= gp.y - searchRange && p.y <= gp.y + searchRange &&
                    p.z >= gp.z - searchRange && p.z <= gp.z + searchRange;
    }

    private bool IsPointInQueue(GridPoint gp)
    {
        return queue.Where(x => x == gp).Any();
    }

    private bool IsPointNode(GridPoint gp)
    {
        return vertices.Where(x => x.Point == gp).Any();
    }


    private bool IsCornerPoint(GridPoint p)
    {
        int maxIndex = DAC.resolution - 1;
        // if all index are either 0 or max value, then the point must be a corner point
        return (p.x == 0 || p.x == maxIndex) && (p.y == 0 || p.y == maxIndex) && (p.z == 0 || p.z == maxIndex);
    }

    private bool IsOnSideLine(GridPoint p)
    {
        int maxIndex = DAC.resolution - 1;
        //two coords need to be either 0 or max for the point to allign with a corner 
        int count = 0;

        if (p.x == 0 || p.x == maxIndex) count++;
        if (p.y == 0 || p.y == maxIndex) count++;
        if (p.z == 0 || p.z == maxIndex) count++;

        return count == 2;
    }

    private bool IsOnSidePlane(GridPoint p)
    {
        int maxIndex = DAC.resolution - 1;
        //If a single of the coords are either 0 or max value, then the point must be on a side
        return p.x == 0 || p.x == maxIndex || p.y == 0 || p.y == maxIndex || p.z == 0 || p.z == maxIndex;

    }
}
