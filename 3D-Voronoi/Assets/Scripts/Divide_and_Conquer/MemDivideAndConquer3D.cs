using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemDivideAndConquer3D : DivideAndConquer
{
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
            DivideAndConquerRecursive(0, halfPointX, 0, halfPointY, 0, halfPointZ);//LeftBottomFront subdivision
            DivideAndConquerRecursive(halfPointX + 1, (resolution.x - 1), 0, halfPointY, 0, halfPointZ);//RightBottomFront subdivision
            DivideAndConquerRecursive(0, halfPointX, halfPointY + 1, (resolution.y - 1), 0, halfPointZ);//LeftTopFront subdivision
            DivideAndConquerRecursive(halfPointX + 1, (resolution.x - 1), halfPointY + 1, (resolution.y - 1), 0, halfPointZ); //RightTopFront subdivision

            DivideAndConquerRecursive(0, halfPointX, 0, halfPointY, halfPointZ + 1, (resolution.z - 1)); //LeftBottomBack subdivision
            DivideAndConquerRecursive(halfPointX + 1, (resolution.x - 1), 0, halfPointY, halfPointZ + 1, (resolution.z - 1)); //RightBottomBack subdivision
            DivideAndConquerRecursive(0, halfPointX, halfPointY + 1, (resolution.y - 1), halfPointZ + 1, (resolution.z - 1)); //LeftTopBack subdivision
            DivideAndConquerRecursive(halfPointX + 1, (resolution.x - 1), halfPointY + 1, (resolution.y - 1), halfPointZ + 1, (resolution.z - 1)); //RightTopBack subdivision
        }
        if (debugType == DEBUGDRAWTYPE.DRAWEDGE || debugType == DEBUGDRAWTYPE.RUN)
        {
            CullInnerPoints();
        }
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
        else if(leftX != rightX && bottomY == topY && frontZ != backZ)
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

}
