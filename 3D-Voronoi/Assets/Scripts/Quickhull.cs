using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GK;

public class Quickhull : MonoBehaviour
{
    public List<Transform> points;
    [SerializeField] private MemDivideAndConquer3D DAC;


    private List<Face> faces;

    // Start is called before the first frame update
    void Start()
    {
        var tmp = new ConvexHullCalculator();
        List<Vector3> newList = new List<Vector3>();
        foreach (var p in points)
        {
            newList.Add(p.position);
        }


        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector3> normals = new List<Vector3>();

        tmp.GenerateHull(newList, false, ref verts, ref tris, ref normals);

        for (int i = 1; i < verts.Count; i++)
        {
            Debug.DrawLine(verts[i - 1], verts[i], Color.red, 200f);
        }
        Debug.DrawLine(verts[0], verts[verts.Count-1], Color.red, 200f);
        Debug.Log("Done???");
    }

    public class Face
    {
        // points making the plane
        public Vector3 A { get; set; }
        public Vector3 B { get; set; }
        public Vector3 C { get; set; }
        // the plane object
        public Plane plane { get; }

        public Face(Vector3 a, Vector3 b, Vector3 c) 
        {
            A = a;
            B = b;
            C = c;
            plane = new Plane(a, b, c);
        }

        public void Draw() 
        {
            Debug.DrawLine(A, B, Color.red);
            Debug.DrawLine(B, C, Color.red);
            Debug.DrawLine(C, A, Color.red);

            //draw normal of the plane
            //var c = CenterPoint();
            //Debug.DrawLine(c, c + plane.normal, Color.green);
        }

        private Vector3 CenterPoint()
        {
            var x = (A.x + B.x + C.x) / 3;
            var y = (A.y + B.y + C.y) / 3;
            var z = (A.z + B.z + C.z) / 3;
            return new Vector3(x, y, z);
        }

    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            // Divide and conquer
            List<GridPoint> indexes = DAC.cells[0].points;
            List<Vector3> lst = new List<Vector3>();

            foreach (GridPoint GP in indexes)
            {
                lst.Add(DAC.GridPointCenter(GP.x, GP.y, GP.z));
            }



            // QuickHull 

            var tmp = new ConvexHullCalculator();
            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector3> normals = new List<Vector3>();

            tmp.GenerateHull(lst, false, ref verts, ref tris, ref normals);

            for (int i = 1; i < verts.Count; i++)
            {
                Debug.DrawLine(verts[i - 1], verts[i], Color.red, 200f);
            }
            Debug.DrawLine(verts[0], verts[verts.Count - 1], Color.red, 200f);

        }
    }

    private List<Face> GenerateInitialFaces(Vector3 A, Vector3 B, Vector3 C, Vector3 D)
    {
        var list = new List<Face>
        {
            new Face(A, B, C),
            new Face(A, D, B),
            new Face(B, D, C)
        };
        return list;
    }


    public void Run()
    {

    }
}
