using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public static void GenerateMesh(List<CellPlane> planes, int cellId, Material polyHedronMaterial)
    {
        GameObject polyHedron = new GameObject();
        Mesh polyHedronMesh = new Mesh();
        var cell = GameObject.FindGameObjectWithTag("DivideAndConquer").GetComponent<MemDivideAndConquer3D>().cells[cellId];

        List<Vector3> vertices = new List<Vector3>();
        List<int> tris = new List<int>();
        for(int i = 0; i < planes.Count; i++)
        {
            GenerateSingleFace(planes[i], ref vertices, ref tris);
        }

        polyHedron.transform.position = cell.seed;
        for(int i = 0; i< vertices.Count; i++)
        {
            vertices[i] = vertices[i] - cell.seed;
        }

        polyHedronMesh.vertices = vertices.ToArray();
        polyHedronMesh.triangles = tris.ToArray();

        //Set Normals, uvs, and color
        Vector3[] normals = new Vector3[vertices.Count];
        Vector2[] uv = new Vector2[vertices.Count];
        Color[] colors = new Color[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            normals[i] = -Vector3.forward;
            uv[i] = new Vector2(0, 0);
            colors[i] = cell.color;
        }
        polyHedronMesh.normals = normals;
        polyHedronMesh.uv = uv;
        polyHedronMesh.colors = colors;


        var meshFilter = polyHedron.AddComponent<MeshFilter>();
        var meshRenderer = polyHedron.AddComponent<MeshRenderer>();
        meshFilter.mesh = polyHedronMesh;
        meshRenderer.material = polyHedronMaterial;

        //Instantiate(polyHedron, cell.seed, Quaternion.identity);

    }

    private static void GenerateSingleFace(CellPlane plane, ref List<Vector3> vertices, ref List<int> tris)
    {
        Vector3[] vertexArray = new Vector3[plane.vertices.Count + 1];
        for(int i = 0; i <plane.vertices.Count; i++)
        {
            vertexArray[i] = plane.vertices[i].position;
        }
        vertexArray[vertexArray.Length - 1] = plane.CenterPoint();

        //int[] trisArray = new int[plane.vertices.Count * 2 * 3];
        //List<int> trisArray = new List<int>();

        for(int i = 0; i < vertexArray.Length-1; i++)
        {
            if(i == vertexArray.Length - 2)
            {
                tris.Add(i + vertices.Count);
                tris.Add(0 + vertices.Count);
                tris.Add(vertexArray.Length - 1 + vertices.Count);

                tris.Add(vertexArray.Length - 1 + vertices.Count);
                tris.Add(0 + vertices.Count);
                tris.Add(i + vertices.Count);
            }
            else
            {
                tris.Add(i + vertices.Count);
                tris.Add(i + 1 + vertices.Count);
                tris.Add(vertexArray.Length - 1 + vertices.Count);

                tris.Add(vertexArray.Length - 1 + vertices.Count);
                tris.Add(i + 1 + vertices.Count);
                tris.Add(i + vertices.Count);
            }
        }

        foreach (Vector3 vertex in vertexArray)
        {
            vertices.Add(vertex);
        }

        //mesh.triangles = tris.ToArray();
    }
}
