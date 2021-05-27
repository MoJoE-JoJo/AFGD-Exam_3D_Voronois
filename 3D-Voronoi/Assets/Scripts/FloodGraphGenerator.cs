using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloodGraphGenerator : MonoBehaviour
{

    private DivideAndConquer DAC; //Remember to set
    private int _combineRange = 5;
    private int resolution;

    private GridPoint _gridPoint;
    private HashSet<GraphVertex> vertices;
    private HashSet<GridPoint> visited;
    private Queue<GridPoint> queue;

    public void CombineNodes(GraphVertex best, GraphVertex worst)
    {
        // add the Cell IDs of the other vertex
        foreach (int item in worst.cellIds)
        {
            best.AddCellID(item);
        }
    }

    public void Init(DivideAndConquer divideAndConquer, int resolution, int combineRange)
    {
        DAC = divideAndConquer;
        this.resolution = resolution;
        _combineRange = combineRange;
        vertices = new HashSet<GraphVertex>();
    }

    public HashSet<GraphVertex> Run()
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

        foreach (GraphVertex vertex in vertices)
        {

            ConnectionFlood(vertex);
            vertex.Position = DAC.GridPointCenter(vertex.GridPoint.x, vertex.GridPoint.y, vertex.GridPoint.z);

            //TODO???
            //foreach (var id in vertex.cellIds)
            //{
            //    VCell cell = DAC.cells[id];
            //    cell.AddRangeOfNeighbors(vertex.cellIds);
            //}
        }

        return vertices;
    }

    int count = 0;
    bool onetime = true;
    public void DebugDraw()
    {
        foreach (GraphVertex item in vertices)
        {
            //Gizmos.DrawSphere(DAC.GridPointCenter(item.Point.x, item.Point.y, item.Point.z), 0.2f);
            //Gizmos.DrawSphere(DAC.GridPointCenter(item.Point.x, item.Point.y, item.Point.z), 0.2f);

            if (onetime)
            {
                var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                ball.transform.position = DAC.GridPointCenter(item.GridPoint.x, item.GridPoint.y, item.GridPoint.z);
                ball.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                ball.gameObject.name = count++ + ": " + item.ToString();

            }

            foreach (GraphVertex vertex in item.connectedVertices)
            {
                Debug.DrawLine(item.Position, vertex.Position, Color.white);
            }
        }
        onetime = false;
    }

    private void VertexFlood(int cellID)
    {
        while (queue.Count != 0)
        {
            GridPoint current = queue.Dequeue();

            FloodFromPointToFindVertices(current, cellID);
        }
    }

    private class QueueElement
    {
        public GridPoint GridPoint { get; set; }
        public bool FoundPoint { get; set; }
        public LinkedList<QueueElement> Chain { get; set; }
    }

    private void ConnectionFlood(GraphVertex rootVertex)
    {


        int x, y, z, id;
        int val = 1;
        // reset queue and add root vertex to visited
        Queue<QueueElement> connectionQueue = new Queue<QueueElement>();
        visited.Clear();
        visited.Add(rootVertex.GridPoint);
        // === initFlood from vertex ===
        GridPoint rootgp = rootVertex.GridPoint;
        for (x = rootgp.x - val; x <= rootgp.x + val; x++)
        {
            for (y = rootgp.y - val; y <= rootgp.y + val; y++)
            {
                for (z = rootgp.z - val; z <= rootgp.z + val; z++)
                {
                    try
                    {
                        id = DAC.grid[x][y][z];
                    }
                    catch (IndexOutOfRangeException) // lazy fix for checking out of bounds
                    {
                        continue;
                    }
                    // if id is unmarked (-1) ignore
                    if (id == -1)
                    {
                        continue;
                    }
                    _gridPoint.x = x;
                    _gridPoint.y = y;
                    _gridPoint.z = z;
                    visited.Add(_gridPoint);

                    if (IgnoreThisPoint(rootgp, x, y, z)) continue;
                    // Enqueue the point with a new Linked list, starting the chain.
                    var queueEle = new QueueElement { GridPoint = _gridPoint, FoundPoint = false, Chain = new LinkedList<QueueElement>() };
                    queueEle.Chain.AddFirst(queueEle);
                    connectionQueue.Enqueue(queueEle);
                    visited.Add(_gridPoint);
                }
            }
        }

        while (connectionQueue.Count != 0)
        {
            // Current queue element in use
            var element = connectionQueue.Dequeue();
            GridPoint gp = element.GridPoint;
            bool found = element.FoundPoint;
            LinkedList<QueueElement> chain = element.Chain;

            if (found)
            {
                continue;
            }
            for (x = gp.x - val; x <= gp.x + val; x++)
            {
                for (y = gp.y - val; y <= gp.y + val; y++)
                {
                    for (z = gp.z - val; z <= gp.z + val; z++)
                    {
                        if (IgnoreThisPoint(gp, x, y, z)) continue;

                        try
                        {
                            id = DAC.grid[x][y][z];
                        }
                        catch (IndexOutOfRangeException) // lazy fix for checking out of bounds
                        {
                            continue;
                        }
                        // if id is unmarked (-1) ignore
                        if (id == -1)
                        {
                            continue;
                        }

                        _gridPoint.x = x;
                        _gridPoint.y = y;
                        _gridPoint.z = z;
                        if (!visited.Contains(_gridPoint))
                        {

                            GraphVertex foundVertex;
                            if (IsPointVertex(_gridPoint, rootVertex, out foundVertex))
                            {
                                //if point is a vertex then mark found as true
                                found = true;
                                //add a connection to origin vertex
                                rootVertex.AddConnection(foundVertex);
                                foundVertex.AddConnection(rootVertex);

                                #region OLD
                                //if (AreOtherVerticesWithinrange(_gridPoint, _combineRange * 2, rootVertex, out _))
                                //{
                                //    offset = 0;
                                //}

                                //// if on the side of cube
                                //if (IsVertexInsideCube(foundVertex))
                                //{
                                //    offset = 5;
                                //}

                                //int value = 7;
                                ////check if any of the elements in the queue are within range of this vertex point, if yes mark them as found point so the search don't continue. 
                                //foreach (var ele in connectionQueue)
                                //{
                                //    if (PointInRange(_gridPoint, ele.GridPoint, value))
                                //    {
                                //        ele.FoundPoint = true;
                                //    }
                                //}
                                #endregion

                                // go through the chain and mark everything as found
                                foreach (var item in chain)
                                {
                                    item.FoundPoint = true;
                                }
                            }
                            else // not a vertex point, add to queue to continue search
                            {
                                visited.Add(_gridPoint);
                                var newQueue = new QueueElement { GridPoint = _gridPoint, FoundPoint = found };

                                var firstElement = chain.First;

                                ////check if the queue has split up into different direction, by seeing if the new point and the lastest point are adjecent or not
                                //if (!ArePointsAdjacent(_gridPoint, lastElement.Value.GridPoint))
                                //{
                                //    //Chain has split, create a new chain (reset the linked list.)
                                //    chain = new LinkedList<QueueElement>();


                                // split the chain??
                                if (!PointInRange(_gridPoint, firstElement.Value.GridPoint, 5))
                                {
                                    //Chain has split, create a new chain(reset the linked list.)
                                    chain = new LinkedList<QueueElement>();
                                }
                                newQueue.Chain = chain;
                                newQueue.Chain.AddFirst(newQueue);

                                connectionQueue.Enqueue(newQueue);
                            }
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// Flood search from a point and find vertices where the a certain amount of cells lie nest to one another. If vertices are very close, combine them into one vertex.
    /// </summary>
    /// <param name="gp">Starting search GridPoint</param>
    /// <param name="cellID">The current cell id that to generate/find vertices for</param>
    private void FloodFromPointToFindVertices(GridPoint gp, int cellID)
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

                    //if (IgnoreThisPoint(gp, x, y, z)) continue;
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
                        _gridPoint.x = x;
                        _gridPoint.y = y;
                        _gridPoint.z = z;
                        // if gridpoint not visited, add to queue and visited
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

        bool createNewVertex = false;
        int prio = surroundingCells.Values.ToList().Sum();
        if (IsCornerPoint(gp))
        {
            //override prio, to ensure corner point is kept
            prio = int.MaxValue;
            // always create a new node in this case
            createNewVertex = true;
        }
        else if (IsOnSideLine(gp))
        {
            // create new node if 1 or more different cell types are near
            if (surroundingCells.Keys.Count >= 1)
            {
                // if on side line, give a boost to prio
                prio += 7 * _combineRange;
                createNewVertex = true;
            }
        }
        else if (IsOnSidePlane(gp))
        {
            // create new node if 2 or more cells are near
            if (surroundingCells.Keys.Count >= 2)
            {
                // if on side plane, give a small boost to prio
                prio += 5 * _combineRange;
                createNewVertex = true;
            }
        }
        else
        {
            // need to be at least 3 ids around to be a Node
            if (surroundingCells.Keys.Count >= 3)
            {
                createNewVertex = true;
            }
        }

        // If a new node is supposed to be created here, look if any nodes are nearby, then combine the nodes into one
        if (createNewVertex)
        {
            // Create a new 
            GraphVertex newVertex = new GraphVertex(gp, prio, cellID);
            foreach (int item in surroundingCells.Keys)
            {
                newVertex.AddCellID(item);
            }

            List<GraphVertex> currentVertices;
            List<GraphVertex> toDelete = new List<GraphVertex>();
            if (FindNearbyVertex(newVertex.GridPoint, _combineRange, out currentVertices))
            {
                foreach (var vertex in currentVertices)
                {
                    if (vertex.GridPoint != gp)
                    {
                        if (newVertex.Priotity > vertex.Priotity) // the new node is a better candidate, eat the nearby node n, and mark it for deletion
                        {
                            CombineNodes(newVertex, vertex);
                            toDelete.Add(vertex);
                        }
                        else
                        {
                            // let the existing node, eat the new one, set bool to not create a new node
                            CombineNodes(vertex, newVertex);
                            createNewVertex = false; // mark to not add the new vertex
                            newVertex = vertex;
                        }
                    }
                }
            }
            if (createNewVertex)
            {
                vertices.Add(newVertex);
            }
            foreach (var item in toDelete)
            {
                vertices.Remove(item);
            }
        }

        // add new points to the queue
        foreach (GridPoint item in pointsToQueue)
        {
            queue.Enqueue(item);
        }
    }
    /// ========== HELPER METHODS ==========
    #region HELPER METHODS
    /// <summary>
    /// Returns true if the x y z results in being diagonnally placed compared to gp
    /// </summary>
    /// <param name="gp"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private bool IgnoreThisPoint(GridPoint gp, int x, int y, int z)
    {
        if (gp.x - 1 == x && gp.y - 1 == y) return true;
        else if (gp.x == x && gp.y == y && gp.z == z) return true;
        else if (gp.x - 1 == x && gp.z - 1 == z) return true;
        else if (gp.y - 1 == y && gp.z - 1 == z) return true;
        else if (gp.y - 1 == y && gp.z + 1 == z) return true;
        else if (gp.y + 1 == y && gp.z + 1 == z) return true;
        else if (gp.y + 1 == y && gp.x + 1 == x) return true;
        else if (gp.y + 1 == y && gp.x - 1 == x) return true;
        else if (gp.x + 1 == x && gp.z - 1 == z) return true;
        else if (gp.x + 1 == x && gp.z + 1 == z) return true;
        else if (gp.x - 1 == x && gp.z + 1 == z) return true;
        else if (gp.y + 1 == y && gp.z - 1 == z) return true;
        else if (gp.y - 1 == y && gp.x + 1 == x) return true;
        return false;
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

    private bool IsVertexInsideCube(GraphVertex foundVertex)
    {
        return !IsOnSidePlane(foundVertex.GridPoint);
    }

    private bool ArePointsAdjacent(GridPoint a, GridPoint b)
    {
        return PointInRange(a, b, 2);
    }

    private bool DoesVerticesSharesCell(GraphVertex rootVertex, GraphVertex foundVertex)
    {
        return rootVertex.cellIds.Union(foundVertex.cellIds).Any();
    }

    private bool FindNearbyVertex(GridPoint gp, int combineRange, out List<GraphVertex> vertexes, GraphVertex rootVertex = null)
    {
        var list = new List<GraphVertex>();
        foreach (var item in vertices)
        {
            var p = item.GridPoint;
            if (p == gp) continue;
            if (rootVertex != null && p == rootVertex.GridPoint) continue;
            if (p.x >= gp.x - combineRange && p.x <= gp.x + combineRange &&
                   p.y >= gp.y - combineRange && p.y <= gp.y + combineRange &&
                   p.z >= gp.z - combineRange && p.z <= gp.z + combineRange)
            {
                list.Add(item);
            }
        }
        vertexes = list;
        return vertexes.Count > 0;
        return false;
    }

    private bool IsPointInQueue(GridPoint gp)
    {
        return queue.Where(x => x == gp).Any();
    }

    private bool IsPointVertex(GridPoint gp, GraphVertex rootVertex, out GraphVertex foundVertex)
    {
        foundVertex = vertices.Where(x => x.GridPoint == gp).FirstOrDefault();
        return foundVertex != null && foundVertex != rootVertex;
    }


    private bool IsCornerPoint(GridPoint p)
    {
        int maxIndex = resolution - 1;
        // if all index are either 0 or max value, then the point must be a corner point
        return (p.x == 0 || p.x == maxIndex) && (p.y == 0 || p.y == maxIndex) && (p.z == 0 || p.z == maxIndex);
    }

    private bool IsOnSideLine(GridPoint p)
    {
        int maxIndex = resolution - 1;
        //two coords need to be either 0 or max for the point to allign with a corner 
        int count = 0;

        if (p.x == 0 || p.x == maxIndex) count++;
        if (p.y == 0 || p.y == maxIndex) count++;
        if (p.z == 0 || p.z == maxIndex) count++;

        return count == 2;
    }

    private bool IsOnSidePlane(GridPoint p)
    {
        int maxIndex = resolution - 1;
        //If a single of the coords are either 0 or max value, then the point must be on a side
        return p.x == 0 || p.x == maxIndex || p.y == 0 || p.y == maxIndex || p.z == 0 || p.z == maxIndex;

    }
    #endregion


}
