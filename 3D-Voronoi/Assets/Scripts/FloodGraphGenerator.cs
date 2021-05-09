using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FloodGraphGenerator : MonoBehaviour
{

    [SerializeField] private MemDivideAndConquer3D DAC;
    [SerializeField] private int combineNodeSearchrange = 2;


    [Header("Bool run button")]
    public bool run = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    public class Node
    {
        public GridPoint Point { get; set; }
        public List<Edge> Edges { get; set; }
        public int Priotity { get; set; }

        public Node(GridPoint p, int prio)
        {
            Point = p;
            Edges = new List<Edge>();
            Priotity = prio;
        }

        public void Combine(Node n)
        {
            foreach (Edge item in n.Edges)
            {
                if (item.To == n)
                {
                    item.To = this;
                }
                else if (item.From == n)
                {
                    item.From = this;
                }
                Edges.Add(item);
            }
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

    public class Edge
    {
        public int CellID { get; set; }
        public Node From { get; set; }
        public Node To { get; set; }
    }

    public struct QueueElement
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

    private List<Node> nodes;
    private HashSet<GridPoint> visited;
    private Queue<QueueElement> queue;

    private void Run()
    {
        visited = new HashSet<GridPoint>();
        nodes = new List<Node>();
        // for each cell created by Divide and conquer, start a flood
        foreach (VCell cell in DAC.cells)
        {
            // reset queue and run flood
            queue = new Queue<QueueElement>();
            queue.Enqueue(new QueueElement { gridPoint = cell.points[0], prevNode = null });
            visited.Add(cell.points[0]);
            Flood(cell.id);

            Debug.Log($"Done with cell -> {cell.id}");
        }

        // DEBUG: draw balls for the nodes
        int count = 0;
        foreach (Node item in nodes)
        {
            var ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            ball.transform.position = DAC.GridPointCenter(item.Point.x, item.Point.y, item.Point.z);
            ball.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            ball.gameObject.name = "" + count++;
        }

    }


    private void Flood(int cellID)
    {
        while (queue.Count != 0)
        {
            QueueElement current = queue.Dequeue();

            CheckSurroundingGridPoints(current, cellID);
        }
    }

    private void CheckSurroundingGridPoints(QueueElement queueElement, int cellID)
    {
        // Need to handle a few cases here, dont want to lookup outside the borders of the grid
        GridPoint gp = queueElement.gridPoint;

        List<GridPoint> pointsToQueue = new List<GridPoint>();
        Dictionary<int, int> surroundingCells = new Dictionary<int, int>();

        // === Step 1: scan surround gridpoints for what there ID is 

        for (int x = gp.x - 1; x <= gp.x + 1; x++)
        {
            for (int y = gp.y - 1; y <= gp.y + 1; y++)
            {
                for (int z = gp.z - 1; z <= gp.z + 1; z++)
                {
                    int id;
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
                        GridPoint newGP = new GridPoint { x = x, y = y, z = z };
                        if (!visited.Contains(newGP))
                        {
                            visited.Add(newGP);
                            pointsToQueue.Add(newGP);
                            //queue.Enqueue(new QueueElement { gridPoint = newGP, queueElement.prevNode });
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

        Node node = queueElement.prevNode;
        bool nodeCreated = false;
        int prio = surroundingCells.Values.ToList().Sum();
        if (IsCornerPoint(gp))
        {
            // always create a new node in this case
            nodeCreated = true;
            Node newNode = new Node(gp, prio);
            if (node != null)
            {
                newNode.Edges.Add(new Edge { From = node, To = newNode, CellID = cellID });
            }
            node = newNode;
        }
        else if (IsOnSideLine(gp))
        {
            // create new node if 1 or more different cell types are near
            if (surroundingCells.Keys.Count >= 1)
            {
                nodeCreated = true;
                Node newNode = new Node(gp, prio);
                if (node != null)
                {
                    newNode.Edges.Add(new Edge { From = newNode, To = node, CellID = cellID });
                }
                node = newNode;
            }
        }
        else if (IsOnSidePlane(gp))
        {
            // create new node if 2 or more cells are near
            if (surroundingCells.Keys.Count >= 2)
            {
                nodeCreated = true;
                Node newNode = new Node(gp, prio);
                if (node != null)
                {
                    newNode.Edges.Add(new Edge { From = newNode, To = node, CellID = cellID });
                }
                node = newNode;
            }
        }
        else
        {
            // need to be at least 3 id around
            if (surroundingCells.Keys.Count >= 3)
            {
                nodeCreated = true;
                Node newNode = new Node(gp, prio);
                if (node != null)
                {
                    newNode.Edges.Add(new Edge { From = newNode, To = node, CellID = cellID });
                }
                node = newNode;
            }
        }

        // find nearby nodes, if 
        if (nodeCreated)
        {
            List<Node> toDelete = new List<Node>();
            bool foundNode = false;
            foreach (Node n in nodes)
            {
                GridPoint p = n.Point;
                if (p == gp)
                {
                    continue;
                }
                // if node is near 
                int offset = combineNodeSearchrange;
                if (p.x >= gp.x - offset && p.x <= gp.x + offset &&
                    p.y >= gp.y - offset && p.y <= gp.y + offset &&
                    p.z >= gp.z - offset && p.z <= gp.z + offset)
                {
                    foundNode = true;
                    if (node.Priotity > n.Priotity) // new node is a better candidate
                    {
                        node.Combine(n);
                        toDelete.Add(n);
                    }
                }
            }
            if (foundNode)
            {
                if (toDelete.Count == 0)
                {
                    node = queueElement.prevNode;
                    nodeCreated = false;
                }
                else
                {
                    nodes.Add(node);
                    foreach (Node n in toDelete)
                    {
                        nodes.Remove(n);
                    }
                }
            }
            else //no nearby nodes found, just add to the list.
            {
                nodes.Add(node);
            }

        }

        // End cases: either we end in a point supposed to be a node, or not.

        //if final point: no new point to add to the queue, and queue is empty
        if (pointsToQueue.Count == 0 && queue.Count == 0)
        {
            if (nodeCreated)
            {
                //connect up with the newest Node 

                Node newest = nodes[nodes.Count - 1];
                Node secondNewest = nodes[nodes.Count - 2];

                if (newest == queueElement.prevNode)
                {
                    Edge edge = new Edge { CellID = cellID, From = node, To = secondNewest };
                    node.Edges.Add(edge);
                }
                else
                {
                    Edge edge = new Edge { CellID = cellID, From = node, To = newest };
                    node.Edges.Add(edge);
                }
            }
            else // not a node point
            {
                // take two newest nodes and create an edge between them
                Node newest = nodes[nodes.Count - 1];
                Node secondNewest = nodes[nodes.Count - 2];

                Edge edge = new Edge { CellID = cellID, From = newest, To = secondNewest };
                newest.Edges.Add(edge);

                //does it need to go both ways?
                secondNewest.Edges.Add(edge);

            }
        }
        // add points to the queue, 
        foreach (GridPoint item in pointsToQueue)
        {
            queue.Enqueue(new QueueElement { gridPoint = item, prevNode = node });
        }
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
