using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemDivideAndConquer3D : MonoBehaviour
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
    //private Vector3 gridPointCenterVector = new Vector3();
    public List<Vector3> seedPoints;
    public int resolution;
    private Vector3 origin;

    public float size;
    public List<VCell> cells;

    public int[][][] grid;


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
        //front points
        var rightBottomFront = origin;
        var leftTopFront = origin;
        var rightTopFront = origin;

        //back points
        var leftBottomBack = origin;
        var rightBottomBack = origin;
        var leftTopBack = origin;
        var rightTopBack = origin;

        //rightBottomFront
        rightBottomFront.x += size;

        //leftTopFront
        leftTopFront.y += size;

        //rightTopFront
        rightTopFront.x += size;
        rightTopFront.y += size;

        //leftBottomBack
        leftBottomBack.z += size;

        //rightBottomBack
        rightBottomBack.x += size;
        rightBottomBack.z += size;

        //leftTopBack
        leftTopBack.y += size;
        leftTopBack.z += size;

        //rightTopBack
        rightTopBack.x += size;
        rightTopBack.y += size;
        rightTopBack.z += size;

        Debug.DrawLine(origin, rightBottomFront, Color.white);
        Debug.DrawLine(origin, leftTopFront, Color.white);
        Debug.DrawLine(rightBottomFront, rightTopFront, Color.white);
        Debug.DrawLine(leftTopFront, rightTopFront, Color.white);

        Debug.DrawLine(leftBottomBack, rightBottomBack, Color.white);
        Debug.DrawLine(leftBottomBack, leftTopBack, Color.white);
        Debug.DrawLine(rightBottomBack, rightTopBack, Color.white);
        Debug.DrawLine(leftTopBack, rightTopBack, Color.white);

        Debug.DrawLine(origin, leftBottomBack, Color.white);
        Debug.DrawLine(rightBottomFront, rightBottomBack, Color.white);
        Debug.DrawLine(leftTopFront, leftTopBack, Color.white);
        Debug.DrawLine(rightTopFront, rightTopBack, Color.white);


    }

    private void DrawSeeds()
    {
        float size = 0.10f;
        for (int i = 0; i < cells.Count; i++)
        {
            Debug.DrawLine(cells[i].seed - new Vector3(size, 0, 0), cells[i].seed + new Vector3(size, 0, 0), cells[i].color);
            Debug.DrawLine(cells[i].seed - new Vector3(0, size, 0), cells[i].seed + new Vector3(0, size, 0), cells[i].color);
            Debug.DrawLine(cells[i].seed - new Vector3(0, 0, size), cells[i].seed + new Vector3(0, 0, size), cells[i].color);
        }
    }

    private void DrawPointGrid()
    {
        int x, y, z;
        for (x = 0; x < grid.Length; x++)
        {
            for (y = 0; y < grid[x].Length; y++)
            {
                for (z = 0; z < grid[x][y].Length; z++)
                {
                    if (grid[x][y][z] > -1) DebugDrawPoint(grid[x][y][z], x, y, z);

                }
            }
        }
    }

    private void DebugDrawPoint(int id, int x, int y, int z)
    {

        //var newPoint = new DnCDebugPoint();

        debugPoint.center = GridPointCenter(x, y, z);


        debugPoint.x = size / resolution;
        debugPoint.y = size / resolution;
        debugPoint.z = size / resolution;
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
        int x, y, z;
        int j;
        grid = new int[resolution][][];
        for (int i = 0; i < grid.Length; i++)
        {
            grid[i] = new int[resolution][];
            for (j = 0; j <grid[i].Length; j++)
            {
                grid[i][j] = new int[resolution];
            }
        }
        for (x = 0; x < grid.Length; x++)
        {
            for (y = 0; y < grid[x].Length; y++)
            {
                for(z = 0; z < grid[x][y].Length; z++)
                {
                    grid[x][y][z] = -1;
                }
            }
        }

    }

    private void DividAndConquer()
    {
        //Step 1: calculate Points
        var leftBottomFrontID = FindNearestSeed(0, 0, 0);
        var rightBottomFrontID = FindNearestSeed(resolution - 1, 0, 0);
        var leftTopFrontID = FindNearestSeed(0, resolution - 1, 0);
        var rightTopFrontID = FindNearestSeed(resolution - 1, resolution - 1, 0);

        var leftBottomBackID = FindNearestSeed(0, 0, resolution - 1);
        var rightBottomBackID = FindNearestSeed(resolution - 1, 0, resolution - 1);
        var leftTopBackID = FindNearestSeed(0, resolution - 1, resolution - 1);
        var rightTopBackID = FindNearestSeed(resolution - 1, resolution - 1, resolution - 1);

        //Step 2: Base Case
        if (CheckBaseCase(leftBottomFrontID, leftTopFrontID, rightBottomFrontID, rightTopFrontID, leftBottomBackID, leftTopBackID, rightBottomBackID, rightTopBackID))
        {
            BaseCase(leftBottomFrontID, 0, resolution - 1, 0, resolution - 1, 0, resolution - 1);
        }
        //Step 3: Subdivide Case
        else
        {
            int halfPoint = (resolution - 1) / 2;
            DivideAndConquerRecursive(0, halfPoint, 0, halfPoint, 0, halfPoint); //LeftBottomFront subdivision
            DivideAndConquerRecursive(halfPoint + 1, (resolution - 1), 0, halfPoint, 0, halfPoint); //RightBottomFront subdivision
            DivideAndConquerRecursive(0, halfPoint, halfPoint + 1, (resolution - 1), 0, halfPoint); //LeftTopFront subdivision
            DivideAndConquerRecursive(halfPoint + 1, (resolution - 1), halfPoint + 1, (resolution - 1), 0, halfPoint); //RightTopFront subdivision

            DivideAndConquerRecursive(0, halfPoint, 0, halfPoint, halfPoint + 1, (resolution - 1)); //LeftBottomBack subdivision
            DivideAndConquerRecursive(halfPoint + 1, (resolution - 1), 0, halfPoint, halfPoint + 1, (resolution - 1)); //RightBottomBack subdivision
            DivideAndConquerRecursive(0, halfPoint, halfPoint + 1, (resolution - 1), halfPoint + 1, (resolution - 1)); //LeftTopBack subdivision
            DivideAndConquerRecursive(halfPoint + 1, (resolution - 1), halfPoint + 1, (resolution - 1), halfPoint + 1, (resolution - 1)); //RightTopBack subdivision
        }
        if (debugType == DEBUGDRAWTYPE.DRAWEDGE) CullInnerPoints();
        else if (!drawDivideAndConquer) CullInnerPoints();
    }

    private void DivideAndConquerRecursive(int leftX, int rightX, int bottomY, int topY, int frontZ, int backZ)
    {
        //Check for single point
        if (leftX == rightX && bottomY == topY && frontZ == backZ) TrivialCase(leftX, bottomY, frontZ);
        else if (leftX == rightX && bottomY == topY && frontZ != backZ)
        {
            //add 2
            TrivialCase(leftX, bottomY, frontZ);
            TrivialCase(leftX, bottomY, backZ);
        }
        else if (leftX != rightX && bottomY == topY && frontZ == backZ)
        {
            //add 2
            TrivialCase(leftX, bottomY, frontZ);
            TrivialCase(rightX, bottomY, frontZ);
        }
        else if (leftX == rightX && bottomY != topY && frontZ == backZ)
        {
            //add 2
            TrivialCase(leftX, bottomY, frontZ);
            TrivialCase(leftX, topY, frontZ);
        }
        else if (leftX == rightX && bottomY != topY && frontZ != backZ)
        {
            //add 4
            TrivialCase(leftX, bottomY, frontZ);
            TrivialCase(leftX, bottomY, backZ);
            TrivialCase(leftX, topY, frontZ);
            TrivialCase(leftX, topY, backZ);
        }
        else if (leftX != rightX && bottomY != topY && frontZ == backZ)
        {
            //add 4
            TrivialCase(leftX, bottomY, frontZ);
            TrivialCase(leftX, topY, frontZ);
            TrivialCase(rightX, bottomY, frontZ);
            TrivialCase(rightX, topY, frontZ);
        }
        
        else
        {
            //Step 1: calculate Points
            var leftBottomFrontID = FindNearestSeed(leftX, bottomY, frontZ);
            var rightBottomFrontID = FindNearestSeed(rightX, bottomY, frontZ);
            var leftTopFrontID = FindNearestSeed(leftX, topY, frontZ);
            var rightTopFrontID = FindNearestSeed(rightX, topY, frontZ);

            var leftBottomBackID = FindNearestSeed(leftX, bottomY, backZ);
            var rightBottomBackID = FindNearestSeed(rightX, bottomY, backZ);
            var leftTopBackID = FindNearestSeed(leftX, topY, backZ);
            var rightTopBackID = FindNearestSeed(rightX, topY, backZ);
            //Step 2: Base Case
            if (CheckBaseCase(leftBottomFrontID, leftTopFrontID, rightBottomFrontID, rightTopFrontID, leftBottomBackID, leftTopBackID, rightBottomBackID, rightTopBackID))
            {
                BaseCase(leftBottomFrontID, leftX, rightX, bottomY, topY, frontZ, backZ);
            }
            //Step 3: Subdivide Case
            else
            {
                //Something is not working
                int halfPointX = leftX + (rightX - leftX) / 2;
                int halfPointY = bottomY + (topY - bottomY) / 2;
                int halfPointZ = frontZ + (backZ - frontZ) / 2;
                DivideAndConquerRecursive(leftX, halfPointX, bottomY, halfPointY, frontZ, halfPointZ); //LeftBottomFront subdivision
                DivideAndConquerRecursive(halfPointX + 1, rightX, bottomY, halfPointY, frontZ, halfPointZ); //RightBottomFront subdivision
                DivideAndConquerRecursive(leftX, halfPointX, halfPointY + 1, topY, frontZ, halfPointZ); //LeftTopFront subdivision
                DivideAndConquerRecursive(halfPointX + 1, rightX, halfPointY + 1, topY, frontZ, halfPointZ); //RightTopFront subdivision

                DivideAndConquerRecursive(leftX, halfPointX, bottomY, halfPointY, halfPointZ + 1, backZ); //LeftBottomBack subdivision
                DivideAndConquerRecursive(halfPointX + 1, rightX, bottomY, halfPointY, halfPointZ + 1, backZ); //RightBottomBack subdivision
                DivideAndConquerRecursive(leftX, halfPointX, halfPointY + 1, topY, halfPointZ + 1, backZ); //LeftTopBack subdivision
                DivideAndConquerRecursive(halfPointX + 1, rightX, halfPointY + 1, topY, halfPointZ + 1, backZ); //RightTopBack subdivision
            }
        }
    }

    private void BaseCase(int id, int leftX, int rightX, int bottomY, int topY, int frontZ, int backZ)
    {
        int x, y, z;
        //Full add
        if (debugType == DEBUGDRAWTYPE.DRAWALL)
        {
            for (x = leftX; x <= rightX; x++)
            {
                for (y = bottomY; y <= topY; y++)
                {
                    for(z = frontZ; z <= backZ; z++)
                    {
                        addGridPoint.x = x;
                        addGridPoint.y = y;
                        addGridPoint.z = z;
                        grid[x][y][z] = id;
                        cells[id].points.Add(addGridPoint);
                    }
                }
            }
        }

        
        //Adds only the corners
        if (debugType == DEBUGDRAWTYPE.DRAWCHECKPOINTS)
        {
            addGridPoint.x = leftX;
            addGridPoint.y = bottomY;
            addGridPoint.z = frontZ;
            grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
            cells[id].points.Add(addGridPoint);

            addGridPoint.y = topY;
            grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
            cells[id].points.Add(addGridPoint);

            addGridPoint.x = rightX;
            grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
            cells[id].points.Add(addGridPoint);

            addGridPoint.y = bottomY;
            grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
            cells[id].points.Add(addGridPoint);

            addGridPoint.z = backZ;
            grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
            cells[id].points.Add(addGridPoint);

            addGridPoint.x = leftX;
            grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
            cells[id].points.Add(addGridPoint);

            addGridPoint.y = topY;
            grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
            cells[id].points.Add(addGridPoint);

            addGridPoint.x = rightX;
            grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
            cells[id].points.Add(addGridPoint);
        }
        

        //Adds only square edge
        if (debugType == DEBUGDRAWTYPE.DRAWSQUARES || debugType == DEBUGDRAWTYPE.DRAWEDGE)
        {
            for (x = leftX; x <= rightX; x++)
            {
                addGridPoint.x = x;

                addGridPoint.y = bottomY;
                addGridPoint.z = frontZ;
                grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
                cells[id].points.Add(addGridPoint);

                addGridPoint.y = topY;
                grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
                cells[id].points.Add(addGridPoint);

                addGridPoint.z = backZ;
                grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
                cells[id].points.Add(addGridPoint);

                addGridPoint.y = bottomY;
                grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
                cells[id].points.Add(addGridPoint);

            }
            for (y = bottomY + 1; y <= topY - 1; y++)
            {
                addGridPoint.y = y;

                addGridPoint.x = leftX;
                addGridPoint.z = frontZ;
                grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
                cells[id].points.Add(addGridPoint);

                addGridPoint.x = rightX;
                grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
                cells[id].points.Add(addGridPoint);

                addGridPoint.z = backZ;
                grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
                cells[id].points.Add(addGridPoint);

                addGridPoint.x = leftX;
                grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
                cells[id].points.Add(addGridPoint);

            }
            for (z = frontZ + 1; z <= backZ - 1; z++)
            {
                addGridPoint.z = z;

                addGridPoint.x = leftX;
                addGridPoint.y = bottomY;
                grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
                cells[id].points.Add(addGridPoint);

                addGridPoint.x = rightX;
                grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
                cells[id].points.Add(addGridPoint);

                addGridPoint.y = topY;
                grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
                cells[id].points.Add(addGridPoint);

                addGridPoint.x = leftX;
                grid[addGridPoint.x][addGridPoint.y][addGridPoint.z] = id;
                cells[id].points.Add(addGridPoint);
            }
        }
    }

    private void TrivialCase(int x, int y, int z)
    {

        var seedID = FindNearestSeed(x, y, z);

        grid[x][y][z] = seedID;

        addGridPoint.x = x;
        addGridPoint.y = y;
        cells[seedID].points.Add(addGridPoint);
    }

    private void CullInnerPoints()
    {
        List<GridPoint> newCellPointList;
        int pointX;
        int pointY;
        int pointZ;
        bool onRim;

        int x, y, z;
        int pointIndex;

        for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
        {
            newCellPointList = new List<GridPoint>();
            for (pointIndex = 0; pointIndex < cells[cellIndex].points.Count; pointIndex++)
            {
                pointX = cells[cellIndex].points[pointIndex].x;
                pointY = cells[cellIndex].points[pointIndex].y;
                pointZ = cells[cellIndex].points[pointIndex].z;
                onRim = false;
                if (pointX == 0 || pointX == resolution - 1 || pointY == 0 || pointY == resolution - 1 || pointZ == 0 || pointZ == resolution - 1) onRim = true;
                else
                {
                    for (x = pointX - 1; x <= pointX + 1; x++) //Get neighbors on x-axis
                    {
                        for (y = pointY - 1; y <= pointY + 1; y++) //Get neighbors on y-axis
                        {
                            for (z = pointZ - 1; z >= pointZ + 1; z++) //Get neighbors on z-axis
                            {
                                if (grid[x][y][z] > -1 && grid[pointX][pointY][pointZ] != grid[x][y][z])
                                {
                                    onRim = true;
                                    addGridPoint.x = pointX;
                                    addGridPoint.y = pointY;
                                    addGridPoint.z = pointZ;
                                }
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
                    grid[pointX][pointY][pointZ] = -1;
                }
            }
            cells[cellIndex].points = newCellPointList;
        }
    }

    private bool CheckBaseCase(int seed1, int seed2, int seed3, int seed4, int seed5, int seed6, int seed7, int seed8)
    {
        if (seed1 == seed2 && seed1 == seed3 && seed1 == seed4 && seed1 == seed5 && seed1 == seed6 && seed1 == seed7 && seed1 == seed8) return true;
        else return false;
    }

    private int FindNearestSeed(int x, int y, int z)
    {
        var center = GridPointCenter(x, y, z);

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

    private Vector3 GridPointCenter(int x, int y, int z)
    {
        return new Vector3(
        origin.x + (x * size / resolution) + 0.5f * size / resolution,
        origin.y + (y * size / resolution) + 0.5f * size / resolution,
        origin.y + (z * size / resolution) + 0.5f * size / resolution);
    }

    #endregion
}
