using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivideAndConquer : MonoBehaviour
{
    public GameObject area;
    public List<Vector3> seedPoints;
    public int resolution;
    public Vector3 origin;
    public float size; // along the x-axis and z-axis
    public List<VCell> cells;
    public DnCPoint[][] grid;
    public GameObject gridCellObject;

    void Start()
    {
        //Debug.Log(area.GetComponent<MeshCollider>().bounds.extents);
        for(int i = 1; i <= seedPoints.Count; i++)
        {
            var cell = new VCell
            {
                id = i - 1,
                seed = seedPoints[i - 1],
                points = new List<Vector3>(),
                color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f))
            };
            cells.Add(cell);
        }
        //area.SetActive(false);
        GenerateGrid();
    }

    void Update()
    {
        
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
                    (outer*size/resolution) + 0.5f * size/resolution, 
                    0,
                    0-(inner * size / resolution) - 0.5f * size/resolution);


                newPoint.center = center;
                newPoint.x = size / resolution;
                newPoint.y = size / resolution;
                
                grid[outer][inner] = newPoint;
                var go = Instantiate(gridCellObject, this.transform);
                go.transform.position = newPoint.center;
                go.GetComponent<MeshRenderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                go.transform.localScale = new Vector3(newPoint.x, 1, newPoint.y);
                
            }
        }

    }
}
