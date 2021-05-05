using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphDebugging : MonoBehaviour
{
    public List<GraphVertex> vertices;
    public bool debugDraw = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (debugDraw)
        {
            foreach(GraphVertex gv in vertices)
            {
                gv.DebugDraw();
            }
        }
    }
}
