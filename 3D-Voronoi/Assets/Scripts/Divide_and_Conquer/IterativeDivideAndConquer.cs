using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IterativeDivideAndConquer : DivideAndConquer
{
    [Header("Debugging")]
    public bool drawArea = false;
    public bool drawSeeds = false;
    public bool drawDivideAndConquer = false;
    public DEBUGDRAWTYPE debugType = DEBUGDRAWTYPE.DRAWEDGE;
    private DnCDebugPoint debugPoint = new DnCDebugPoint();

    /*
    [Header("Redraw")]
    public bool rerun;
    */

    [Header("Algorithm Stuff")]
    private GridPoint addGridPoint = new GridPoint();
    private Vector3 pointVector = new Vector3();
    private Queue<(int, int, int, int, int, int)> pointsToCheck = new Queue<(int, int, int, int, int, int)>();
    //private Vector3 gridPointCenterVector = new Vector3();
    //[Range(0f, 1f)]
    private List<Vector3> seedPoints;
    public int resolution;
    private Vector3 origin;

    public float size;
    public List<VCell> cells;

    public int[][][] grid;


    void Start()
    {
        //SetSeeds();
        //InitGrid();
        //DividAndConquer();
    }
    private void Update()
    {
        /*
        if (rerun)
        {
            rerun = false;
            SetSeeds();
            InitGrid();
            DividAndConquer();
        }
        */
        if (drawArea) DrawGridArea();
        if (drawSeeds && seedPoints != null) DrawSeeds();
        if (drawDivideAndConquer && grid != null) DrawPointGrid();
    }

    private void OnDrawGizmos()
    {
        if (drawArea) DrawGridArea();
    }

    //----------DEBUGGING METHODS----------
    #region debugging
    private void DrawGridArea()
    {
        origin = transform.position;
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
    override public void Run(List<Vector3> seeds)
    {
        SetSeeds(seeds);
        InitGrid();
        DividAndConquer();
    }
    private void SetSeeds(List<Vector3> seeds)
    {
        seedPoints = seeds;
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
            for (j = 0; j < grid[i].Length; j++)
            {
                grid[i][j] = new int[resolution];
            }
        }
        for (x = 0; x < grid.Length; x++)
        {
            for (y = 0; y < grid[x].Length; y++)
            {
                for (z = 0; z < grid[x][y].Length; z++)
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
            pointsToCheck.Enqueue((0, halfPoint, 0, halfPoint, 0, halfPoint));
            pointsToCheck.Enqueue((halfPoint + 1, (resolution - 1), 0, halfPoint, 0, halfPoint));
            pointsToCheck.Enqueue((0, halfPoint, halfPoint + 1, (resolution - 1), 0, halfPoint));
            pointsToCheck.Enqueue((halfPoint + 1, (resolution - 1), halfPoint + 1, (resolution - 1), 0, halfPoint));

            pointsToCheck.Enqueue((0, halfPoint, 0, halfPoint, halfPoint + 1, (resolution - 1)));
            pointsToCheck.Enqueue((halfPoint + 1, (resolution - 1), 0, halfPoint, halfPoint + 1, (resolution - 1)));
            pointsToCheck.Enqueue((0, halfPoint, halfPoint + 1, (resolution - 1), halfPoint + 1, (resolution - 1)));
            pointsToCheck.Enqueue((halfPoint + 1, (resolution - 1), halfPoint + 1, (resolution - 1), halfPoint + 1, (resolution - 1)));

            int leftX, rightX, bottomY, topY, frontZ, backZ;
            while (pointsToCheck.Count > 0)
            {
                var range = pointsToCheck.Dequeue();
                leftX = range.Item1;
                rightX = range.Item2;
                bottomY = range.Item3;
                topY = range.Item4;
                frontZ = range.Item5;
                backZ = range.Item6;

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
                else if (leftX != rightX && bottomY == topY && frontZ != backZ)
                {
                    //add 4
                    TrivialCase(leftX, topY, frontZ);
                    TrivialCase(leftX, topY, backZ);
                    TrivialCase(rightX, topY, frontZ);
                    TrivialCase(rightX, topY, backZ);
                }

                else
                {
                    //Step 1: calculate Points
                    leftBottomFrontID = FindNearestSeed(leftX, bottomY, frontZ);
                    rightBottomFrontID = FindNearestSeed(rightX, bottomY, frontZ);
                    leftTopFrontID = FindNearestSeed(leftX, topY, frontZ);
                    rightTopFrontID = FindNearestSeed(rightX, topY, frontZ);

                    leftBottomBackID = FindNearestSeed(leftX, bottomY, backZ);
                    rightBottomBackID = FindNearestSeed(rightX, bottomY, backZ);
                    leftTopBackID = FindNearestSeed(leftX, topY, backZ);
                    rightTopBackID = FindNearestSeed(rightX, topY, backZ);
                    //Step 2: Base Case
                    if (CheckBaseCase(leftBottomFrontID, leftTopFrontID, rightBottomFrontID, rightTopFrontID, leftBottomBackID, leftTopBackID, rightBottomBackID, rightTopBackID))
                    {
                        BaseCase(leftBottomFrontID, leftX, rightX, bottomY, topY, frontZ, backZ);
                    }
                    //Step 3: Subdivide Case
                    else
                    {
                        int halfPointX = leftX + (rightX - leftX) / 2;
                        int halfPointY = bottomY + (topY - bottomY) / 2;
                        int halfPointZ = frontZ + (backZ - frontZ) / 2;
                        pointsToCheck.Enqueue((leftX, halfPointX, bottomY, halfPointY, frontZ, halfPointZ));
                        pointsToCheck.Enqueue((halfPointX + 1, rightX, bottomY, halfPointY, frontZ, halfPointZ));
                        pointsToCheck.Enqueue((leftX, halfPointX, halfPointY + 1, topY, frontZ, halfPointZ));
                        pointsToCheck.Enqueue((halfPointX + 1, rightX, halfPointY + 1, topY, frontZ, halfPointZ));

                        pointsToCheck.Enqueue((leftX, halfPointX, bottomY, halfPointY, halfPointZ + 1, backZ));
                        pointsToCheck.Enqueue((halfPointX + 1, rightX, bottomY, halfPointY, halfPointZ + 1, backZ));
                        pointsToCheck.Enqueue((leftX, halfPointX, halfPointY + 1, topY, halfPointZ + 1, backZ));
                        pointsToCheck.Enqueue((halfPointX + 1, rightX, halfPointY + 1, topY, halfPointZ + 1, backZ));
                    }
                }
            }
        }
        if (debugType == DEBUGDRAWTYPE.DRAWEDGE)
        {
            CullInnerPoints();
        }

        else if (!drawDivideAndConquer)
        {
            CullInnerPoints();
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
                    for (z = frontZ; z <= backZ; z++)
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
        addGridPoint.z = z;
        cells[seedID].points.Add(addGridPoint);
    }

    private void CullInnerPoints()
    {
        List<GridPoint> newCellPointList;
        int pointX;
        int pointY;
        int pointZ;
        int tempId = -1;
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

                //Edge of the cube points
                if (
                    ((pointY == 0 || pointY == resolution - 1) && (pointZ == 0 || pointZ == resolution - 1)) ||
                    ((pointX == 0 || pointX == resolution - 1) && (pointZ == 0 || pointZ == resolution - 1)) ||
                    ((pointX == 0 || pointX == resolution - 1) && (pointY == 0 || pointY == resolution - 1))
                    ) onRim = true;
                else if (
                    CheckCullForXYPlanes(pointX, pointY, pointZ) ||
                    CheckCullForYZPlanes(pointX, pointY, pointZ) ||
                    CheckCullForXZPlanes(pointX, pointY, pointZ)
                    ) onRim = true;
                else if (pointX == 0 || pointX == resolution - 1 || pointY == 0 || pointY == resolution - 1 || pointZ == 0 || pointZ == resolution - 1) onRim = false;

                //Inner points
                else
                {
                    for (x = pointX - 1; x <= pointX + 1; x++) //Get neighbors on x-axis
                    {
                        for (y = pointY - 1; y <= pointY + 1; y++) //Get neighbors on y-axis
                        {
                            for (z = pointZ - 1; z <= pointZ + 1; z++) //Get neighbors on z-axis
                            {
                                if (grid[x][y][z] > -1 && grid[pointX][pointY][pointZ] != grid[x][y][z])
                                {
                                    if (tempId == -1) tempId = grid[x][y][z];
                                    else if (tempId > -1 && grid[x][y][z] != tempId) onRim = true;

                                }
                            }
                        }
                    }
                }

                if (onRim)
                {
                    addGridPoint.x = pointX;
                    addGridPoint.y = pointY;
                    addGridPoint.z = pointZ;
                    newCellPointList.Add(addGridPoint);
                }
                else if (!onRim)
                {
                    grid[pointX][pointY][pointZ] = -1;
                }
                tempId = -1;

            }
            cells[cellIndex].points = newCellPointList;
        }
    }

    private bool CheckCullForXYPlanes(int pointX, int pointY, int pointZ)
    {
        int x, y;
        if (pointZ == 0 || pointZ == resolution - 1)
        {
            for (x = pointX - 1; x <= pointX + 1; x++) //Get neighbors on x-axis
            {
                for (y = pointY - 1; y <= pointY + 1; y++) //Get neighbors on y-axis
                {
                    if (grid[x][y][pointZ] > -1 && grid[pointX][pointY][pointZ] != grid[x][y][pointZ])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        return false;
    }

    private bool CheckCullForYZPlanes(int pointX, int pointY, int pointZ)
    {
        int y, z;
        if (pointX == 0 || pointX == resolution - 1)
        {
            for (y = pointY - 1; y <= pointY + 1; y++)
            {
                for (z = pointZ - 1; z <= pointZ + 1; z++)
                {
                    if (grid[pointX][y][z] > -1 && grid[pointX][pointY][pointZ] != grid[pointX][y][z])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        return false;
    }
    private bool CheckCullForXZPlanes(int pointX, int pointY, int pointZ)
    {
        int x, z;
        if (pointY == 0 || pointY == resolution - 1)
        {
            for (x = pointX - 1; x <= pointX + 1; x++)
            {
                for (z = pointZ - 1; z <= pointZ + 1; z++)
                {
                    if (grid[x][pointY][z] > -1 && grid[pointX][pointY][pointZ] != grid[x][pointY][z])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        return false;
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
        float newDistance = 0f;
        for (int i = 1; i < seedPoints.Count; i++)
        {
            newDistance = (seedPoints[i] - center).magnitude;
            if (newDistance < distance)
            {
                distance = newDistance;
                returnID = i;
            }
        }

        return returnID;
    }

    public Vector3 GridPointCenter(int x, int y, int z)
    {
        
        pointVector.x = origin.x + (x * size / resolution) + 0.5f * size / resolution;
        pointVector.y = origin.y + (y * size / resolution) + 0.5f * size / resolution;
        pointVector.z = origin.y + (z * size / resolution) + 0.5f * size / resolution;
        return pointVector;
        /*
        return new Vector3(
        origin.x + (x * size / resolution) + 0.5f * size / resolution,
        origin.y + (y * size / resolution) + 0.5f * size / resolution,
        origin.y + (z * size / resolution) + 0.5f * size / resolution);
        */
    }

    #endregion
}
