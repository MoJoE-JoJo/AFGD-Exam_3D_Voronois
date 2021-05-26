using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedGenerator : MonoBehaviour
{
    private int resolution;
    private Vector3 origin;
    private List<Vector3> seeds;
    private float size;

    public void Init(int resolution, Vector3 origin, float size)
    {
        this.resolution = resolution;
        this.origin = origin;
        this.size = size;
    }

    public void Run(int seed)
    {
        GenerateSeeds(seed);
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
    public List<Vector3> GenerateSeeds(int randomSeed)
    {
        Random.InitState(randomSeed);
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
                    seed.y = Random.Range(seed.y - range, seed.y + range);
                    seed.x = Random.Range(seed.x - range, seed.x + range);
                    seed.z = Random.Range(seed.z - range, seed.z + range);
                    seeds.Add(seed);
                }
            }
        }
        return seeds;
    }
    #endregion
}
