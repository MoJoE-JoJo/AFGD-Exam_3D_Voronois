using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiArea : MonoBehaviour
{
    public Vector3 Origin { get => transform.position; }
    private Vector3 size;
    public void Init(Vector3 size)
    {
        this.size = size;
    }

    public void DrawGridArea()
    {
        //front points
        var rightBottomFront = Origin;
        var leftTopFront = Origin;
        var rightTopFront = Origin;

        //back points
        var leftBottomBack = Origin;
        var rightBottomBack = Origin;
        var leftTopBack = Origin;
        var rightTopBack = Origin;

        //rightBottomFront
        rightBottomFront.x += size.x;

        //leftTopFront
        leftTopFront.y += size.y;

        //rightTopFront
        rightTopFront.x += size.x;
        rightTopFront.y += size.y;

        //leftBottomBack
        leftBottomBack.z += size.z;

        //rightBottomBack
        rightBottomBack.x += size.x;
        rightBottomBack.z += size.z;

        //leftTopBack
        leftTopBack.y += size.y;
        leftTopBack.z += size.z;

        //rightTopBack
        rightTopBack.x += size.x;
        rightTopBack.y += size.y;
        rightTopBack.z += size.z;

        Debug.DrawLine(Origin, rightBottomFront, Color.white);
        Debug.DrawLine(Origin, leftTopFront, Color.white);
        Debug.DrawLine(rightBottomFront, rightTopFront, Color.white);
        Debug.DrawLine(leftTopFront, rightTopFront, Color.white);

        Debug.DrawLine(leftBottomBack, rightBottomBack, Color.white);
        Debug.DrawLine(leftBottomBack, leftTopBack, Color.white);
        Debug.DrawLine(rightBottomBack, rightTopBack, Color.white);
        Debug.DrawLine(leftTopBack, rightTopBack, Color.white);

        Debug.DrawLine(Origin, leftBottomBack, Color.white);
        Debug.DrawLine(rightBottomFront, rightBottomBack, Color.white);
        Debug.DrawLine(leftTopFront, leftTopBack, Color.white);
        Debug.DrawLine(rightTopFront, rightTopBack, Color.white);


    }
}
