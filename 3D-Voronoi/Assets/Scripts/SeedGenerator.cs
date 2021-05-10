using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedGenerator : MonoBehaviour
{
    
    public List<Vector3> GenerateSeeds()
    {
        var seeds = new List<Vector3>()
        {
            new Vector3(4.88f, 2.82f, 7.94f),
            new Vector3(3.6f, 8.16f, 6.83f),
            new Vector3(9.67f, 5.69f, 1.65f)
        };
        return seeds;
    }
}
