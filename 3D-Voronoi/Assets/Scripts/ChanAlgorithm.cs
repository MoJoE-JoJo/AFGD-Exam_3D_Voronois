using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChanAlgorithm : MonoBehaviour
{
    [SerializeField]
    private List<Transform> initialPoints;


    public class Point
    {
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

        void act()
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
    // Start is called before the first frame update
    void Start()
    {

        empty = new Point
        {
            pos = new Vector3(INF, INF, INF),
            prev = null,
            next = null
        };

        //testing 
        var lst = new List<Point>();

        foreach (var p in initialPoints)
        {
            var point = new Point
            {
                pos = p.position,
                prev = empty,
                next = empty
            };
            lst.Add(point);
        }
        // this merge sort should connect each point togehter like a linked list in a sorted manor. 
        var tmp = Sort(lst.ToArray(), lst.Count);

        Debug.Log(tmp);
    }

    private float Turn(Point p, Point q, Point r)
    {
        if (p == empty || q == empty || r == empty)
        {
            return 1.0f;
        }
        //c++ return (q->x-p->x)*(r->y-p->y) - (r->x-p->x)*(q->y-p->y);
        return (q.x - p.x) * (r.y - p.y) - (r.x - p.x) * (q.y - p.y);
    }

    private float Time(Point p, Point q, Point r)
    {
        if (p == empty || q == empty || r == empty)
        {
            return INF;
        }
        // c++ ((q->x - p->x) * (r->z - p->z) - (r->x - p->x) * (q->z - p->z)) / turn(p, q, r);
        return ((q.x - p.x) * (r.z - p.z) - (r.x - p.x) * (q.z - p.z)) / Turn(p, q, r);
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


    private T[] SplitLeft<T>(T[] array)
    {
        var len = array.Length;
        T[] rtnArray;
        if (len % 2 == 0)
        {
            len = len / 2;
            rtnArray = new T[len];
        }
        else
        {
            len = (len / 2);
            rtnArray = new T[len];
        }

        for (int i = 0; i < len; i++)
        {
            rtnArray[i] = array[i];
        }
        return rtnArray;
    }

    private T[] SplitRight<T>(T[] array)
    {
        var len = array.Length;
        var startIndex = array.Length / 2;

        if (len % 2 == 0) //even
        {
            len = len / 2;
        }
        else //uneven, plus 1 to the length
        {
            len = (len / 2);
        }
        T[] rtnArray = new T[len];

        for (int i = startIndex; i < len + startIndex; i++)
        {
            rtnArray[i] = array[i];
        }
        return rtnArray;
    }

    // "pointer" to a head point
    private Point head = new Point();
    private Point Sort(Point[] points, int n) //mergesort
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
        int limit = 0; //prevent inf loop
        do
        {
            if (a.x < b.x)
            {
                c = c.next = a;

                // a was the first, assign head to be that point
                if (limit == 0)
                {
                    head.next = a;
                }

                a = a.next;
            }
            else
            {
                c = c.next = b;
                if (limit == 0)
                {
                    head.next = b;
                }

                b = b.next;
            }
            limit++;
        } while (c != empty || limit > 200);
        return head.next;
    }


    // Update is called once per frame
    void Update()
    {

    }
}
