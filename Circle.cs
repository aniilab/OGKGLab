using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGeometry
{
    public class Circle
    {
        private double radius;
        private Point center;


        public Circle (double r, Point c)
        {
            this.radius = r;
            this.center = new Point(c.X, c.Y);
        }
        public double Radius
        {
            get {return radius;}
            set { radius = value; }
        }
        
        public Point Center
        {
            get { return center; }
            set
            {
                center.X = value.X;
                center.Y = value.Y;
            }
        }
  
    }
}
