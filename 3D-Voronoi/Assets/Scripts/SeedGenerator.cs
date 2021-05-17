using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedGenerator : MonoBehaviour
{
    [Header("Debugging")]
    public bool drawArea = false;
    public bool drawSeeds = false;
    public bool rerun;

    [Header("Algorithm Stuff")]
    public int resolution;
    private Vector3 origin;
    private List<Vector3> seeds;
    public float size;

    private void Update()
    {
        if (drawArea) DrawArea();
        if (drawSeeds) DrawSeeds();
        if (rerun)
        {
            rerun = false;
            GenerateSeeds();
        }
    }

    private void OnDrawGizmos()
    {
        if (drawArea) DrawArea();
    }

    //----------DEBUGGING METHODS----------
    #region debugging
    private void DrawArea()
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
        if(seeds!= null)
        {
            for (int i = 0; i < seeds.Count; i++)
            {
                Debug.DrawLine(seeds[i] - new Vector3(size, 0, 0), seeds[i] + new Vector3(size, 0, 0), Color.white);
                Debug.DrawLine(seeds[i] - new Vector3(0, size, 0), seeds[i] + new Vector3(0, size, 0), Color.white);
                Debug.DrawLine(seeds[i] - new Vector3(0, 0, size), seeds[i] + new Vector3(0, 0, size), Color.white);
            }
        }
    }


    #endregion

    //----------ALGORITHM METHODS----------
    #region algorithm
    public List<Vector3> GenerateSeeds()
    {
        origin = transform.position;

        /*
        seeds = new List<Vector3>()
        {
            new Vector3(4.88f, 2.82f, 7.94f),
            new Vector3(3.6f, 8.16f, 6.83f),
            new Vector3(9.67f, 5.69f, 1.65f)
        };
        */

        seeds = new List<Vector3>();
        int x, y, z;
        for(x=0; x<resolution; x++)
        {
            for(y=0; y<resolution; y++)
            {
                for(z=0; z<resolution; z++)
                {
                    var seed = new Vector3(
                        origin.x + (x * size / resolution) + 0.5f * size / resolution,
                        origin.y + (y * size / resolution) + 0.5f * size / resolution,
                        origin.y + (z * size / resolution) + 0.5f * size / resolution);
                    float range = (size / resolution / 2) - 0.1f * size / resolution;
                    seed.x = Random.Range(seed.x - range, seed.x + range);
                    seed.y = Random.Range(seed.y - range, seed.y + range);
                    seed.z = Random.Range(seed.z - range, seed.z + range);
                    seeds.Add(seed);
                }
            }
        }
        return seeds;
    }
    #endregion
}
