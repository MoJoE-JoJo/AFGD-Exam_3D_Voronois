using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAlgorithm : MonoBehaviour
{
    [Header("-----Run Algorithm-----")]
    public bool run;

    [Header("-----Algorithm Phase Implementations-----")]
    public VoronoiArea area;
    public SeedGenerator seedGenerator;
    public DivideAndConquer divideAndConquer;
    public FloodGraphGenerator floodGraphGenerator;
    public RenderStep renderStep;

    [Header("-----Algorithm Parameters-----")]
    [Tooltip("Random if 0, else it is deterministic")]public int randomSeed;
    public GameObject polyhedronParent;
    public Material polyhedronMaterial;
    public bool useColor = true;
    public Color polyhedronColor;
    public float areaSize;
    public int resolutionDivideAndConquer;
    public int resolutionSeed;
    [Range(1, 10)] public int floodCombineRange = 4;

    [Header("-----Algorithm Debugging-----")]
    public bool drawArea;
    public bool drawSeeds;
    public DEBUGDRAWTYPE dacDrawType;
    public bool drawDivideAndConquer;
    public bool drawFloodedGraph;
    public bool drawFaceCentroids;
    public bool hidePolyhedrons;

    private bool areaInited = false;
    private bool seedsInited = false;
    private bool gridInited = false;
    private bool floodGraphInited = false;
    private bool renderInited = false;

    void Start()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (drawArea)
        {
            if (!areaInited)
            {
                area.Init(areaSize);
                areaInited = true;
            }
            area.DrawGridArea();
        }
        if (drawSeeds)
        {
            if (!seedsInited)
            {
                seedGenerator.Init(resolutionSeed, area.Origin, areaSize);
                seedsInited = true;
            }
            seedGenerator.DrawSeeds();
        }
        if (drawDivideAndConquer)
        {
            if (!gridInited)
            {
                divideAndConquer.InitGrid();
                gridInited = true;
            }
            divideAndConquer.DrawPointGrid();
        }
        if (drawFloodedGraph)
        {
            if (!floodGraphInited)
            {
                floodGraphGenerator.Init(divideAndConquer, resolutionDivideAndConquer, floodCombineRange);
                floodGraphInited = true;
            }
            floodGraphGenerator.DebugDraw();
        }
        if (!renderInited)
        {
            renderStep.Init(polyhedronMaterial, polyhedronParent);
            renderInited = true;
        }
        if (hidePolyhedrons) renderStep.HidePolyhedrons(true);
        else if (!hidePolyhedrons) renderStep.HidePolyhedrons(false);
        if (drawFaceCentroids)
        {
            renderStep.DebugDrawPlaneCenters();
        }

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
        seedGenerator.Init(resolutionSeed, area.Origin, areaSize);
        seedsInited = true;
        if (randomSeed == 0) randomSeed = System.DateTime.Now.Ticks.GetHashCode();
        Random.InitState(randomSeed);
        var seeds = seedGenerator.GenerateSeeds();

        //Divide and conquer
        if (!drawDivideAndConquer) dacDrawType = DEBUGDRAWTYPE.RUN;
        divideAndConquer.Init(seeds, resolutionDivideAndConquer, area.Origin, areaSize, dacDrawType);
        gridInited = true;
        divideAndConquer.Run();

        //Flooding, combining
        floodGraphGenerator.Init(divideAndConquer, resolutionDivideAndConquer, floodCombineRange);
        floodGraphInited = true;
        var vertices = floodGraphGenerator.Run();

        //Render output
        renderStep.Init(polyhedronMaterial, polyhedronParent);
        renderInited = true;
        renderStep.Run(divideAndConquer, vertices, useColor, polyhedronColor);

    }
}
