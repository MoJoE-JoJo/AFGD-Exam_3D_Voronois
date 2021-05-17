using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlaneGenerator : MonoBehaviour
{
    public bool debugGenerate = false;
    public bool debugDrawCenters = false;
    public bool debugMeshGenerate = false;
    public Material polyHedronMaterial;
    public List<GraphVertex> cellVertices;
    public int cellId;
    private List<CellPlane> planes;

    public void Update()
    {
        if (debugGenerate)
        {
            planes = GeneratePlanesForCell(cellId, cellVertices);
            Debug.Log(planes.Count);
            debugGenerate = false;
        }
        if (debugDrawCenters)
        {
            foreach(CellPlane cp in planes)
            {
                cp.DebugDrawCenter();
            }
        }
        if (debugMeshGenerate)
        {
            debugMeshGenerate = false;
            MeshGenerator.GenerateMesh(planes, cellId, polyHedronMaterial);
        }
    }

    public List<CellPlane> GeneratePlanesForCell(int cellId, List<GraphVertex> vertices)
    {
        //For each vertex run breadth first search, check for cycles, stop when a number of cycles equal to number of neighboors of the vertex
        var planes = new List<CellPlane>();
        for(int i = 0; i < vertices.Count; i++)
        {
            var planes3 = BreadthFirstSearch(cellId, vertices, vertices[i]);
            foreach(CellPlane planeIn3 in planes3)
            {
                var contained = false;
                foreach(CellPlane plane in planes)
                {
                    if (planeIn3.PlaneEquals(plane))
                    {
                        contained = true;
                        break;
                    }
                }
                if (!contained) planes.Add(planeIn3);
            }
        }


        for (int i = 0; i < planes.Count; i++)
        {
            if (!planes[i].CheckValidPlane())
            {
                planes.RemoveAt(i);
                i--;
            }
        }
        return planes;
    }

    private List<CellPlane> BreadthFirstSearch(int cellId, List<GraphVertex> graph, GraphVertex root)
    {
        var targetNumberOfPlanes = FindNumberOfNeighborsInCell(cellId, root.connectedVertices);
        var planes = new List<CellPlane>();

        HashSet<GraphVertex> searched = new HashSet<GraphVertex>();
        Queue<LinkedList<GraphVertex>> frontier = new Queue<LinkedList<GraphVertex>>();
        List<LinkedList<GraphVertex>> chains = new List<LinkedList<GraphVertex>>();
        foreach(GraphVertex neighbor in root.connectedVertices)
        {
            if (graph.Contains(neighbor))
            {
                var chain = new LinkedList<GraphVertex>();
                chains.Add(chain);
                chain.AddLast(root);
                chain.AddLast(neighbor);
                frontier.Enqueue(chain);
            }
        }
        searched.Add(root);

        while (frontier.Count > 0 /*&& planes.Count < targetNumberOfPlanes*/)
        {
            LinkedList<GraphVertex> chain = frontier.Dequeue();
            var cycleFound = -1;
            for(int i = 0; i< chains.Count; i++)
            {
                if(chains[i].Last.Value == chain.Last.Value && chains[i] != chain)
                {
                    cycleFound = i;
                }
            }

            /*
            if (searched.Contains(chain.Last.Value))
            {
                //should only happen when a cycle has been detected and a plane created   
                break;
            }
            */


            if (cycleFound > -1)
            {
                var chain1 = chain;
                var chain2 = chains[cycleFound];
                chains.RemoveAt(cycleFound);
                chains.Remove(chain);

                List<GraphVertex> vertices = new List<GraphVertex>();
                var node = chain1.First;
                while(node != chain1.Last)
                {
                    vertices.Add(node.Value);
                    node = node.Next;
                }
                node = chain2.Last;
                while(node != chain2.First)
                {
                    vertices.Add(node.Value);
                    node = node.Previous;
                }
                var plane = new CellPlane();
                plane.vertices = vertices;
                planes.Add(plane);
            }

            else
            {
                foreach (GraphVertex neighbor in chain.Last.Value.connectedVertices)
                {
                    if (graph.Contains(neighbor) && !searched.Contains(neighbor))
                    {
                        var newChain = new LinkedList<GraphVertex>();
                        foreach (GraphVertex vertex in chain)
                        {
                            newChain.AddLast(vertex);
                        }
                        if (chains.Contains(chain)) chains.Remove(chain);

                        chains.Add(newChain);
                        newChain.AddLast(neighbor);
                        frontier.Enqueue(newChain);
                    }
                }
            }

            searched.Add(chain.Last.Value);
        }

        return planes;

    }

    private int FindNumberOfNeighborsInCell(int cellId, List<GraphVertex> neighbors)
    {
        var number = 0;
        for(int i = 0; i<neighbors.Count; i++)
        {
            if (neighbors[i].cellIds.Contains(cellId)) number++;
        }
        return number;
    }

}
