using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGeometry
{
    public class ByDist : Comparer<Segment>
    {
        private double Ra1, Ra2, Rb1, Rb2;
        public override int Compare(Segment a, Segment b)
        {
            Ra1 = a.First.Radius;
            Ra2 = a.Last.Radius;
            Rb1 = b.First.Radius;
            Rb2 = b.Last.Radius;

            if (Ra1 < Rb1) { return -1; }
            else if (Ra1 > Rb1) { return 1; }
            else
            {
                if (Ra2 < Rb2) { return -1; }
                else { return 1; }
            }
        }
    }

    public class Segment //: IEquatable<Segment>
    {
        private MyPoint first;
        private MyPoint last;

        public Segment()
        {
            this.first = new MyPoint(new Point(0, 0));
            this.last = new MyPoint(new Point(0, 0));
        }
        public Segment(MyPoint frst, MyPoint lst)
        {
            this.first = new MyPoint(new Point(frst.Vertex.X, frst.Vertex.Y));
            this.last  = new MyPoint(new Point(lst.Vertex.X, lst.Vertex.Y));
        }

        public MyPoint First
        {
            get { return first; }
            set
            {
                first = value;
            }
        }

        public MyPoint Last
        {
            get { return last; }
            set { last = value; }
        }

        public double A
        {
            get
            {
                return first.Vertex.Y - last.Vertex.Y;
            }
        }

        public double B
        {
            get
            {
                return last.Vertex.X - first.Vertex.X;
            }
        }

        public double C
        {
            get
            {
                return (first.Vertex.X * last.Vertex.Y - last.Vertex.X * first.Vertex.Y);
            }
        }


        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Segment objAsPart = obj as Segment;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Last.GetHashCode();
        }
        public bool Equals(Segment other)
        {
            if (other == null) return false;
            return (this.First.Equals(other.First) && this.Last.Equals(other.Last));
        }

    }
}
