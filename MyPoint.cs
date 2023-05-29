using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGeometry
{

    public class ByCoordinates : Comparer<MyPoint>
    {
        private int x_1, x_2, y_1, y_2;
        public override int Compare(MyPoint a, MyPoint b)
        {
            x_1 = a.Vertex.X;
            x_2 = b.Vertex.X;
            y_1 = a.Vertex.Y;
            y_2 = b.Vertex.Y;
            if (x_1 < x_2)
            {
                return -1;
            }
            else if (x_1 == x_2)
            {
                if (y_1 < y_2)
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }
            else { return 1; }
        }
    }

    public class MyPoint : IEquatable<MyPoint>
    {
        private Point _vertex;
        private double angle;
        private double radius;
        private int poligon;
        List<Segment> edges;

        


  public MyPoint()
        {
            this._vertex = new Point(0, 0);
            this.angle = 0;
            this.radius = 0;
            this.poligon = 0;
            this.edges = new List<Segment>();
        }

        public MyPoint(Point p)
        {
            this._vertex = new Point(p.X, p.Y);
            this.angle = 0;
            this.radius = 0;
            this.poligon = 0;
            this.edges = new List<Segment>();
        }

        public MyPoint(Point p, double r, int pol)
        {
            this._vertex = new Point(p.X, p.Y);
            this.angle = 0;
            this.radius = r;
            this.poligon = pol;
            this.edges = new List<Segment>();
        }

        public MyPoint(Point p, int pol)
        {
            this._vertex = new Point(p.X, p.Y);
            this.angle = 0;
            this.radius = 0;
            this.poligon = pol;
            this.edges = new List<Segment>();
        }

        public MyPoint(Point p, int pol, List<Segment> e)
        {
            this._vertex = new Point(p.X, p.Y);
            this.angle = 0;
            this.radius = 0;
            this.poligon = pol;
            this.edges = new List<Segment>(e);
        }

        public MyPoint(Point p, double an, double rad, int pol, List<Segment> e)
        {
            this._vertex = new Point(p.X, p.Y);
            this.angle = an;
            this.radius = rad;
            this.poligon = pol;
            this.edges = new List<Segment>(e);
        }

        public int Poligon
        {
            get { return poligon; }
            set { poligon = value; }
        }

        public double Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public double Radius
        {
            get { return radius; }
            set { radius = value; }
        }

        public List<Segment> Edges
        {
            get { return edges; }
            set { edges = new List<Segment>(value); }
        }

        public Point Vertex
        {
            get { return _vertex; }
            set
            {
                _vertex.X = value.X;
                _vertex.Y = value.Y;
            }
        }


        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            MyPoint objAsPart = obj as MyPoint;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public override int GetHashCode()
        {
            return Vertex.GetHashCode();
        }
        public bool Equals(MyPoint other)
        {
            if (other == null) return false;
            return (this.Vertex.Equals(other.Vertex));
        }


    }
}
