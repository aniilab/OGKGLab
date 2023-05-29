using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGeometry
{
    public class ByDistance : Comparer<Pair>
    {
        private double d1, d2;
        public override int Compare(Pair a, Pair b)
        {
            d1 = a.Distance;
            d2 = b.Distance;
            if (d1 < d2)
            {
                return -1;
            }
            else if (d1 > d2)
            {
                return 1;
            }
            else { return 0; }
        }
    }

    public class Pair : IEquatable<Pair>
    {
        private int vertex;
        private double distance;

        public Pair()
        {
            this.vertex = 0;
            this.distance = 0;
        }

        public Pair(double d, int v)
        {
            this.vertex = v;
            this.distance = d;
        }

        public int Vertex
        {
            get { return vertex; }
            set { vertex = value; }
        }

        public double Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Pair objAsPart = obj as Pair;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public override int GetHashCode()
        {
            return Distance.GetHashCode();
        }
        public bool Equals(Pair other)
        {
            if (other == null) return false;
            return (this.Distance.Equals(other.Distance));
        }




    }
}
