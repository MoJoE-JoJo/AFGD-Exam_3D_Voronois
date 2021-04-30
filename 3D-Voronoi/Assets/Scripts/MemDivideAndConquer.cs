using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DEBUGDRAWTYPE { DRAWALL, DRAWEDGE, DRAWSQUARES, DRAWCHECKPOINTS}

public class MemDivideAndConquer : MonoBehaviour
{
    [Header("Debugging")]
    public bool drawArea = false;
    public bool drawSeeds = false;
    public bool drawDivideAndConquer = false;
    public DEBUGDRAWTYPE debugType = DEBUGDRAWTYPE.DRAWEDGE;
    private DnCDebugPoint debugPoint = new DnCDebugPoint();


    [Header("Redraw")]
    public bool rerun;

    [Header("Algorithm Stuff")]
    private GridPoint addGridPoint = new GridPoint();
    private Vector3 gridPointCenterVector = new Vector3();
    public List<Vector3> seedPoints;
    public int resolution;
    private Vector3 origin;
    
    public float size;
    public List<VCell> cells;
    
    public int[][] grid;
    

    void Start()
    {
        SetSeeds();
        InitGrid();
        DividAndConquer();
    }
    private void Update()
    {
        if (rerun)
        {
            rerun = false;
            SetSeeds();
            InitGrid();
            DividAndConquer();
        }
        if (drawArea) DrawGridArea();
        if (drawSeeds) DrawSeeds();
        if (drawDivideAndConquer) DrawPointGrid();
    }

    //----------DEBUGGING METHODS----------
    #region debugging
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
        for (int i = 0; i < cells.Count; i++)
        {
            Debug.DrawLine(cells[i].seed - new Vector3(size, 0, 0), cells[i].seed + new Vector3(size, 0, 0), cells[i].color);
            Debug.DrawLine(cells[i].seed - new Vector3(0, size, 0), cells[i].seed + new Vector3(0, size, 0), cells[i].color);
        }
    }

    private void DrawPointGrid()
    {
        int x, y;
        for (x = 0; x < grid.Length; x++)
        {
            for (y = 0; y < grid[x].Length; y++)
            {
                if (grid[x][y] > -1) DebugDrawPoint((int)grid[x][y], x, y);
                //if (grid[outer][inner].cellId != -1) grid[outer][inner].DebugDraw();
            }
        }
    }

    private void DebugDrawPoint(int id, int x, int y)
    {

        //var newPoint = new DnCDebugPoint();

        debugPoint.center = GridPointCenter(x, y);


        debugPoint.x = size / resolution;
        debugPoint.y = size / resolution;
        debugPoint.DebugDraw(cells[id].color);
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

    private void InitGrid()
    {
        int x, y;
        grid = new int[resolution][];
        for (int i = 0; i < grid.Length; i++)
        {
            grid[i] = new int[resolution];
        }
        for (x = 0; x < grid.Length; x++)
        {
            for (y = 0; y < grid[x].Length; y++)
            {
                grid[x][y] = -1;
            }
        }

    }

    private void DividAndConquer()
    {
        //Step 1: calculate Points
        var topLeftID = FindNearestSeed(0, 0);
        var topRightID = FindNearestSeed(resolution - 1, 0);
        var bottomLeftID = FindNearestSeed(0, resolution - 1);
        var bottomRightID = FindNearestSeed(resolution - 1, resolution - 1);
        //Step 2: Base Case
        if (CheckBaseCase(topLeftID, topRightID, bottomLeftID, bottomRightID))
        {
            BaseCase(topLeftID, 0, 0, resolution - 1, resolution - 1);
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
        if(debugType == DEBUGDRAWTYPE.DRAWEDGE) CullInnerPoints();
        else if(!drawDivideAndConquer) CullInnerPoints();
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
                int halfPointX = topLeftX + (bottomRightX - topLeftX) / 2;
                int halfPointY = topLeftY + (bottomRightY - topLeftY) / 2;
                DivideAndConquerRecursive(topLeftX, topLeftY, halfPointX, halfPointY); //TopLeft subdivision
                DivideAndConquerRecursive(halfPointX + 1, topLeftY, bottomRightX, halfPointY); //TopRight subdivision
                DivideAndConquerRecursive(topLeftX, halfPointY + 1, halfPointX, bottomRightY); //BottomLeft subdivision
                DivideAndConquerRecursive(halfPointX + 1, halfPointY + 1, bottomRightX, bottomRightY); //BottomRight subdivision
            }
        }
    }

    private void BaseCase(int id, int topLeftX, int topLeftY, int bottomRightX, int bottomRightY)
    {
        int x, y;
        //Full add
        if (debugType == DEBUGDRAWTYPE.DRAWALL)
        {
            for (x = topLeftX; x <= bottomRightX; x++)
            {
                for (y = topLeftY; y <= bottomRightY; y++)
                {
                    addGridPoint.x = x;
                    addGridPoint.y = y;
                    grid[x][y] = id;
                    cells[id].points.Add(addGridPoint);
                }
            }
        }

        //Adds only the corners
        if(debugType == DEBUGDRAWTYPE.DRAWCHECKPOINTS)
        {
            addGridPoint.x = topLeftX;
            addGridPoint.y = topLeftY;
            grid[topLeftX][topLeftY] = id;
            cells[id].points.Add(addGridPoint);

            addGridPoint.x = bottomRightX;
            addGridPoint.y = topLeftY;
            grid[bottomRightX][topLeftY]= id;
            cells[id].points.Add(addGridPoint);

            addGridPoint.x = topLeftX;
            addGridPoint.y = topLeftY;
            grid[topLeftX][bottomRightY]= id;
            cells[id].points.Add(addGridPoint);

            addGridPoint.x = bottomRightX;
            addGridPoint.y = bottomRightY;
            grid[bottomRightX][bottomRightY]= id;
            cells[id].points.Add(addGridPoint);
        }

        //Adds only square edge
        if(debugType == DEBUGDRAWTYPE.DRAWSQUARES || debugType == DEBUGDRAWTYPE.DRAWEDGE)
        {
            for (x = topLeftX; x <= bottomRightX; x++)
            {
                addGridPoint.x = x;
                addGridPoint.y = topLeftY;
                grid[x][topLeftY] = id;
                cells[id].points.Add(addGridPoint);
            }
            for (x = topLeftX; x <= bottomRightX; x++)
            {
                addGridPoint.x = x;
                addGridPoint.y = bottomRightY;
                grid[x][bottomRightY] = id;
                cells[id].points.Add(addGridPoint);
            }
            for (y = topLeftY + 1; y <= bottomRightY - 1; y++)
            {
                addGridPoint.x = topLeftX;
                addGridPoint.y = y;
                grid[topLeftX][y] = id;
                cells[id].points.Add(addGridPoint);
            }
            for (y = topLeftY + 1; y <= bottomRightY - 1; y++)
            {
                addGridPoint.x = bottomRightX;
                addGridPoint.y = y;
                grid[bottomRightX][y] = id;
                cells[id].points.Add(addGridPoint);
            }
        }
    }

    private void TrivialCase(int x, int y)
    {

        var seedID = FindNearestSeed(x, y);

        grid[x][y]= seedID;

        addGridPoint.x = x;
        addGridPoint.y = y;
        cells[seedID].points.Add(addGridPoint);
    }

    private void CullInnerPoints()
    {
        List<GridPoint> newCellPointList;
        int pointX;
        int pointY;
        bool onRim;

        int x, y;
        int pointIndex;

        for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
        {
            newCellPointList = new List<GridPoint>();
            for (pointIndex = 0; pointIndex < cells[cellIndex].points.Count; pointIndex++)
            {
                pointX = cells[cellIndex].points[pointIndex].x;
                pointY = cells[cellIndex].points[pointIndex].y;
                onRim = false;
                if (pointX == 0 || pointX == resolution - 1 || pointY == 0 || pointY == resolution - 1) onRim = true;
                else
                {
                    for (x = pointX - 1; x <= pointX + 1; x++)
                    {
                        for (y = pointY - 1; y <= pointY + 1; y++)
                        {
                            if (grid[x][y] > -1 && grid[pointX][pointY] != grid[x][y])
                            {
                                onRim = true;
                                addGridPoint.x = pointX;
                                addGridPoint.y = pointY;
                            }
                            //if (grid[x][y] != null && grid[pointX][pointY] != grid[x][y]) onRim = true;
                        }
                    }
                }
                if (onRim)
                {
                    newCellPointList.Add(addGridPoint);
                }
                else if (!onRim)
                {
                    grid[pointX][pointY] = -1;
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
        var center = GridPointCenter(x, y);

        float distance = (seedPoints[0] - center).magnitude;

        var returnID = 0;
        for (int i = 1; i < seedPoints.Count; i++)
        {
            float newDistance = (seedPoints[i] - center).magnitude;
            if (newDistance < distance)
            {
                distance = newDistance;
                returnID = i;
            }
        }

        return returnID;
    }

    private Vector3 GridPointCenter(int x, int y)
    {

        gridPointCenterVector.x = origin.x + (x * size / resolution) + 0.5f * size / resolution;
        gridPointCenterVector.y = origin.y + 0 - (y * size / resolution) - 0.5f * size / resolution;
        gridPointCenterVector.z = 0;
        return gridPointCenterVector;
    }

    #endregion
}
