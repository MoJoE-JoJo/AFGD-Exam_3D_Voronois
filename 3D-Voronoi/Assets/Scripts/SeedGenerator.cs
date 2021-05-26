using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedGenerator : MonoBehaviour
{
    private Vector3Int resolution;
    private Vector3 origin;
    private List<Vector3> seeds;
    private Vector3 size;

    public void Init(Vector3Int resolution, Vector3 origin, Vector3 size)
    {
        this.resolution = resolution;
        this.origin = origin;
        this.size = size;
    }

    public void Run()
    {
        GenerateSeeds();
    }

    //----------DEBUGGING METHODS----------
    #region debugging
    public void DrawSeeds()
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
        for(x=0; x<resolution.x; x++)
        {
            for(y=0; y<resolution.y; y++)
            {
                for(z=0; z<resolution.z; z++)
                {
                    var seed = new Vector3(
                        origin.x + (x * size.x / resolution.x) + 0.5f * size.x / resolution.x,
                        origin.y + (y * size.y / resolution.y) + 0.5f * size.y / resolution.y,
                        origin.y + (z * size.z / resolution.z) + 0.5f * size.z / resolution.z);
                    float rangeX = (size.x / resolution.x / 2) - 0.1f * size.x / resolution.x;
                    float rangeY = (size.y / resolution.y / 2) - 0.1f * size.y / resolution.y;
                    float rangeZ = (size.z / resolution.z / 2) - 0.1f * size.z / resolution.z;

                    seed.y = Random.Range(seed.y - rangeX, seed.y + rangeX);
                    seed.x = Random.Range(seed.x - rangeY, seed.x + rangeY);
                    seed.z = Random.Range(seed.z - rangeZ, seed.z + rangeZ);
                    seeds.Add(seed);
                }
            }
        }
        return seeds;
    }
    #endregion
}
