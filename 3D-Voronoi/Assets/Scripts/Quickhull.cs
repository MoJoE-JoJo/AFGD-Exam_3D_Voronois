using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quickhull : MonoBehaviour
{
    public List<Transform> points;

    private List<Face> faces;

    // Start is called before the first frame update
    void Start()
    {
       faces =  GenerateInitialFaces(points[0].position, points[1].position, points[2].position, points[3].position);
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
            Run();  
        }

        foreach (var face in faces)
        {
            face.Draw();
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


        //initial hull, select 4 points
        // generate first 4 faces
        //var planes = CreatePlanesBasedOn4points(points[0].position, points[1].position, points[2].position, points[3].position);

        //foreach (var plane in planes)
        //{
        //    plane.
        //}


        // Find Point with highest Y
        // Find Point with Lowest Y

        // Find Point with highest X
        // Find Point with Lowest X

        //select a pair and create a line/plane between

        // an edge and a face are the "same"  

        //fat planes
    }
}
