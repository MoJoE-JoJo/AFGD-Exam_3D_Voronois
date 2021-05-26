using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IterativeDivideAndConquer : DivideAndConquer
{
    private Queue<(int, int, int, int, int, int)> pointsToCheck = new Queue<(int, int, int, int, int, int)>();
    override protected void DividAndConquer()
    {
        //Step 1: calculate Points
        var leftBottomFrontID = FindNearestSeed(0, 0, 0);
        var rightBottomFrontID = FindNearestSeed(resolution.x - 1, 0, 0);
        var leftTopFrontID = FindNearestSeed(0, resolution.y - 1, 0);
        var rightTopFrontID = FindNearestSeed(resolution.x - 1, resolution.y - 1, 0);

        var leftBottomBackID = FindNearestSeed(0, 0, resolution.z - 1);
        var rightBottomBackID = FindNearestSeed(resolution.x - 1, 0, resolution.z - 1);
        var leftTopBackID = FindNearestSeed(0, resolution.y - 1, resolution.z - 1);
        var rightTopBackID = FindNearestSeed(resolution.x - 1, resolution.y - 1, resolution.z - 1);

        //Step 2: Base Case
        if (CheckBaseCase(leftBottomFrontID, leftTopFrontID, rightBottomFrontID, rightTopFrontID, leftBottomBackID, leftTopBackID, rightBottomBackID, rightTopBackID))
        {
            BaseCase(leftBottomFrontID, 0, resolution.x - 1, 0, resolution.y - 1, 0, resolution.z - 1);
        }
        //Step 3: Subdivide Case
        else
        {
            int halfPointX = (resolution.x - 1) / 2;
            int halfPointY = (resolution.y - 1) / 2;
            int halfPointZ = (resolution.z - 1) / 2;
            pointsToCheck.Enqueue((0, halfPointX, 0, halfPointY, 0, halfPointZ));
            pointsToCheck.Enqueue((halfPointX + 1, (resolution.x - 1), 0, halfPointY, 0, halfPointZ));
            pointsToCheck.Enqueue((0, halfPointX, halfPointY + 1, (resolution.y - 1), 0, halfPointZ));
            pointsToCheck.Enqueue((halfPointX + 1, (resolution.x - 1), halfPointY + 1, (resolution.y - 1), 0, halfPointZ));

            pointsToCheck.Enqueue((0, halfPointX, 0, halfPointY, halfPointZ + 1, (resolution.z - 1)));
            pointsToCheck.Enqueue((halfPointX + 1, (resolution.x - 1), 0, halfPointY, halfPointZ + 1, (resolution.z - 1)));
            pointsToCheck.Enqueue((0, halfPointX, halfPointY + 1, (resolution.y - 1), halfPointZ + 1, (resolution.z - 1)));
            pointsToCheck.Enqueue((halfPointX + 1, (resolution.x - 1), halfPointY + 1, (resolution.y - 1), halfPointZ + 1, (resolution.z - 1)));

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
                        halfPointX = leftX + (rightX - leftX) / 2;
                        halfPointY = bottomY + (topY - bottomY) / 2;
                        halfPointZ = frontZ + (backZ - frontZ) / 2;
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
        if (debugType == DEBUGDRAWTYPE.DRAWEDGE || debugType == DEBUGDRAWTYPE.RUN)
        {
            CullInnerPoints();
        }
    }
}
