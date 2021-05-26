using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RenderStep : MonoBehaviour
{
    private Material polyhedronMaterial;
    private GameObject polyhedronParent;
    private List<List<CellPlane>> allCellPlanes;


    public void Init(Material mat, GameObject polyhedronParent)
    {
        allCellPlanes = new List<List<CellPlane>>();
        polyhedronMaterial = mat;
        this.polyhedronParent = polyhedronParent;
    }


    public void Run(DivideAndConquer DAC, HashSet<GraphVertex> vertices)
    {
        allCellPlanes = new List<List<CellPlane>>();
        foreach (VCell item in DAC.cells)
        {
            var cellVertices = vertices.Where(x => x.cellIds.Contains(item.id)).ToList();

            
            var planes = PlaneGenerator.GeneratePlanesForCell(item.id, cellVertices);
            allCellPlanes.Add(planes);

            var go = MeshGenerator.GenerateMesh(planes, item, polyhedronMaterial);
            go.transform.parent = polyhedronParent.transform;
        }
    }

    public void DebugDrawPlaneCenters()
    {
        for(int i = 0; i<allCellPlanes.Count; i++)
        {
            for(int j = 0; j<allCellPlanes[i].Count; j++)
            {
                allCellPlanes[i][j].DebugDrawCenter();
            }
        }
    }

    public void HidePolyhedrons(bool hide)
    {
        polyhedronParent.SetActive(!hide);
    }

}
