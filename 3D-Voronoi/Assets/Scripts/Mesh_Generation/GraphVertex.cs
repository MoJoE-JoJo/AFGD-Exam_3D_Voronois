using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphVertex : MonoBehaviour
{
    public List<int> cellIds;
    public List<GraphVertex> connectedVertices;
    public Vector3 position;
    private MemDivideAndConquer3D memDnC;

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        memDnC = GameObject.FindGameObjectWithTag("DivideAndConquer").GetComponent<MemDivideAndConquer3D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void DebugDraw()
    {
        for(int vertex = 0; vertex<connectedVertices.Count; vertex++)
        {
            for(int cellIdIndex = 0; cellIdIndex < cellIds.Count; cellIdIndex++)
            {
                if (connectedVertices[vertex].cellIds.Contains(cellIds[cellIdIndex]))
                {
                    Debug.DrawLine(position, connectedVertices[vertex].position, memDnC.cells[cellIds[cellIdIndex]].color);
                }
            }
        }
    }
}
