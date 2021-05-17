using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivideAndConquerOld : MonoBehaviour
{
    public bool debug = false;
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
            for (int i = 0; i < seedPoints.Count; i++)
            {
                cells[i].points = new List<GridPoint>();
            }
            DrawPointGrid();
            DrawSeeds();
            DividAndConquer();
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

    private void DrawPointGrid()
    {
        for (int outer = 0; outer < grid.Length; outer++)
        {
            for (int inner = 0; inner < grid[outer].Length; inner++)
            {
                if(grid[outer][inner].cellId != -1)grid[outer][inner].DebugDraw();
            }
        }
    }
    #endregion
    //----------ALGORITHM METHODS----------
    #region algorithm
    private void SetSeeds()
    {
        origin = transform.position;
        cells = new List<VCell>();
        for (int i = 0; i < seedPoints.Count; i++)
        {
            var cell = new VCell
            {
                id = i,
                seed = seedPoints[i] + origin,
                points = new List<GridPoint>(),
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

    private void DividAndConquer()
    {
        //Step 1: calculate Points
        var topLeftID = FindNearestSeed(0,0);
        var topRightID = FindNearestSeed(resolution - 1, 0);
        var bottomLeftID = FindNearestSeed(0, resolution - 1);
        var bottomRightID = FindNearestSeed(resolution - 1, resolution - 1);
        //Step 2: Base Case
        if(CheckBaseCase(topLeftID, topRightID, bottomLeftID, bottomRightID))
        {
            BaseCase(topLeftID, 0,0, resolution - 1, resolution - 1);
        }
        //Step 3: Subdivide Case
        else
        {
            int halfPoint = (resolution - 1) / 2;
            DivideAndConquerRecursive(0, 0, halfPoint, halfPoint); //TopLeft subdivision
            DivideAndConquerRecursive(halfPoint + 1, 0, (resolution - 1), halfPoint); //TopRight subdivision
            DivideAndConquerRecursive(0, halfPoint + 1, halfPoint, (resolution - 1)); //BottomLeft subdivision
            DivideAndConquerRecursive(halfPoint + 1, halfPoint + 1, (resolution - 1), (resolution - 1)); //BottomRight subdivision
        }
        //TODO: Clear unnecessary data
        CullInnerPoints();
    }

    private void DivideAndConquerRecursive(int topLeftX, int topLeftY, int bottomRightX, int bottomRightY)
    {
        //Check for single point
        if (topLeftX == bottomRightX && topLeftY == bottomRightY) TrivialCase(topLeftX, topLeftY);
        //Check for only 2 points
        else if ((topLeftX == bottomRightX && topLeftY != bottomRightY) || (topLeftX != bottomRightX && topLeftY == bottomRightY)) 
        {
            TrivialCase(topLeftX, topLeftY);
            TrivialCase(bottomRightX, bottomRightY);
        }
        else
        {
            //Step 1: calculate Points
            var topLeftID = FindNearestSeed(topLeftX, topLeftY);
            var topRightID = FindNearestSeed(bottomRightX, topLeftY);
            var bottomLeftID = FindNearestSeed(topLeftX, bottomRightY);
            var bottomRightID = FindNearestSeed(bottomRightX, bottomRightY);
            //Step 2: Base Case
            if (CheckBaseCase(topLeftID, topRightID, bottomLeftID, bottomRightID))
            {
                BaseCase(topLeftID, topLeftX, topLeftY, bottomRightX, bottomRightY);
            }
            //Step 3: Subdivide Case
            else
            {
                int halfPointX = topLeftX + (bottomRightX-topLeftX) / 2;
                int halfPointY = topLeftY + (bottomRightY-topLeftY) / 2;
                DivideAndConquerRecursive(topLeftX, topLeftY, halfPointX, halfPointY); //TopLeft subdivision
                DivideAndConquerRecursive(halfPointX + 1, topLeftY, bottomRightX, halfPointY); //TopRight subdivision
                DivideAndConquerRecursive(topLeftX, halfPointY + 1, halfPointX, bottomRightY); //BottomLeft subdivision
                DivideAndConquerRecursive(halfPointX + 1, halfPointY + 1, bottomRightX, bottomRightY); //BottomRight subdivision
            }
        }
    }

    private void BaseCase(int id, int topLeftX, int topLeftY, int bottomRightX, int bottomRightY)
    {

        /*
        //Full add
        for(int x = topLeftX; x <= bottomRightX; x++)
        {
            for(int y = topLeftY; y <= bottomRightY; y++)
            {
                grid[x][y].cellId = id;
                grid[x][y].color = cells[id].color;
                cells[id].points.Add(grid[x][y].center);
            }
        }
        */
        /*
        //Adds only the corners
        grid[topLeftX][topLeftY].cellId = id;
        grid[topLeftX][topLeftY].color = cells[id].color;
        cells[id].points.Add(grid[topLeftX][topLeftY].center);

        grid[bottomRightX][topLeftY].cellId = id;
        grid[bottomRightX][topLeftY].color = cells[id].color;
        cells[id].points.Add(grid[bottomRightX][topLeftY].center);

        grid[topLeftX][bottomRightY].cellId = id;
        grid[topLeftX][bottomRightY].color = cells[id].color;
        cells[id].points.Add(grid[topLeftX][bottomRightY].center);

        grid[bottomRightX][bottomRightY].cellId = id;
        grid[bottomRightX][bottomRightY].color = cells[id].color;
        cells[id].points.Add(grid[bottomRightX][bottomRightY].center);
        */

        //Adds only square edge
        
        for(int x = topLeftX; x <= bottomRightX; x++)
        {
            grid[x][topLeftY].cellId = id;
            grid[x][topLeftY].color = cells[id].color;
            cells[id].points.Add(new GridPoint { x = x, y = topLeftY });
        }
        for (int x = topLeftX; x <= bottomRightX; x++)
        {
            grid[x][bottomRightY].cellId = id;
            grid[x][bottomRightY].color = cells[id].color;
            cells[id].points.Add(new GridPoint { x = x, y = bottomRightY});
        }
        for (int y = topLeftY+1; y <= bottomRightY-1; y++)
        {
            grid[topLeftX][y].cellId = id;
            grid[topLeftX][y].color = cells[id].color;
            cells[id].points.Add(new GridPoint { x = topLeftX, y = y});
        }
        for (int y = topLeftY+1; y <= bottomRightY-1; y++)
        {
            grid[bottomRightX][y].cellId = id;
            grid[bottomRightX][y].color = cells[id].color;
            cells[id].points.Add(new GridPoint { x = bottomRightX, y = y});
        }
    }

    private void TrivialCase(int x, int y)
    {
        var seedID = FindNearestSeed(x, y);

        grid[x][y].cellId = seedID;
        grid[x][y].color = cells[seedID].color;
        cells[seedID].points.Add(new GridPoint { x = x, y = y});
    }

    private void CullInnerPoints()
    {
        for(int cellIndex = 0; cellIndex<cells.Count; cellIndex++)
        {
            var newCellPointList = new List<GridPoint>();
            for(int pointIndex = 0; pointIndex < cells[cellIndex].points.Count; pointIndex++)
            {
                int pointX = cells[cellIndex].points[pointIndex].x;
                int pointY = cells[cellIndex].points[pointIndex].y;
                var onRim = false;
                if (pointX == 0 || pointX == resolution - 1 || pointY == 0 || pointY == resolution - 1) onRim = true;
                else
                {
                    for(int x = pointX-1; x <= pointX+1; x++)
                    {
                        for(int y = pointY-1; y <= pointY+1; y++)
                        {
                            if (grid[x][y].cellId > -1 && grid[pointX][pointY].cellId != grid[x][y].cellId) onRim = true;
                        }
                    }
                }
                if (onRim)
                {
                    newCellPointList.Add(new GridPoint { x = pointX, y = pointY });
                }
                else if (!onRim)
                {
                    grid[pointX][pointY].color = Color.green;
                    grid[pointX][pointY].cellId = -1;
                }
            }
            cells[cellIndex].points = newCellPointList;
        }
    }

    private bool CheckBaseCase(int seed1, int seed2, int seed3, int seed4)
    {
        if (seed1 == seed2 && seed1 == seed3 && seed1 == seed4) return true;
        else return false;
    }

    private int FindNearestSeed(int x, int y)
    {
        float distance = (seedPoints[0] - grid[x][y].center).magnitude;

        var returnID = 0;
        for(int i = 1; i<seedPoints.Count; i++)
        {
            float newDistance = (seedPoints[i] - grid[x][y].center).magnitude;
            if (newDistance < distance) 
            {
                distance = newDistance;
                returnID = i;
            }
        }

        return returnID;
    }

    #endregion
}
