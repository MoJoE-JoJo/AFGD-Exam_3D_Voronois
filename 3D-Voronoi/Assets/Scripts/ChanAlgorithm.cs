using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChanAlgorithm : MonoBehaviour
{
    [SerializeField]
    private List<Transform> initialPoints;

    public class Point
    {
        //for debugging purpose
        public string name = "";
        public Vector3 pos;
        public Point prev, next;

        public float x
        {
            get { return pos.x; }
            set { pos.x = value; }
        }
        public float y
        {
            get { return pos.y; }
            set { pos.y = value; }
        }
        public float z
        {
            get { return pos.z; }
            set { pos.z = value; }
        }

        public void AddOrRemove()
        {
            //if prev.next equal this point, remove it from the linked list
            if (prev.next == this)
            {
                prev.next = next;
                next.prev = prev;
            }
            else // if not, make sure next and prev points to this
            {
                prev.next = this;
                next.prev = this;
            }
        }
    }
    private static float INF = float.MaxValue;
    private Point empty;

    private List<Quickhull.Face> faces;
    // Start is called before the first frame update
    void Start()
    {
        faces = new List<Quickhull.Face>();

        empty = new Point
        {
            pos = new Vector3(INF, INF, INF),
            prev = empty,
            next = empty,
            name = "empty"
        };

        //testing 
        var lst = new List<Point>();

        foreach (var p in initialPoints)
        {
            var point = new Point
            {
                pos = p.position,
                name = p.gameObject.name,
                prev = null,
                next = null
            };
            lst.Add(point);
        }
        int n = lst.Count;
        // this merge sort should connect each point togehter like a linked list in a sorted manor. 
        Point sortedLinkedList = Sort(lst.ToArray(), lst.Count);

        Point[] A = new Point[2 * n];
        Point[] B = new Point[2 * n];
        Point[] P = new Point[n];

        // this is replace c++ way of prefilling structs in a array
        for (int i = 0; i < A.Length; i++)
        {
            A[i] = new Point();
            B[i] = new Point();
        }
        //find lower hull
        Hull(false, sortedLinkedList, n, A, B, 0, 0);

        for (int i = 0; A[i] != empty; A[i++].AddOrRemove())
        {
            var face = A[i];
            Debug.Log(face.prev.name + " " + face.name + " " + face.next.name);

            faces.Add(new Quickhull.Face(face.prev.pos, face.pos, face.next.pos));

            //Debug.DrawLine(face.prev.pos, face.pos, Color.red, 200f);
            //Debug.DrawLine(face.pos, face.next.pos, Color.red, 200f);
            //Debug.DrawLine(face.next.pos, face.prev.pos, Color.red, 200f);
        }

        //reset arrays and resort the list

        sortedLinkedList = Sort(lst.ToArray(), lst.Count);
        for (int i = 0; i < A.Length; i++)
        {
            A[i] = new Point();
            B[i] = new Point();
        }
        //find upper hull
        Hull(true, sortedLinkedList, n, A, B, 0, 0);

        Debug.Log("ROUND 2");
        for (int i = 0; A[i] != empty; A[i++].AddOrRemove())
        {
            var face = A[i];
            Debug.Log(face.prev.name + " " + face.name + " " + face.next.name);

            faces.Add(new Quickhull.Face(face.prev.pos, face.pos, face.next.pos));

            //Debug.DrawLine(face.prev.pos, face.pos, Color.red, 200f);
            //Debug.DrawLine(face.pos, face.next.pos, Color.red, 200f);
            //Debug.DrawLine(face.next.pos, face.prev.pos, Color.red, 200f);
        }

        //res = res.Where(x => x != null).ToArray();
        //Dictionary<Point, bool> dict = new Dictionary<Point, bool>();
        //for (int i = 0; i < res.Length; i++)
        //{
        //    Point current = res[i];
        //    while (current != empty && current != null)
        //    {
        //        if (!dict.ContainsKey(current)) dict.Add(current, true);
        //        current = current.next;
        //    }
        //}
        Debug.Log("Done?");
    }

    private void Update()
    {
        foreach (Quickhull.Face face in faces)
        {
            face.Draw();
        }
    }



    #region === MergeSort linked list ===
    private T[] Split<T>(T[] array, bool left)
    {
        int startIndex = 0;
        int n = array.Length / 2;
        if (!left)
        {
            startIndex = array.Length / 2;

            if (array.Length % 2 != 0)
            {
                n++;
            }
        }

        T[] rtnArray = new T[n];

        for (int i = 0; i < n; i++)
        {
            rtnArray[i] = array[i + startIndex];
        }
        return rtnArray;
    }

    // "pointer" to a head point
    private Point head = new Point();
    /// <summary>
    /// Takes an array of Points, merge sorts them based on the X axis and then builds them into in a linked list. 
    /// </summary>
    /// <param name="points"></param>
    /// <param name="n"></param>
    /// <returns>The first point of the linked list</returns>
    private Point Sort(Point[] points, int n)
    {
        Point a, b, c;

        if (n == 1)
        {
            points[0].next = empty;
            return points[0];
        }
        // give the "left" half of the array
        var p1 = Split(points, true);
        a = Sort(p1, n / 2);
        // give the right half of the array
        var p2 = Split(points, false);
        b = Sort(p2, n - n / 2);
        c = head;
        int count = 0; //prevent inf loop
        do
        {
            if (a.x < b.x)
            {
                c = c.next = a;
                if (count == 0) // a was the first, assign head to be that point
                {
                    head.next = a;
                }
                a = a.next;
            }
            else
            {
                c = c.next = b;
                if (count == 0) // b was the first, assign head.next to be that point
                {
                    head.next = b;
                }
                b = b.next;
            }
            count++; //used to prevent infinite loop
        } while (c != empty || count > 1000);
        return head.next;
    }

    #endregion

    #region === hull generation ===    
    bool hasEmpty(Point p, Point q, Point r)
    {
        return (p == empty || q == empty || r == empty);
    }
    private float Turn(Point p, Point q, Point r)
    {
        //if (p == empty || p == null ||
        //    q == empty || q == null ||
        //    r == empty || r == null)
        //{
        //    return 1.0f;
        //}
        //c++ return (q->x-p->x)*(r->y-p->y) - (r->x-p->x)*(q->y-p->y);
        var t = (q.x - p.x) * (r.y - p.y) - (r.x - p.x) * (q.y - p.y);
        return t;
    }

    private float Time(Point p, Point q, Point r)
    {
        if (p == empty || p == null ||
            q == empty || q == null ||
            r == empty || r == null)
        {
            return INF;
        }
        // c++ ((q->x - p->x) * (r->z - p->z) - (r->x - p->x) * (q->z - p->z)) / turn(p, q, r);
        var t = ((q.x - p.x) * (r.z - p.z) - (r.x - p.x) * (q.z - p.z)) / Turn(p, q, r);
        return t;
    }
    private void Hull(bool isLowerHull, Point pointList, int n, Point[] A, Point[] B, int indexOffset, int level)
    {
        Point u, v, mid;
        //float[] t = new float[6];
        float oldt, newt;

        int i, j, k, l, minl = 0;
        // Base case, when only one point,
        if (n == 1)
        {
            //remove point from the list
            pointList.next = empty;
            pointList.prev = empty;
            A[indexOffset] = empty;
            return;
        }
        for (u = pointList, i = 0; i < n / 2 - 1; u = u.next, ++i) ; // I really dont like this format
        mid = v = u.next;

        // recurse on left and right sides
        Hull(isLowerHull, pointList, n / 2, B, A, indexOffset, level + 1); //recurse left side 
        Hull(isLowerHull, mid, n - n / 2, B, A, indexOffset + n / 2 * 2, level + 1); //recurse right side  
        //Hull(lower, mid, n - n / 2, B, A, 0); //recurse right side

        //find initial bridge
        // a bridge is the two points on the left and right where the two hulls 
        while (true)
        {
            if (isLowerHull)
            {
                if (!hasEmpty(u, v, v.next) && Turn(u, v, v.next) < 1.0f) v = v.next;
                else if (!hasEmpty(u.prev, u, v) && Turn(u.prev, u, v) < 1.0f) u = u.prev;
                else break;
            }
            else
            {
                if (!hasEmpty(u, v, v.next) && Turn(u, v, v.next) > 1.0f) v = v.next;
                else if (!hasEmpty(u.prev, u, v) && Turn(u.prev, u, v) > 0) u = u.prev;
                else break;
            }
        }

        //merge by tracking bridge u v over time
        //if (indexOffset > 0)
        //{
        //    i = indexOffset-1;
        //    k = indexOffset-1;
        //    j = indexOffset - 1 + (n / 2 * 2);
        //}
        //else 
        //{
        //    i = 0;
        //    k = 0;
        //    j = (n / 2 * 2) - 1;
        //}

        i = 0;
        j = n / 2 * 2;
        k = 0;
        oldt = -INF;
        //for (i = k = indexOffset, j = indexOffset + (n / 2 * 2) - 1, oldt = -INF; ; oldt = newt)
        while (true)
        {
            bool _event = false;
            Point[] p = new Point[6], q = new Point[6], r = new Point[6];

            // find the next possible events COPING FORMAT FROM OTHER CODE i found
            p[0] = B[indexOffset + i].prev; q[0] = B[indexOffset + i]; r[0] = B[indexOffset + i].next; // left hull
            p[1] = B[indexOffset + j].prev; q[1] = B[indexOffset + j]; r[1] = B[indexOffset + j].next; // right hull
            p[2] = u; q[2] = u.next; r[2] = v;                  // bridge..
            p[3] = u.prev; q[3] = u; r[3] = v;
            p[4] = u; q[4] = v.prev; r[4] = v;
            p[5] = u; q[5] = v; r[5] = v.next;

            // original code
            //t[0] = Time(B[i].prev, B[i], B[i].next);
            //t[1] = Time(B[j].prev, B[j], B[j].next);
            //t[2] = Time(u, u.next, v);
            //t[3] = Time(u.prev, u, v);
            //t[4] = Time(u, v.prev, v);
            //t[5] = Time(u, v, v.next);


            for (newt = INF, l = 0; l < 6; ++l)
            {
                float t = Time(p[l], q[l], r[l]);
                if (t > oldt && t < newt)
                {
                    minl = l;
                    newt = t;
                }
            }
            if (newt == INF)
            {
                // no events found, hull is merged??
                break;
            }

            // this is taken from another source
            switch (minl)
            {
                case 0: //left side
                    _event = B[indexOffset + i].x < u.x;
                    break;
                case 1: //right side
                    _event = B[indexOffset + j].x > v.x;
                    break;
                default:
                    break;
            }

            switch (minl)
            {
                case 0:
                    if (_event) A[indexOffset + k++] = B[indexOffset + i];
                    B[indexOffset + i++].AddOrRemove();
                    break;
                case 1:
                    if (_event) A[indexOffset + k++] = B[indexOffset + j];
                    B[indexOffset + j++].AddOrRemove();
                    break;
                case 2:
                    u = u.next;
                    A[indexOffset + k++] = u;
                    break;
                case 3:
                    A[indexOffset + k++] = u;
                    u = u.prev;
                    break;
                case 4:
                    v = v.prev;
                    A[indexOffset + k++] = v;
                    break;
                case 5:
                    A[indexOffset + k++] = v;
                    v = v.next;
                    break;
                default:
                    break;
            }

            oldt = newt;
        }
        A[k] = empty; // "mark the end of the merged hull"

        //go back in time and update pointers
        // make the bridge and point next to eachother
        u.next = v;
        v.prev = u;

        for (--k; k >= 0; --k)
        {
            if (A[indexOffset + k].x <= u.x || A[indexOffset + k].x >= v.x)
            {
                A[indexOffset + k].AddOrRemove();
                if (A[indexOffset + k] == u) u = u.prev;
                else if (A[indexOffset + k] == v) v = v.next;
            }
            else
            {
                //inside the bridge, so a bridge endpoint
                u.next = A[indexOffset + k];
                A[indexOffset + k].prev = u;
                v.prev = A[indexOffset + k];
                A[indexOffset + k].next = v;

                // make it a bridge endpoint, on the left or right side
                if (A[indexOffset + k].x < mid.x) u = A[indexOffset + k];
                else v = A[indexOffset + k];
            }
        }
    }
    #endregion
}
