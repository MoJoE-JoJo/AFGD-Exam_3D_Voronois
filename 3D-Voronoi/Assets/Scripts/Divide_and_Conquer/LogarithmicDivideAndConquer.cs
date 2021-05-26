using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcTree
{
    private class Node
    {
        public bool leaf;
        public Vector3 center;
        public Vector3 extents;
        public Node[] children;
        public VecToId[] elements;
    }
    private struct VecToId
    {
        public Vector3 vector;
        public int id;
    }

    private Node root;
    public int leafSize = 1;

    public void ConstructTree(Vector3 origin, Vector3 size, List<Vector3> points)
    {
        var rootExtents = size;
        rootExtents.x = size.x / 2;
        rootExtents.y = size.y / 2;
        rootExtents.z = size.z / 2;
        root = new Node()
        {
            leaf = false,
            center = origin + rootExtents,
            extents = rootExtents,
            children = new Node[8],
            elements = null
        };

        var vecToIdPoints = new List<VecToId>();

        for(int i = 0; i<points.Count; i++)
        {
            vecToIdPoints.Add(new VecToId() { vector = points[i], id = i});
        }
        //Check if only one node is needed
        RecConstruct(root, vecToIdPoints);
    }

    private void DefineChildAndStartNextRecursion(Node current, int childIndex, Vector3 childCenter, List<VecToId> pointsContained)
    {
        var childExtents = current.extents;
        childExtents.x = current.extents.x / 2;
        childExtents.y = current.extents.y / 2;
        childExtents.z = current.extents.z / 2;

        current.children[childIndex] = new Node()
        {
            leaf = false,
            center = childCenter,
            extents = childExtents,
            children = null,
            elements = null
        };
        RecConstruct(current.children[childIndex], pointsContained);
    }

    private void RecConstruct(Node node, List<VecToId> points)
    {
        if (points.Count == leafSize)
        {
            node.elements = new VecToId[points.Count];
            for (int i = 0; i < node.elements.Length; i++)
            {
                node.elements[i] = points[i];
            }
            node.leaf = true;
        }
        else
        {
            node.children = new Node[8];
            List<VecToId> xLeftYBottomZFront = new List<VecToId>();
            List<VecToId> xLeftYBottomZBack = new List<VecToId>();
            List<VecToId> xLeftYTopZFront = new List<VecToId>();
            List<VecToId> xLeftYTopZBack = new List<VecToId>();

            List<VecToId> xRightYBottomZFront = new List<VecToId>();
            List<VecToId> xRightYBottomZBack = new List<VecToId>();
            List<VecToId> xRightYTopZFront = new List<VecToId>();
            List<VecToId> xRightYTopZBack = new List<VecToId>();

            foreach (VecToId vecToId in points)
            {
                if (vecToId.vector.x < node.center.x && vecToId.vector.y < node.center.y && vecToId.vector.z < node.center.z)
                {
                    xLeftYBottomZFront.Add(vecToId);
                }
                else if (vecToId.vector.x < node.center.x && vecToId.vector.y < node.center.y && vecToId.vector.z >= node.center.z)
                {
                    xLeftYBottomZBack.Add(vecToId);
                }
                else if (vecToId.vector.x < node.center.x && vecToId.vector.y >= node.center.y && vecToId.vector.z < node.center.z)
                {
                    xLeftYTopZFront.Add(vecToId);
                }
                else if (vecToId.vector.x < node.center.x && vecToId.vector.y >= node.center.y && vecToId.vector.z >= node.center.z)
                {
                    xLeftYTopZBack.Add(vecToId);
                }

                else if (vecToId.vector.x >= node.center.x && vecToId.vector.y < node.center.y && vecToId.vector.z < node.center.z)
                {
                    xRightYBottomZFront.Add(vecToId);
                }
                else if (vecToId.vector.x >= node.center.x && vecToId.vector.y < node.center.y && vecToId.vector.z >= node.center.z)
                {
                    xRightYBottomZBack.Add(vecToId);
                }
                else if (vecToId.vector.x >= node.center.x && vecToId.vector.y >= node.center.y && vecToId.vector.z < node.center.z)
                {
                    xRightYTopZFront.Add(vecToId);
                }
                else if (vecToId.vector.x >= node.center.x && vecToId.vector.y >= node.center.y && vecToId.vector.z >= node.center.z)
                {
                    xRightYTopZBack.Add(vecToId);
                }
            }

            if (xLeftYBottomZFront.Count > 0)
            {
                var childCenter = node.center;
                childCenter.x -= node.extents.x / 2;
                childCenter.y -= node.extents.y / 2;
                childCenter.z -= node.extents.z / 2;

                DefineChildAndStartNextRecursion(node, 0, childCenter, xLeftYBottomZFront);
            }
            if (xLeftYBottomZBack.Count > 0)
            {
                var childCenter = node.center;
                childCenter.x -= node.extents.x / 2;
                childCenter.y -= node.extents.y / 2;
                childCenter.z += node.extents.z / 2;

                DefineChildAndStartNextRecursion(node, 1, childCenter, xLeftYBottomZBack);
            }
            if (xLeftYTopZFront.Count > 0)
            {
                var childCenter = node.center;
                childCenter.x -= node.extents.x / 2;
                childCenter.y += node.extents.y / 2;
                childCenter.z -= node.extents.z / 2;

                DefineChildAndStartNextRecursion(node, 2, childCenter, xLeftYTopZFront);
            }
            if (xLeftYTopZBack.Count > 0)
            {
                var childCenter = node.center;
                childCenter.x -= node.extents.x / 2;
                childCenter.y += node.extents.y / 2;
                childCenter.z += node.extents.z / 2;

                DefineChildAndStartNextRecursion(node, 3, childCenter, xLeftYTopZBack);
            }
            if (xRightYBottomZFront.Count > 0)
            {
                var childCenter = node.center;
                childCenter.x += node.extents.x / 2;
                childCenter.y -= node.extents.y / 2;
                childCenter.z -= node.extents.z / 2;

                DefineChildAndStartNextRecursion(node, 4, childCenter, xRightYBottomZFront);
            }
            if (xRightYBottomZBack.Count > 0)
            {
                var childCenter = node.center;
                childCenter.x += node.extents.x / 2;
                childCenter.y -= node.extents.y / 2;
                childCenter.z += node.extents.z / 2;

                DefineChildAndStartNextRecursion(node, 5, childCenter, xRightYBottomZBack);
            }
            if (xRightYTopZFront.Count > 0)
            {
                var childCenter = node.center;
                childCenter.x += node.extents.x / 2;
                childCenter.y += node.extents.y / 2;
                childCenter.z -= node.extents.z / 2;

                DefineChildAndStartNextRecursion(node, 6, childCenter, xRightYTopZFront);
            }
            if (xRightYTopZBack.Count > 0)
            {
                var childCenter = node.center;
                childCenter.x += node.extents.x / 2;
                childCenter.y += node.extents.y / 2;
                childCenter.z += node.extents.z / 2;

                DefineChildAndStartNextRecursion(node, 7, childCenter, xRightYTopZBack);
            }
        }
    }




    public int FindNearestElement(Vector3 point)
    {

        return 0;
    }

}

public class LogarithmicDivideAndConquer : DivideAndConquer
{
    private Queue<(int, int, int, int, int, int)> pointsToCheck = new Queue<(int, int, int, int, int, int)>();
    private OcTree ocTree;
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

    override public void Init(List<Vector3> seeds, Vector3Int resolution, Vector3 origin, Vector3 size, DEBUGDRAWTYPE debugType)
    {
        base.Init(seeds, resolution, origin, size, debugType);
        BuildTree();
    }

    private void BuildTree()
    {
        ocTree = new OcTree();
        ocTree.ConstructTree(origin, size, seedPoints);
        Debug.Log("yolo");
    }

    override protected int FindNearestSeed(int x, int y, int z)
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
}
