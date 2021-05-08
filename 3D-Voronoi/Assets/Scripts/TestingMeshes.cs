using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingMeshes : MonoBehaviour
{
    public GameObject cube;
    public MeshFilter cubeMesh;
    public Mesh weirdMesh;

    // Start is called before the first frame update
    void Start()
    {
        weirdMesh = new Mesh();
        Vector3[] vertices = new Vector3[4]
        {
            new Vector3(0, 1, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 0)
        };
        weirdMesh.vertices = vertices;
        cubeMesh.mesh = weirdMesh;

        int[] tris = new int[12]
        {
            // lower left triangle
            0, 2, 1,
            // upper right triangle
            2, 3, 1,
            1,2,0,
            1,3,2
        };
        weirdMesh.triangles = tris;

        Vector3[] normals = new Vector3[4]
        {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward
        };
        weirdMesh.normals = normals;

        Vector2[] uv = new Vector2[4]
        {
              new Vector2(0, 0),
              new Vector2(0, 0),
              new Vector2(0, 0),
              new Vector2(0, 0)
        };
        weirdMesh.uv = uv;

        Color[] colors = new Color[4]
        {
            Color.green,
            Color.red,
            Color.yellow,
            Color.cyan
        };

        weirdMesh.colors = colors;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
