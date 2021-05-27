using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KDTree
{
    private enum AXIS { X,Y,Z};
    private class Node
    {
        public AXIS splittingAxis;
        public float center;
        public bool isLeaf;
        public Node leftChild;
        public Node rightChild;
        public VecToId[] elements;
    }

    private struct VecToId
    {
        public Vector3 vector;
        public int id;
    }

    private Node root;
    private int leafSize = 1;
    public int counter = 0;

    public void ConstructTree(List<Vector3> points)
    {
        var vecToIdPoints = new List<VecToId>();

        for (int i = 0; i < points.Count; i++)
        {
            vecToIdPoints.Add(new VecToId() { vector = points[i], id = i });
        }

        int depth = 0;
        float center = 0;
        for(int i = 0; i< points.Count; i++)
        {
             center += points[i].x;
        }
        center = center / points.Count;
        root = new Node()
        {
            isLeaf = false,
            center = center,
            splittingAxis = AXIS.X,
            leftChild = null,
            rightChild = null,
            elements = null
        };
        RecConstruct(root, vecToIdPoints, depth);
    }

    private void RecConstruct(Node node, List<VecToId> points, int depth)
    {
        if (points.Count <= leafSize)
        {
            node.elements = new VecToId[points.Count];
            for (int i = 0; i < node.elements.Length; i++)
            {
                node.elements[i] = points[i];
            }
            node.isLeaf = true;
            
        }
        else
        {
            AXIS axis = (AXIS)(depth % 3);
            List<VecToId> leftPoints = new List<VecToId>();
            List<VecToId> rightPoints = new List<VecToId>();

            foreach (VecToId vecToId in points)
            {
                if (axis == AXIS.X)
                {
                    if (vecToId.vector.x < node.center) leftPoints.Add(vecToId);
                    else rightPoints.Add(vecToId);
                }
                else if (axis == AXIS.Y)
                {
                    if (vecToId.vector.y < node.center) leftPoints.Add(vecToId);
                    else rightPoints.Add(vecToId);
                }
                else if (axis == AXIS.Z)
                {
                    if (vecToId.vector.z < node.center) leftPoints.Add(vecToId);
                    else rightPoints.Add(vecToId);
                }
            }
            var newDepth = depth + 1;
            var newAxis = (AXIS)(newDepth % 3);
            var leftCenter = 0f;
            var rightCenter = 0f;
            if (newAxis == AXIS.X)
            {
                for (int i = 0; i < leftPoints.Count; i++)
                {
                    leftCenter += leftPoints[i].vector.x;
                }
                for (int i = 0; i < rightPoints.Count; i++)
                {
                    rightCenter += rightPoints[i].vector.x;
                }
                leftCenter = leftCenter / leftPoints.Count;
                rightCenter = rightCenter / rightPoints.Count;
            }
            else if (newAxis == AXIS.Y)
            {
                for (int i = 0; i < leftPoints.Count; i++)
                {
                    leftCenter += leftPoints[i].vector.y;
                }
                for (int i = 0; i < rightPoints.Count; i++)
                {
                    rightCenter += rightPoints[i].vector.y;
                }
                leftCenter = leftCenter / leftPoints.Count;
                rightCenter = rightCenter / rightPoints.Count;
            }
            else if (newAxis == AXIS.Z)
            {
                for (int i = 0; i < leftPoints.Count; i++)
                {
                    leftCenter += leftPoints[i].vector.z;
                }
                for (int i = 0; i < rightPoints.Count; i++)
                {
                    rightCenter += rightPoints[i].vector.z;
                }
                leftCenter = leftCenter / leftPoints.Count;
                rightCenter = rightCenter / rightPoints.Count;
            }

            node.leftChild = new Node()
            {
                isLeaf = false,
                center = leftCenter,
                splittingAxis = newAxis,
                leftChild = null,
                rightChild = null,
                elements = null
            };
            node.rightChild = new Node()
            {
                isLeaf = false,
                center = rightCenter,
                splittingAxis = newAxis,
                leftChild = null,
                rightChild = null,
                elements = null
            };
            RecConstruct(node.leftChild, leftPoints, newDepth);
            RecConstruct(node.rightChild, rightPoints, newDepth);
        }
    }

    public int FindNearestElement(Vector3 point)
    {
        var champion = new VecToId() { id = -1};
        if (point.x < root.center)
        {
            RecFNE(root.leftChild, point, ref champion);
            var champDis = (champion.vector - point).magnitude;
            var disToCenter = Mathf.Abs(root.center - point.x);
            if (disToCenter <= champDis) RecFNE(root.rightChild, point, ref champion);
        }
        else
        {
            RecFNE(root.rightChild, point, ref champion);
            var champDis = (champion.vector - point).magnitude;
            var disToCenter = Mathf.Abs(root.center - point.x);
            if (disToCenter <= champDis) RecFNE(root.leftChild, point, ref champion);
        }

            return champion.id;
    }

    private void RecFNE(Node node, Vector3 point, ref VecToId champion)
    {
        if (node.isLeaf)
        {
            counter++;
            if (champion.id == -1) champion = node.elements[0];
            else
            {
                var champDis = (champion.vector - point).magnitude;
                var nodeElementDis = (node.elements[0].vector - point).magnitude;
                if (nodeElementDis < 0) Debug.Log(nodeElementDis);
                if (nodeElementDis <= champDis) champion = node.elements[0];
            }
            return;
        }
        if (node.splittingAxis == AXIS.X)
        {
            if (point.x < node.center)
            {
                RecFNE(node.leftChild, point, ref champion);
                var champDis = (champion.vector - point).magnitude;
                var disToCenter = Mathf.Abs(node.center- point.x);
                if(disToCenter<= champDis) RecFNE(node.rightChild, point, ref champion);
            }
            else
            {
                RecFNE(node.rightChild, point, ref champion);
                var champDis = (champion.vector - point).magnitude;
                var disToCenter = Mathf.Abs(node.center - point.x);
                if (disToCenter <= champDis) RecFNE(node.leftChild, point, ref champion);
            }
        }
        else if (node.splittingAxis == AXIS.Y)
        {
            if (point.y < node.center)
            {
                RecFNE(node.leftChild, point, ref champion);
                var champDis = (champion.vector - point).magnitude;
                var disToCenter = Mathf.Abs(node.center - point.y);
                if (disToCenter <= champDis) RecFNE(node.rightChild, point, ref champion);
            }
            else
            {
                RecFNE(node.rightChild, point, ref champion);
                var champDis = (champion.vector - point).magnitude;
                var disToCenter = Mathf.Abs(node.center - point.y);
                if (disToCenter <= champDis) RecFNE(node.leftChild, point, ref champion);
            }
        }
        else if (node.splittingAxis == AXIS.Z)
        {
            if (point.z < node.center)
            {
                RecFNE(node.leftChild, point, ref champion);
                var champDis = (champion.vector - point).magnitude;
                var disToCenter = Mathf.Abs(node.center - point.z);
                if (disToCenter <= champDis) RecFNE(node.rightChild, point, ref champion);
            }
            else
            {
                RecFNE(node.rightChild, point, ref champion);
                var champDis = (champion.vector - point).magnitude;
                var disToCenter = Mathf.Abs(node.center - point.z);
                if (disToCenter <= champDis) RecFNE(node.leftChild, point, ref champion);
            }
        }
    }
}

public class LogarithmicDivideAndConquer : DivideAndConquer
{
    private Queue<(int, int, int, int, int, int)> pointsToCheck = new Queue<(int, int, int, int, int, int)>();
    public KDTree kdTree;
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
        kdTree = new KDTree();
        kdTree.ConstructTree(seedPoints);
        Debug.Log("yolo");
    }
    public int counter = 0;
    override protected int FindNearestSeed(int x, int y, int z)
    {
        counter += resolution.x * resolution.y * resolution.z;
        var center = GridPointCenter(x, y, z);
        var returnID = kdTree.FindNearestElement(center);
        return returnID;
    }
}
