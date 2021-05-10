using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAlgorithm : MonoBehaviour
{
    [Header("-----Run Algorithm-----")]
    public bool run;

    [Header("-----Algorithm Phase Implementations-----")]
    public SeedGenerator seedGenerator;
    public MemDivideAndConquer3D divideAndConquer;
    public FloodGraphGenerator floodGraphGenerator;
    public RenderStep renderStep;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (run)
        {
            run = false;
            Run();
        }
    }

    public void Run()
    {
        //Seed Generation
        var seeds = seedGenerator.GenerateSeeds();

        //Divide and conquer
        divideAndConquer.Run(seeds);


        //Flooding, combining

        //Render output
    }
}
