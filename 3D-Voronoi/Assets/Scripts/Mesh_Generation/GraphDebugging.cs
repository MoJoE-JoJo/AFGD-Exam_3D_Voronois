using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphDebugging : MonoBehaviour
{
    public List<GraphVertex> vertices;
    public bool debugDraw = false;
    public GraphVertex gv1;
    public GraphVertex gv2;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(gv1 == gv2);
    }

    // Update is called once per frame
    void Update()
    {
        if (debugDraw)
        {
            foreach(GraphVertex gv in vertices)
            {
                //gv.DebugDraw();
            }
        }
    }
}
