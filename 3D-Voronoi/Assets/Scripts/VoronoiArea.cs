using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiArea : MonoBehaviour
{
    public Vector3 Origin { get => transform.position; }
    private float size;
    public void Init(float size)
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
