using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivideAndConquer : MonoBehaviour
{
    public bool debug;
    private bool debugGridCreated = false;
    //public GameObject area;
    public List<Vector3> seedPoints;
    public int resolution;
    private Vector3 origin;
    public float size; // along the x-axis and z-axis
    public List<VCell> cells;
    public DnCPoint[][] grid;
    //public GameObject gridCellObject;

    void Start()
    {
        //Debug.Log(area.GetComponent<MeshCollider>().bounds.extents);
        SetSeeds();
        //area.SetActive(false);
        GenerateGrid();
    }

    void Update()
    {
        
    }

    //----------DEBUGGING METHODS----------
    #region debugging
    private void OnDrawGizmos()
    {
        if (debug)
        {
            if (!debugGridCreated) 
            {
                SetSeeds();
                GenerateGrid();
                debugGridCreated = true;
            }
            for (int outer = 0; outer < grid.Length; outer++)
            {
                for (int inner = 0; inner < grid[outer].Length; inner++)
                {
                    grid[outer][inner].DebugDraw();
                }
            }
            DrawSeeds();
        }
        else if(!debug && debugGridCreated)
        {
            debugGridCreated = false;
        }

        DrawGridArea();

    }
    private void DrawGridArea()
    {
        var topRight = origin;
        topRight.x += size;
        var bottomLeft = origin;
        bottomLeft.y -= size;
        var bottomRight = origin;
        bottomRight.y -= size;
        bottomRight.x += size;

        Debug.DrawLine(origin, topRight, Color.white);
        Debug.DrawLine(origin, bottomLeft, Color.white);
        Debug.DrawLine(bottomLeft, bottomRight, Color.white);
        Debug.DrawLine(topRight, bottomRight, Color.white);
    }

    private void DrawSeeds()
    {
        float size = 0.10f;
        for(int i=0; i<cells.Count; i++)
        {
            Debug.DrawLine(cells[i].seed-new Vector3(size, 0, 0), cells[i].seed + new Vector3(size, 0, 0), cells[i].color);
            Debug.DrawLine(cells[i].seed - new Vector3(0, size, 0), cells[i].seed + new Vector3(0, size, 0), cells[i].color);
        }
    }
    #endregion
    //----------ALGORITHM METHODS----------
    #region algorithm
    private void SetSeeds()
    {
        origin = transform.position;
        cells = new List<VCell>();
        for (int i = 1; i <= seedPoints.Count; i++)
        {
            var cell = new VCell
            {
                id = i - 1,
                seed = seedPoints[i - 1] + origin,
                points = new List<Vector3>(),
                color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f))
            };
            cells.Add(cell);
        }
    }

    private void GenerateGrid()
    {
        grid = new DnCPoint[resolution][];
        for(int i = 0; i < grid.Length; i++)
        {
            grid[i] = new DnCPoint[resolution];
        }
        for (int outer = 0; outer < grid.Length; outer++)
        {
            for (int inner = 0; inner < grid[outer].Length; inner++)
            {
                var newPoint = new DnCPoint();
                
                var center = new Vector3(
                    origin.x + (outer*size/resolution) + 0.5f * size/resolution,
                    origin.y + 0 - (inner * size / resolution) - 0.5f * size / resolution,
                    0);


                newPoint.center = center;
                newPoint.x = size / resolution;
                newPoint.y = size / resolution;
                
                grid[outer][inner] = newPoint;
            }
        }

    }
    #endregion
}
