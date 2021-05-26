using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunAlgorithm : MonoBehaviour
{
    [Header("-----Run Algorithm-----")]
    [SerializeField] private bool run;

    [Header("-----Algorithm Phase Implementations-----")]
    [SerializeField] private VoronoiArea area;
    [SerializeField] private SeedGenerator seedGenerator;
    [SerializeField] private DivideAndConquer divideAndConquer;
    [SerializeField] private FloodGraphGenerator floodGraphGenerator;
    [SerializeField] private RenderStep renderStep;

    [Header("-----Algorithm Parameters-----")]
    [Tooltip("Random if 0, else it is deterministic")]public int randomSeed;
    [SerializeField] private GameObject polyhedronParent;
    [SerializeField] private Material polyhedronMaterial;
    [SerializeField] private bool useColor = true;
    [SerializeField] private Color polyhedronColor;

    [SerializeField] private Vector3 areaSize;
    [SerializeField] [Tooltip("Values that are too different can result in faulty results")] private Vector3Int resolutionDivideAndConquer;
    [SerializeField] private Vector3Int resolutionSeed;




    [Header("-----Algorithm Debugging-----")]
    [SerializeField] private bool drawArea;
    [SerializeField] private bool drawSeeds;
    [SerializeField] private DEBUGDRAWTYPE dacDrawType;
    [SerializeField] private bool drawDivideAndConquer;
    [SerializeField] private bool drawFloodedGraph;
    [SerializeField] private bool drawFaceCentroids;
    [SerializeField] private bool hidePolyhedrons;

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
                floodGraphGenerator.Init(divideAndConquer, resolutionDivideAndConquer);
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
        floodGraphGenerator.Init(divideAndConquer, resolutionDivideAndConquer);
        floodGraphInited = true;
        var vertices = floodGraphGenerator.Run();

        //Render output
        renderStep.Init(polyhedronMaterial, polyhedronParent);
        renderInited = true;
        renderStep.Run(divideAndConquer, vertices, useColor, polyhedronColor);

    }
}
