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

        public void Act()
        {
            if (prev.next != this)
            {
                prev.next = this; // insert point
            }
            else
            {
                //remove this point from the "linked list"
                prev.next = next;
                next.prev = prev;
            }
        }
    }
    private static float INF = float.MaxValue;
    private Point empty;

    private Point[] res;
    // Start is called before the first frame update
    void Start()
    {

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
        var sortedLinkedList = Sort(lst.ToArray(), lst.Count);

        Point[] A = new Point[2 * n];
        Point[] B = new Point[2 * n];

        //for (int i = 0; i < A.Length; i++)
        //{
            //A[i] = new Point();
            //B[i] = new Point();
        //}
        Hull(true, sortedLinkedList, n, A, B, 0);

        Hull(false, sortedLinkedList, n, A, B, 0);

        res = A;

        res = res.Where(x => x != null).ToArray();
        Dictionary<Point, bool> dict = new Dictionary<Point, bool>();
        for (int i = 0; i < res.Length ; i++)
        {
            Point current = res[i];
            while (current != empty && current != null)
            {
                if (!dict.ContainsKey(current)) dict.Add(current, true);
                current = current.next;
            }
        }

        Debug.Log("Done?");
    }


    bool hasEmpty(Point p, Point q, Point r)
    {
        return (p == empty || q == empty || r == empty);
    }

    private float Turn(Point p, Point q, Point r)
    {
        if (p == empty || p == null ||
            q == empty || q == null ||
            r == empty || r == null)
        {
            return 1.0f;
        }
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

    private void Hull(bool lower, Point pointList, int n, Point[] A, Point[] B, int indexOffset)
    {
        Point u, v, mid;
        float[] t = new float[6];
        float oldt, newt;

        int i, j, k, l, minl = 0;
        if (n == 1)
        {
            A[indexOffset] = pointList.prev = pointList.next = empty;
            return;
        }
        for (u = pointList, i = 0; i < n / 2 - 1; u = u.next, i++) ; // I reallt dont like this format

        mid = v = u.next;

        // recurse on left and right sides
        Hull(lower, pointList, n / 2, B, A, indexOffset); //recurse left side
        Hull(lower, mid, n - n / 2, B, A, indexOffset + n / 2 * 2); //recurse right side

        //find initial bridge
        while(true)
        {
            if (lower)
            {
                if (Turn(u, v, v.next) < 1) v = v.next;
                else if (Turn(u.prev, u, v) < 1) u = u.prev;
                else break;
            }
            else 
            {
                if (Turn(u, v, v.next) > 1) v = v.next;
                else if (Turn(u.prev, u, v) > 1) u = u.prev;
                else break;
            }
        }

        //merge by tracking bridge u v over time
        //i = 0;
        //k = 0;
        //j = n / 2 * 2;
        //oldt = -INF;
        for (i = k = indexOffset, j = indexOffset + (n/2*2), oldt  = -INF; ; oldt = newt)
        {
            t[0] = Time(B[i].prev, B[i], B[i].next);
            t[1] = Time(B[j].prev, B[j], B[j].next);

            t[2] = Time(u, u.next, v);
            t[3] = Time(u.prev, u, v);
            t[4] = Time(u, v.prev, v);
            t[5] = Time(u, v, v.next);

            for (newt = INF, l = 0; l < 6; l++)
            {
                if (t[l] > oldt && t[l] < newt)
                {
                    minl = l;
                    newt = t[l];
                }
            }
            if (newt == INF)
            {
                break;
            }

            switch (minl)
            {
                case 0:
                    if (B[i].x < u.x)
                    {
                        A[k++] = B[i];
                        B[i++].Act();
                    }
                    break;
                case 1:
                    if (B[j].x > v.x)
                    {
                        A[k++] = B[j];
                        B[j++].Act();
                    }
                    break;
                case 2:
                    A[k++] = u = u.next;
                    break;
                case 3:
                    A[k++] = u;
                    u = u.prev;
                    break;
                case 4:
                    A[k++] = v = v.prev;
                    break;
                case 5:
                    A[k++] = v;
                    v = v.next;
                    break;
                default:
                    break;
            }
            //oldt = newt;
        }
        A[k] = empty; // "mark the end of the merged hull"

        //go back in time and update pointers
        u.next = v;
        v.prev = u;

        for (k--; k >= indexOffset; k--)
        {
            if (A[k].x <= u.x || A[k].x >= v.x)
            {
                A[k].Act();
                if (A[k] == u)
                {
                    u = u.prev;
                }
                else if (A[k] == v)
                {
                    v = v.next;
                }
            }
            else
            {
                //inside the bridge
                u.next = A[k];
                A[k].prev = u;
                v.prev = A[k];
                A[k].next = v;
                if (A[k].x < mid.x)
                {
                    u = A[k];
                }
                else
                {
                    v = A[k];
                }
            }
        }
    }
 
    // Update is called once per frame
    void Update()
    {
    }
}
