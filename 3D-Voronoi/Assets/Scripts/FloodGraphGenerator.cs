using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloodGraphGenerator : MonoBehaviour
{

    private DivideAndConquer DAC; //Remember to set
    private int combineRange = 2;
    private Vector3Int resolution;

    private GridPoint _gridPoint;
    private HashSet<GraphVertex> vertices;
    private HashSet<GridPoint> visited;
    private Queue<GridPoint> queue;

    /// <summary>
    /// Takes the cellIDs stored in the worst and adds them the best Vertex
    /// </summary>
    /// <param name="best"></param>
    /// <param name="worst">The best vertex</param>
    public void CombineVertices(GraphVertex best, GraphVertex worst)
    {
        foreach (int item in worst.cellIds)
        {
            best.AddCellID(item);
        }
    }

    public void Init(DivideAndConquer divideAndConquer, Vector3Int resolution)
    {
        DAC = divideAndConquer;
        this.resolution = resolution;
        vertices = new HashSet<GraphVertex>();
    }

    public HashSet<GraphVertex> Run()
    {
        visited = new HashSet<GridPoint>();
        vertices = new HashSet<GraphVertex>();
        // for each cell created by Divide and conquer, start a flood for finding Vertices
        foreach (VCell cell in DAC.cells)
        {
            //reset queue when starting on a new cell

            queue = new Queue<GridPoint>();

            queue.Enqueue(cell.points[0]);
            visited.Add(cell.points[0]);

            VertexFlood(cell.id);
        }

        foreach (GraphVertex vertex in vertices)
        {
            ConnectionFlood(vertex);
            vertex.Position = DAC.GridPointCenter(vertex.GridPoint.x, vertex.GridPoint.y, vertex.GridPoint.z);
        }

        return vertices;
    }

    public void DebugDraw()
    {
        foreach (GraphVertex item in vertices)
        {
            //Gizmos.DrawSphere(DAC.GridPointCenter(item.Point.x, item.Point.y, item.Point.z), 0.2f);
            Gizmos.DrawSphere(DAC.GridPointCenter(item.GridPoint.x, item.GridPoint.y, item.GridPoint.z), 0.2f);
            /*
            var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.transform.position = DAC.GridPointCenter(item.Point.x, item.Point.y, item.Point.z);
            ball.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            ball.gameObject.name = count++ + ": " + item.ToString();
            */
            foreach (GraphVertex vertex in item.connectedVertices)
            {
                Debug.DrawLine(item.Position, vertex.Position, Color.white);
            }
        }
    }

    private void VertexFlood(int cellID)
    {
        while (queue.Count != 0)
        {
            GridPoint current = queue.Dequeue();
            CellFloodToFindVertices(current, cellID);
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

                    if (IsDiagonal(rootgp, x, y, z)) continue;
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
                        if (IsDiagonal(gp, x, y, z)) continue;
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
                            if (IsPointVertex(_gridPoint, rootVertex, out GraphVertex foundVertex))
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

                                // go through the chain and mark everything as found, so the chain stops searching
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

                                // if the newQueue point are not near the head of the chain, assume the chain has "gone down two paths",
                                // then split the chain by creating a new one from here. 
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
    /// Check if the given point should be a new vertex or not, by checking the surrounding points for their cell ID. 
    /// If the surrounding points share the same, cellID add them to the queue as to be analyzed next. 
    /// </summary>
    /// <param name="gp">Starting search GridPoint</param>
    /// <param name="cellID">The current cell id that to generate/find vertices for</param>
    private void CellFloodToFindVertices(GridPoint gp, int cellID)
    {
        Dictionary<int, int> surroundingCells = new Dictionary<int, int>();

        // === scan surrounding gridpoints for what their ID is and fill Dictionary ===
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
                    catch (IndexOutOfRangeException) // lazy way to ignore out of bounds
                    {
                        continue;
                    }
                    if (id == -1)
                    {
                        // ignore unmarked points
                        continue;
                    }
                    else if (id == cellID) // if same cellID as currently flooding from
                    {
                        _gridPoint.x = x;
                        _gridPoint.y = y;
                        _gridPoint.z = z;
                        // if gridpoint not visited, add to queue and visited
                        if (!visited.Contains(_gridPoint))
                        {
                            visited.Add(_gridPoint);
                            queue.Enqueue(_gridPoint);
                        }
                    }
                    else // not same cellID, add to dictionary or count up if already contains
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
        //count a priority based on the count of the surrounding cells 
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
                prio += 10;
                createNewVertex = true;
            }
        }
        else if (IsOnSidePlane(gp))
        {
            // create new node if 2 or more cells are near
            if (surroundingCells.Keys.Count >= 2)
            {
                // if on side plane, give a small boost to prio
                prio += 5;
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

        // If a new Vertex is supposed to be created here, look if any vertices are nearby, if there are, combine them
        if (createNewVertex)
        {
            // Create a new Vertex
            GraphVertex newVertex = new GraphVertex(gp, prio, cellID);
            foreach (int item in surroundingCells.Keys)
            {
                newVertex.AddCellID(item);
            }

            List<GraphVertex> toDelete = new List<GraphVertex>();
            if (FindNearbyVertices(newVertex.GridPoint, combineRange, out List<GraphVertex> nearbyVertices))
            {
                foreach (var nearbyVertex in nearbyVertices)
                {

                    if (newVertex.Priotity > nearbyVertex.Priotity) // the new Vertex is a better candidate, eat the nearby Vertex and mark it for deletion
                    {
                        CombineVertices(newVertex, nearbyVertex);
                        toDelete.Add(nearbyVertex);
                    }
                    else // let the existing node, eat the new one, swap bool to not create a new node
                    {
                        CombineVertices(nearbyVertex, newVertex);
                        createNewVertex = false; // mark to not add the new vertex
                        newVertex = nearbyVertex;
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
    }
    /// ========== HELPER METHODS ==========
    #region HELPER METHODS
    /// <summary>
    /// Returns true if the x y z results in being a diagonnally placed point compared to gp
    /// </summary>
    /// <param name="gp"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private bool IsDiagonal(GridPoint gp, int x, int y, int z)
    {
        if (gp.x == x && gp.y == y && gp.z == z) return true; //ignore if same coords
        else if (gp.x - 1 == x && gp.y - 1 == y) return true;
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

    private bool IsVertexInsideCube(GraphVertex vertex)
    {
        return !IsOnSidePlane(vertex.GridPoint);
    }

    private bool ArePointsAdjacent(GridPoint a, GridPoint b)
    {
        return PointInRange(a, b, 2);
    }

    private bool DoesVerticesSharesCell(GraphVertex rootVertex, GraphVertex foundVertex)
    {
        return rootVertex.cellIds.Union(foundVertex.cellIds).Any();
    }

    /// <summary>
    /// Goes though the already found vertices and checks if they are close to the provided GridPoint gp. It returns True if any vertices are near the point. 
    /// It also returns the list found in the out variable vertices
    /// </summary>
    /// <param name="gp"></param>
    /// <param name="combineRange"></param>
    /// <param name="vertices"></param>
    /// <param name="rootVertex"></param>
    /// <returns></returns>
    private bool FindNearbyVertices(GridPoint gp, int combineRange, out List<GraphVertex> vertices, GraphVertex rootVertex = null)
    {
        var list = new List<GraphVertex>();
        foreach (var item in this.vertices)
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
        vertices = list;
        return vertices.Count > 0;
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
        int maxIndexX = resolution.x - 1;
        int maxIndexY = resolution.y - 1;
        int maxIndexZ = resolution.z - 1;

        // if all index are either 0 or max value, then the point must be a corner point
        return (p.x == 0 || p.x == maxIndexX) && (p.y == 0 || p.y == maxIndexY) && (p.z == 0 || p.z == maxIndexZ);
    }

    private bool IsOnSideLine(GridPoint p)
    {
        int maxIndexX = resolution.x - 1;
        int maxIndexY = resolution.y - 1;
        int maxIndexZ = resolution.z - 1;

        //two coords need to be either 0 or max for the point to allign with a corner 
        int count = 0;

        if (p.x == 0 || p.x == maxIndexX) count++;
        if (p.y == 0 || p.y == maxIndexY) count++;
        if (p.z == 0 || p.z == maxIndexZ) count++;

        return count == 2;
    }

    private bool IsOnSidePlane(GridPoint p)
    {
        int maxIndexX = resolution.x - 1;
        int maxIndexY = resolution.y - 1;
        int maxIndexZ = resolution.z - 1;

        //If a single of the coords are either 0 or max value, then the point must be on a side
        return p.x == 0 || p.x == maxIndexX || p.y == 0 || p.y == maxIndexY || p.z == 0 || p.z == maxIndexZ;

    }
    #endregion


}
