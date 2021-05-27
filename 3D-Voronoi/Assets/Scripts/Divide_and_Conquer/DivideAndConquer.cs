using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class DivideAndConquer : MonoBehaviour
{
    [Header("Debugging")]
    protected DEBUGDRAWTYPE debugType;
    protected DnCDebugPoint debugPoint = new DnCDebugPoint();

    [Header("Algorithm Stuff")]
    protected GridPoint addGridPoint = new GridPoint();
    protected Vector3 pointVector = new Vector3();

    public int[][][] grid;

    public List<VCell> cells;
    public List<Vector3> seedPoints;
    protected Vector3Int resolution;
    protected Vector3 origin;

    protected Vector3 size;

    #region run
    virtual public void Init(List<Vector3> seeds, Vector3Int resolution, Vector3 origin, Vector3 size, DEBUGDRAWTYPE debugType)
    {
        this.resolution = resolution;
        this.origin = origin;
        this.size = size;
        this.debugType = debugType;
        SetSeeds(seeds);
        InitGrid();
    }

    public void Run()
    {
        DividAndConquer();
    }

    #endregion
    #region debug
    public void DrawPointGrid()
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

    protected void DebugDrawPoint(int id, int x, int y, int z)
    {

        //var newPoint = new DnCDebugPoint();

        debugPoint.center = GridPointCenter(x, y, z);


        debugPoint.x = size.x / resolution.x;
        debugPoint.y = size.y / resolution.y;
        debugPoint.z = size.z / resolution.z;
        debugPoint.DebugDraw(cells[id].color);
    }

    #endregion

    #region algorithm
    abstract protected void DividAndConquer();

    private void SetSeeds(List<Vector3> seeds)
    {
        seedPoints = seeds;
        //origin = transform.position;
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

    public void InitGrid()
    {
        int x, y, z;
        int j;
        grid = new int[resolution.x][][];
        for (int i = 0; i < grid.Length; i++)
        {
            grid[i] = new int[resolution.y][];
            for (j = 0; j < grid[i].Length; j++)
            {
                grid[i][j] = new int[resolution.z];
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

    protected void BaseCase(int id, int leftX, int rightX, int bottomY, int topY, int frontZ, int backZ)
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
        if (debugType == DEBUGDRAWTYPE.DRAWSQUARES || debugType == DEBUGDRAWTYPE.DRAWEDGE || debugType == DEBUGDRAWTYPE.RUN)
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

    protected void TrivialCase(int x, int y, int z)
    {

        var seedID = FindNearestSeed(x, y, z);

        grid[x][y][z] = seedID;

        addGridPoint.x = x;
        addGridPoint.y = y;
        addGridPoint.z = z;
        cells[seedID].points.Add(addGridPoint);
    }

    protected void CullInnerPoints()
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
                    ((pointY == 0 || pointY == resolution.y - 1) && (pointZ == 0 || pointZ == resolution.z - 1)) ||
                    ((pointX == 0 || pointX == resolution.x - 1) && (pointZ == 0 || pointZ == resolution.z - 1)) ||
                    ((pointX == 0 || pointX == resolution.x - 1) && (pointY == 0 || pointY == resolution.y - 1))
                    ) onRim = true;
                else if (
                    CheckCullForXYPlanes(pointX, pointY, pointZ) ||
                    CheckCullForYZPlanes(pointX, pointY, pointZ) ||
                    CheckCullForXZPlanes(pointX, pointY, pointZ)
                    ) onRim = true;
                else if (pointX == 0 || pointX == resolution.x - 1 || pointY == 0 || pointY == resolution.y - 1 || pointZ == 0 || pointZ == resolution.z - 1) onRim = false;

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

    protected bool CheckCullForXYPlanes(int pointX, int pointY, int pointZ)
    {
        int x, y;
        if (pointZ == 0 || pointZ == resolution.z - 1)
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

    protected bool CheckCullForYZPlanes(int pointX, int pointY, int pointZ)
    {
        int y, z;
        if (pointX == 0 || pointX == resolution.x - 1)
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
    protected bool CheckCullForXZPlanes(int pointX, int pointY, int pointZ)
    {
        int x, z;
        if (pointY == 0 || pointY == resolution.y - 1)
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

    protected bool CheckBaseCase(int seed1, int seed2, int seed3, int seed4, int seed5, int seed6, int seed7, int seed8)
    {
        if (seed1 == seed2 && seed1 == seed3 && seed1 == seed4 && seed1 == seed5 && seed1 == seed6 && seed1 == seed7 && seed1 == seed8) return true;
        else return false;
    }

    virtual protected int FindNearestSeed(int x, int y, int z)
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

    public Vector3 GridPointCenter(int x, int y, int z)
    {
        pointVector.x = origin.x + (x * size.x / resolution.x) + 0.5f * size.x / resolution.x;
        pointVector.y = origin.y + (y * size.y / resolution.y) + 0.5f * size.y / resolution.y;
        pointVector.z = origin.y + (z * size.z / resolution.z) + 0.5f * size.z / resolution.z;
        return pointVector;
    }
#endregion
}
