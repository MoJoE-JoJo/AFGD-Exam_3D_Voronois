using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloodGraphGenerator : MonoBehaviour
{

    [SerializeField] private MemDivideAndConquer3D DAC;
    [SerializeField] private int combineRange = 2;

    public bool debugDraw = true;
    [Header("Bool run button")]
    public bool run = false;


    private GridPoint _gridPoint;
    private List<Node> nodes;
    private HashSet<GridPoint> visited;
    private Dictionary<GridPoint, Node> pointToPrevnodeDict; //keep track of what was prev when looking at gridpoints.
    private Queue<QueueElement> queue;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void CombineNodes(Node best, Node worst)
    {
        best.Vertexes.UnionWith(worst.Vertexes);
        best.Vertexes.Remove(worst);
        best.Vertexes.Remove(best); //make sure best node does have ref to itself
        // remove reference of worst in other nodes and replace with best
        foreach (var item in best.Vertexes)
        {
            if (item.Vertexes.Contains(worst))
            {
                item.Vertexes.Remove(worst);
                item.Vertexes.Add(best);
            }
        }
        Debug.Log("done combining");
    }

    public class Node
    {
        public GridPoint Point { get; set; }
        public HashSet<Node> Vertexes { get; set; }
        public int Priotity { get; set; }

        public Node(GridPoint p, int prio)
        {
            Point = p;
            Vertexes = new HashSet<Node>();
            Priotity = prio;
        }

        public override string ToString()
        {
            return $"({Point.x}, {Point.y}, {Point.z})";
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
                Node n = (Node)obj;
                return (n.Point == Point);
            }
        }
    }

    public class QueueElement
    {
        public GridPoint gridPoint;
        public Node prevNode;
    }

    // Update is called once per frame
    void Update()
    {
        if (run)
        {
            run = false;
            Run();
        }
    }

    private void Run()
    {
        visited = new HashSet<GridPoint>();
        nodes = new List<Node>();
        pointToPrevnodeDict = new Dictionary<GridPoint, Node>();
        // for each cell created by Divide and conquer, start a flood
        //VCell cell = DAC.cells[0];
        foreach (VCell cell in DAC.cells)
        {
            // === RUN PREFLOOD
            queue = new Queue<QueueElement>();
            queue.Enqueue(new QueueElement { gridPoint = cell.points[0], prevNode = null });
            Node n = RunPreFlood(cell.id);

            Debug.Log("Preflood point found -> " + n.Point.x + " -> " + n.Point.y + " -> " + n.Point.z);

            // === RESET QUEUE AND VISITED, then run flood from the first node found by preflood. 

            queue = new Queue<QueueElement>();
            visited = new HashSet<GridPoint>();
            pointToPrevnodeDict = new Dictionary<GridPoint, Node>();

            var queueElement = new QueueElement { gridPoint = n.Point, prevNode = null };
            queue.Enqueue(queueElement);
            visited.Add(queueElement.gridPoint);

            Flood(cell.id);
            Debug.Log($"Done with cell -> {cell.id}");
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
        foreach (Node item in nodes)
        {
            var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.transform.position = DAC.GridPointCenter(item.Point.x, item.Point.y, item.Point.z);
            ball.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            ball.gameObject.name = count++ + ": " + item.ToString();

            foreach (Node vertex in item.Vertexes)
            {
                var v = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                v.transform.position = DAC.GridPointCenter(vertex.Point.x, vertex.Point.y, vertex.Point.z);
                v.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                v.gameObject.name = vertex.ToString();
                v.transform.parent = ball.transform;
            }
        }
    }

    private Node RunPreFlood(int cellID)
    {
        while (queue.Count != 0)
        {
            QueueElement current = queue.Dequeue();

            Node n = PreFlood(current, cellID);
            if (n != null)
            {
                return n;
            }
        }
        return null;
    }


    private void Flood(int cellID)
    {
        while (queue.Count != 0)
        {
            QueueElement current = queue.Dequeue();

            CheckSurroundingGridPoints(current, cellID);
        }
    }

    /// <summary>
    /// Checks if the point p is withing range of the point gp
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

    private void CheckSurroundingGridPoints(QueueElement queueEle, int cellID)
    {
        // Need to handle a few cases here, dont want to lookup outside the borders of the grid
        GridPoint gp = queueEle.gridPoint;

        // update grid point dictionary 
        pointToPrevnodeDict.Add(gp, queueEle.prevNode);

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

        Node newNode = queueEle.prevNode;
        bool nodeCreated = false;
        int prio = surroundingCells.Values.ToList().Sum();
        if (IsCornerPoint(gp))
        {
            //override prio, to ensure corner point is kept
            prio = int.MaxValue;
            // always create a new node in this case
            nodeCreated = true;
            newNode = new Node(gp, prio);
        }
        else if (IsOnSideLine(gp))
        {
            // if on side line, give a boost to prio
            // create new node if 1 or more different cell types are near
            if (surroundingCells.Keys.Count >= 1)
            {
                prio += 5;
                nodeCreated = true;
                newNode = new Node(gp, prio);
            }
        }
        else if (IsOnSidePlane(gp))
        {
            // if on side plane, give a small boost to prio
            // create new node if 2 or more cells are near
            if (surroundingCells.Keys.Count >= 2)
            {
                prio += 3;
                nodeCreated = true;
                newNode = new Node(gp, prio);
            }
        }
        else
        {
            // need to be at least 3 ids around to be a Node
            if (surroundingCells.Keys.Count >= 3)
            {
                nodeCreated = true;
                newNode = new Node(gp, prio);
            }
        }

        bool addNewNode = true;
        // If a new node is supposed to be created here, look if any nodes are nearby, then combine the nodes into one
        if (nodeCreated)
        {
            List<Node> toDelete = new List<Node>();
            foreach (Node n in nodes)
            {
                GridPoint p = n.Point;
                if (p == gp)
                {
                    continue;
                }
                // if node is nearby
                if (PointInRange(gp, p, combineRange))
                {
                    if (newNode.Priotity > n.Priotity) // the new node is a better candidate, eat the nearby node n, and mark it for deletion
                    {
                        CombineNodes(newNode, n);
                        toDelete.Add(n);

                        //update queue elements that contains n, to point at newNode instead
                        foreach (var item in queue)
                        {
                            if (item.prevNode == n)
                            {
                                item.prevNode = newNode;
                            }

                        }
                        if (pointToPrevnodeDict.TryGetValue(n.Point, out Node nod))
                        {
                            queueEle.prevNode = nod;
                        }

                        // update prevs in dict
                        // THIS IS SUPER INEFFECTIVE!!! if time allows optimize. Could keep a map from int to nodes or something like that
                        foreach (var item in pointToPrevnodeDict.Where(x => x.Value == n).ToList())
                        {
                            pointToPrevnodeDict[item.Key] = newNode;
                        }
                    }
                    else
                    {
                        // let the existing node, eat the new one, set bool to not create a new node

                        nodeCreated = false;
                        newNode = n;
                        addNewNode = false;
                        //queueEle.prevNode.Vertexes.Add(newNode);
                        //newNode.Vertexes.Add(queueEle.prevNode);
                    }
                }
            }
            if (addNewNode)
            {
                if (queueEle.prevNode != null && !toDelete.Contains(queueEle.prevNode))
                {
                    queueEle.prevNode.Vertexes.Add(newNode);
                    newNode.Vertexes.Add(queueEle.prevNode);
                }
                nodes.Add(newNode);
            }
            foreach (Node n in toDelete)
            {
                nodes.Remove(n);
            }
        }

        // add new points to the queue
        foreach (GridPoint item in pointsToQueue)
        {
            var queueElement = new QueueElement { gridPoint = item, prevNode = newNode };
            queue.Enqueue(queueElement);
        }

        // If any elements in the queue are near the new node, update their prev node, since a new node has been created.  
        if (nodeCreated || addNewNode)
        {
            foreach (var element in queue)
            {
                if (element.prevNode == newNode) continue;
                if (PointInRange(newNode.Point, element.gridPoint, combineRange))
                {
                    element.prevNode = newNode;
                }
            }
        }

        // final point cases (when the queue is empty): either we end in a point supposed to be a node, or not.
        if (queue.Count == 0)
        {
            if (nodeCreated)
            {
                //connect up with the newest Node 
                Node newest = nodes[nodes.Count - 1];
                Node secondNewest = nodes[nodes.Count - 2];

                if (newest == queueEle.prevNode)
                {
                    newNode.Vertexes.Add(secondNewest);
                    secondNewest.Vertexes.Add(newNode);
                }
                else
                {
                    newNode.Vertexes.Add(newest);
                    newest.Vertexes.Add(newNode);
                }
            }
            else // not a node point
            {
                // take two newest nodes and create an edge between them
                Node newest = nodes[nodes.Count - 1];
                Node secondNewest = nodes[nodes.Count - 2];

                newest.Vertexes.Add(secondNewest);
                secondNewest.Vertexes.Add(newest);
            }
        }
        // else if pointsToQueue is zero means that the flooding should have "reached itself" so check if we need to combine the two prev from each side
        else if (pointsToQueue.Count == 0)
        {
            //check surronding points for their prevNode and make connection between them. 
            foreach (var item in pointsWithSameCellId)
            {
                if (IsPointNode(item) || IsPointInQueue(item)) continue;

                bool success = pointToPrevnodeDict.TryGetValue(item, out Node n1);
                if (!success) continue;
                if(n1 == null) continue;
                if (queueEle.prevNode == null) continue;

                if (pointToPrevnodeDict[item] != queueEle.prevNode)
                {
                    Node n2 = queueEle.prevNode;

                    Debug.Log($"Making edge between {n1} and {n2}");
                    n1.Vertexes.Add(n2);
                    n2.Vertexes.Add(n1);
                }
            }
        }
    }

    private bool IsPointInQueue(GridPoint gp)
    {
        return queue.Where(x => x.gridPoint == gp).Any();
    }

    private bool IsPointNode(GridPoint gp)
    {
        return nodes.Where(x => x.Point == gp).Any();
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

    private Node PreFlood(QueueElement qe, int cellID)
    {
        // Need to handle a few cases here, dont want to lookup outside the borders of the grid
        GridPoint gp = qe.gridPoint;

        List<GridPoint> pointsToQueue = new List<GridPoint>();
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

        Node newNode = qe.prevNode;
        bool nodeCreated = false;
        int prio = surroundingCells.Values.ToList().Sum();
        if (IsCornerPoint(gp))
        {
            // always create a new node in this case
            newNode = new Node(gp, prio);
        }
        else if (IsOnSideLine(gp))
        {
            // create new node if 1 or more different cell types are near
            if (surroundingCells.Keys.Count >= 1)
            {
                newNode = new Node(gp, prio);
            }
        }
        else if (IsOnSidePlane(gp))
        {
            // create new node if 2 or more cells are near
            if (surroundingCells.Keys.Count >= 2)
            {
                newNode = new Node(gp, prio);
            }
        }
        else
        {
            // need to be at least 3 ids around to be a Node
            if (surroundingCells.Keys.Count >= 3)
            {
                newNode = new Node(gp, prio);
            }
        }

        foreach (GridPoint item in pointsToQueue)
        {
            var queueElement = new QueueElement { gridPoint = item, prevNode = newNode };
            queue.Enqueue(queueElement);
        }

        return newNode;
    }


}
