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

        GenerateSingleFace(planes[0], ref polyHedronMesh, cell.color);

        var meshFilter = polyHedron.AddComponent<MeshFilter>();
        var meshRenderer = polyHedron.AddComponent<MeshRenderer>();
        meshFilter.mesh = polyHedronMesh;
        meshRenderer.material = polyHedronMaterial;

        //Instantiate(polyHedron, cell.seed, Quaternion.identity);

    }

    private static void GenerateSingleFace(CellPlane plane, ref Mesh mesh, Color color)
    {
        Vector3[] vertices = new Vector3[plane.vertices.Count + 1];
        for(int i = 0; i <plane.vertices.Count; i++)
        {
            vertices[i] = plane.vertices[i].position;
        }
        vertices[vertices.Length - 1] = plane.CenterPoint();

        mesh.vertices = vertices;

        //int[] trisArray = new int[plane.vertices.Count * 2 * 3];
        List<int> tris = new List<int>();

        for(int i = 0; i < vertices.Length-1; i++)
        {
            if(i == vertices.Length - 2)
            {
                tris.Add(i);
                tris.Add(0);
                tris.Add(vertices.Length - 1);

                tris.Add(vertices.Length - 1);
                tris.Add(0);
                tris.Add(i);
            }
            else
            {
                tris.Add(i);
                tris.Add(i + 1);
                tris.Add(vertices.Length - 1);

                tris.Add(vertices.Length - 1);
                tris.Add(i + 1);
                tris.Add(i);
            }
        }

        mesh.triangles = tris.ToArray();

        Vector3[] normals = new Vector3[vertices.Length];
        Vector2[] uv = new Vector2[vertices.Length];
        Color[] colors = new Color[vertices.Length];
        for (int i = 0; i <vertices.Length; i++)
        {
            normals[i] = -Vector3.forward;
            uv[i] = new Vector2(0, 0);
            colors[i] = Color.green;
        }
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.colors = colors;

    }
}
