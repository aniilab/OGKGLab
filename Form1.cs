using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TestGeometry
{

    public partial class Form1 : Form
    {
        Random rnd;
        HashSet<Segment> graph;
        HashSet<Segment> segments;
        List<MyPoint> Vertices;
        List<Circle> circles;
        List<Point> points;
        MyPoint start, end;
        bool fieldBusy = false;

        Graphics g;


        int width;
        int height;
        int h;
        int n;
        public Form1()
        {
            WindowState = FormWindowState.Maximized;
            InitializeComponent();
            width = pictureBox1.Width;
            height = pictureBox1.Height;
            segments = new HashSet<Segment>();
            rnd = new Random();
            circles = new List<Circle>();
            Vertices = new List<MyPoint>();
            points = new List<Point>();
            graph = new HashSet<Segment>();
        }

        public double Dist (Point a_, Point b_)
        {
            return Math.Sqrt(Math.Pow(a_.X - b_.X, 2) + Math.Pow(a_.Y - b_.Y, 2));
        }

        public int MinRad (Point b, int w, int h)
        {
            return Math.Min(Math.Min(b.X, w - b.X), Math.Min(b.Y, h - b.Y));
        }
        
        public int DecimalX (double r, double angle)
        {
            return Convert.ToInt32(r * Math.Cos(Math.PI * angle / 180.0));
        }

        public int DecimalY(double r, double angle)
        {
            return Convert.ToInt32(r * Math.Sin(Math.PI * angle / 180.0));
        }

        public List<Point> GenerateObstacleCenters(int h)
        {
            List<Point> pn = new List<Point>();
            double fromW = 0.03 * width + 1;
            double toW = 0.97 * width - 1;
            double fromH = 0.03 * height + 1;
            double toH = 0.97 * height - 1;

            for (int i = 0; i < h; i++)
            {
                Point a;
                do
                {
                    a = new Point(rnd.Next((int)fromW, (int)toW), rnd.Next((int)fromH, (int)toH));
                }
                while (pn.Contains(a));
                pn.Add(a);
            }
            return pn;
        }

        public void GenerateCircles()
        {
            double min;

            for (int i = 0; i < h; i++)
            {
                min = MinRad(points[i], width, height);
                for (int j = 0; j < h; j++)
                {
                    if (i != j)
                    {
                        if (Dist(points[i], points[j]) < min)
                        {
                            min = Dist(points[i], points[j]);
                        }
                    }
                }
                min = min / 2 - 1;

                circles.Add(new Circle((int)min, points[i]));
            }
        }


        public void CreateObstacles(int h, int n)
        {
            points = GenerateObstacleCenters(h);

            GenerateCircles();

            GenerateVertices();
            fieldBusy = true;

        }

        public void GenerateVertices()
        {
            int verticesLeft = n;
            int poligonsLeft = h - 1;

            for (int i = 0; i < h; i++)
            {
                int number = 0;

                if (i == h - 1)
                {
                    number = verticesLeft;
                }
                else
                {
                    number = rnd.Next(3, verticesLeft - (poligonsLeft * 3));
                }

                verticesLeft -= number;
                poligonsLeft -= 1;

                List<double> angle = new List<double>();
                double temp;
                int[] rad = new int[number];

                for (int j = 0; j < number; j++)
                {
                    do
                    {
                        int o = 90;
                        if (89 + (int)(j * 360.0 / number) >= 360)
                        {
                            o = 360 - (int)(j * 360.0 / number);
                        }
                        temp = rnd.NextDouble() * o + j * 360.0 / number;
                    }
                    while (angle.Contains(temp));
                    angle.Add(temp);

                    rad[j] = (int)circles[i].Radius;
                }

                angle.Sort();

                MyPoint a, b, c, d;
                List<Segment> edgesForVertex = new List<Segment>();

                for (int j = 1; j < number - 1; j++)
                {
                    a = new MyPoint(new Point((DecimalX(rad[j - 1], angle[j - 1]) + circles[i].Center.X), (DecimalY(rad[j - 1], angle[j - 1]) + circles[i].Center.Y)));
                    b = new MyPoint(new Point((DecimalX(rad[j], angle[j]) + circles[i].Center.X), (DecimalY(rad[j], angle[j]) + circles[i].Center.Y)));
                    c = new MyPoint(new Point((DecimalX(rad[j + 1], angle[j + 1]) + circles[i].Center.X), (DecimalY(rad[j + 1], angle[j + 1]) + circles[i].Center.Y)));

                    edgesForVertex.Add(new Segment() { First = new MyPoint() { Vertex = new Point() { X = a.Vertex.X, Y = a.Vertex.Y }, Poligon = i + 1 }, Last = new MyPoint() { Vertex = new Point() { X = b.Vertex.X, Y = b.Vertex.Y }, Poligon = i + 1 } });
                    edgesForVertex.Add(new Segment() { First = new MyPoint() { Vertex = new Point() { X = b.Vertex.X, Y = b.Vertex.Y }, Poligon = i + 1 }, Last = new MyPoint() { Vertex = new Point() { X = c.Vertex.X, Y = c.Vertex.Y }, Poligon = i + 1 } });

                    Vertices.Add(new MyPoint() { Vertex = new Point() { X = b.Vertex.X, Y = b.Vertex.Y }, Radius = Int32.MaxValue, Poligon = i + 1, Edges = edgesForVertex });
                    edgesForVertex.Clear();
                    segments.Add(new Segment() { First = new MyPoint() { Vertex = new Point() { X = a.Vertex.X, Y = a.Vertex.Y }, Poligon = i + 1 }, Last = new MyPoint() { Vertex = new Point() { X = b.Vertex.X, Y = b.Vertex.Y }, Poligon = i + 1 } });
                }

                a = new MyPoint(new Point((DecimalX(rad[number - 2], angle[number - 2]) + circles[i].Center.X), (DecimalY(rad[number - 2], angle[number - 2]) + circles[i].Center.Y)));
                b = new MyPoint(new Point((DecimalX(rad[number - 1], angle[number - 1]) + circles[i].Center.X), (DecimalY(rad[number - 1], angle[number - 1]) + circles[i].Center.Y)));
                c = new MyPoint(new Point((DecimalX(rad[0], angle[0]) + circles[i].Center.X), (DecimalY(rad[0], angle[0]) + circles[i].Center.Y)));
                d = new MyPoint(new Point((DecimalX(rad[1], angle[1]) + circles[i].Center.X), (DecimalY(rad[1], angle[1]) + circles[i].Center.Y)));

                edgesForVertex.Add(new Segment() { First = new MyPoint() { Vertex = new Point() { X = a.Vertex.X, Y = a.Vertex.Y }, Poligon = i + 1 }, Last = new MyPoint() { Vertex = new Point() { X = b.Vertex.X, Y = b.Vertex.Y }, Poligon = i + 1 } });
                edgesForVertex.Add(new Segment() { First = new MyPoint() { Vertex = new Point() { X = b.Vertex.X, Y = b.Vertex.Y }, Poligon = i + 1 }, Last = new MyPoint() { Vertex = new Point() { X = c.Vertex.X, Y = c.Vertex.Y }, Poligon = i + 1 } });

                Vertices.Add(new MyPoint() { Vertex = new Point() { X = b.Vertex.X, Y = b.Vertex.Y }, Radius = Int32.MaxValue, Poligon = i + 1, Edges = edgesForVertex });
                edgesForVertex.Clear();

                edgesForVertex.Add(new Segment() { First = new MyPoint() { Vertex = new Point() { X = b.Vertex.X, Y = b.Vertex.Y }, Poligon = i + 1 }, Last = new MyPoint() { Vertex = new Point() { X = c.Vertex.X, Y = c.Vertex.Y }, Poligon = i + 1 } });
                edgesForVertex.Add(new Segment() { First = new MyPoint() { Vertex = new Point() { X = c.Vertex.X, Y = c.Vertex.Y }, Poligon = i + 1 }, Last = new MyPoint() { Vertex = new Point() { X = d.Vertex.X, Y = d.Vertex.Y }, Poligon = i + 1 } });

                Vertices.Add(new MyPoint() { Vertex = new Point() { X = c.Vertex.X, Y = c.Vertex.Y }, Radius = Int32.MaxValue, Poligon = i + 1, Edges = edgesForVertex });

                edgesForVertex.Clear();

                segments.Add(new Segment() { First = new MyPoint() { Vertex = new Point() { X = a.Vertex.X, Y = a.Vertex.Y }, Poligon = i + 1 }, Last = new MyPoint() { Vertex = new Point() { X = b.Vertex.X, Y = b.Vertex.Y }, Poligon = i + 1 } });
                segments.Add(new Segment() { First = new MyPoint() { Vertex = new Point() { X = b.Vertex.X, Y = b.Vertex.Y }, Poligon = i + 1 }, Last = new MyPoint() { Vertex = new Point() { X = c.Vertex.X, Y = c.Vertex.Y }, Poligon = i + 1 } });
            }
        }

        public double CountAngle(Point start, Point current)
        {
            double angle;
            angle = Math.Asin((current.Y - start.Y) / Math.Sqrt((current.X - start.X) * (current.X - start.X) + (current.Y - start.Y) * (current.Y - start.Y))) * 180 / Math.PI;
            return angle;
        }

        public double CountRadius(Point start, Point current)
        {
            double radius;
            radius = Math.Sqrt((current.X - start.X) * (current.X - start.X) + (current.Y - start.Y) * (current.Y - start.Y));
            return radius;
        }

        public List<MyPoint> SortVerticesByAngle(Point v, string direction)
        {
            List<MyPoint> sortedPoints = new List<MyPoint>();

            foreach( MyPoint p in Vertices)
            {
                if( direction == "right")
                {
                    if (v.X <= p.Vertex.X && v != p.Vertex)
                    {
                        sortedPoints.Add(new MyPoint(new Point(p.Vertex.X, p.Vertex.Y), CountAngle(v, p.Vertex), CountRadius(v, p.Vertex), p.Poligon, p.Edges));
                    }
                }
                else if( direction == "left")
                {
                    if (v.X >= p.Vertex.X && v != p.Vertex)
                    {
                        sortedPoints.Add(new MyPoint(new Point(p.Vertex.X, p.Vertex.Y), CountAngle(v, p.Vertex), CountRadius(v, p.Vertex), p.Poligon, p.Edges));
                    }
                }
            }

            sortedPoints = sortedPoints.OrderByDescending(point => point.Angle).ThenBy(pnt => pnt.Radius).ToList();
            return sortedPoints;
        }

        public bool MyIntersect (Segment m, Segment n)
    {
        bool flag = false;

        if ((m.A * n.First.Vertex.X + m.B * n.First.Vertex.Y + m.C) * (m.A * n.Last.Vertex.X + m.B * n.Last.Vertex.Y + m.C) < 0 && (n.A * m.First.Vertex.X + n.B * m.First.Vertex.Y + n.C) * (n.A * m.Last.Vertex.X + n.B * m.Last.Vertex.Y + n.C) < 0)
        {
            flag = true;
        }
        else if (m.A * n.First.Vertex.X + m.B * n.First.Vertex.Y + m.C == 0 && n.First.Vertex != m.Last.Vertex && n.First.Vertex != m.First.Vertex)
        {
            flag = true;
        }
        else if (m.A * n.Last.Vertex.X + m.B * n.Last.Vertex.Y + m.C == 0 && n.Last.Vertex != m.Last.Vertex && n.Last.Vertex !=m.First.Vertex)
        {
            flag = true;
        }

        return flag;
    }

        public bool IsVisible (MyPoint a, MyPoint b, HashSet<Segment> intersections)
        {
            Segment current = new Segment(a, b);

            bool flag = true;
            foreach (Segment s in intersections)
            {
                if (MyIntersect(current, s))
                {
                    flag = false;
                    break;
                }
            }
            return flag;
        }

        public HashSet<Segment> buildVisibilityGraph()
        {
            HashSet<Segment> gr = new HashSet<Segment>();
            HashSet<MyPoint> visibleVertices = new HashSet<MyPoint>();

            foreach (Segment s in segments)
            {
                gr.Add(new Segment() { First = new MyPoint() { Vertex = new Point() { X = s.First.Vertex.X, Y = s.First.Vertex.Y } }, Last = new MyPoint() { Vertex = new Point() { X = s.Last.Vertex.X, Y = s.Last.Vertex.Y } } });
            }
            foreach (MyPoint v in Vertices)
            {
                visibleVertices = getVisibleVertices(v, "right");

                foreach (MyPoint w in visibleVertices)
                {
                    gr.Add(new Segment() { First = new MyPoint() { Vertex = new Point() { X = v.Vertex.X, Y = v.Vertex.Y } }, Last = new MyPoint() { Vertex = new Point() { X = w.Vertex.X, Y = w.Vertex.Y } } });
                }
                visibleVertices.Clear();
            }
            return gr;
        }

        public HashSet<MyPoint> getVisibleVertices(MyPoint v, string direct)
        {
            List<MyPoint> sortedVertices = SortVerticesByAngle(v.Vertex, direct);

            HashSet<Segment> intersectedSegments = new HashSet<Segment>();
            Segment current = new Segment(v, new MyPoint(v.Vertex, height));

            foreach (Segment s in segments)
            {
                if (MyIntersect(current, s))
                {
                    intersectedSegments.Add(s);
                }
            }

            HashSet<MyPoint> visibleVertices = new HashSet<MyPoint>();

            foreach (MyPoint w in sortedVertices)
            {
                if (v.Poligon != w.Poligon && IsVisible(v, w, intersectedSegments))
                {
                    visibleVertices.Add(w);
                }
                current = new Segment(v, w);
                for (int i = 0; i < w.Edges.Count; i++)
                {
                    Point c;
                    if (direct == "right")
                    {
                        if (w.Edges[i].First.Vertex == w.Vertex)
                        {
                            c = new Point(w.Edges[i].Last.Vertex.X, w.Edges[i].Last.Vertex.Y);
                        }
                        else { c = new Point(w.Edges[i].First.Vertex.X, w.Edges[i].First.Vertex.Y); }

                        if (current.A * c.X + current.B * c.Y + current.C < 0)
                        {
                            intersectedSegments.Add(w.Edges[i]);
                        }
                        else if (current.A * c.X + current.B * c.Y + current.C == 0)
                        {
                            if (Dist(current.First.Vertex, c) > Dist(current.First.Vertex, current.Last.Vertex))
                            {
                                intersectedSegments.Add(w.Edges[i]);
                            }
                        }
                        else
                        {
                            if (intersectedSegments.Contains(w.Edges[i]))
                            {
                                intersectedSegments.Remove(w.Edges[i]);
                            }
                        }
                    }

                    else if (direct == "left")
                    {
                        if (w.Edges[i].First.Vertex == w.Vertex)
                        {
                            c = new Point(w.Edges[i].Last.Vertex.X, w.Edges[i].Last.Vertex.Y);
                        }
                        else { c = new Point(w.Edges[i].First.Vertex.X, w.Edges[i].First.Vertex.Y); }

                        if (current.A * c.X + current.B * c.Y + current.C > 0)
                        {
                            intersectedSegments.Add(w.Edges[i]);
                        }
                        else if (current.A * c.X + current.B * c.Y + current.C == 0)
                        {
                            if (Dist(current.First.Vertex, c) > Dist(current.First.Vertex, current.Last.Vertex))
                            {
                                intersectedSegments.Add(w.Edges[i]);
                            }
                        }
                        else
                        {
                            if (intersectedSegments.Contains(w.Edges[i]))
                            {
                                intersectedSegments.Remove(w.Edges[i]);
                            }
                        }

                    }
                }
            }
            return visibleVertices;
        }
        public HashSet<Segment> InsertStart_EndPoint(MyPoint start, MyPoint end, HashSet<Segment> graph)
        {
            HashSet<MyPoint> visibleVertices = new HashSet<MyPoint>();
            HashSet<Segment> additional = new HashSet<Segment>();

                List<MyPoint> HotPoints = new List<MyPoint>();
                HotPoints.Add(start);
                HotPoints.Add(end);
                    for (int i = 0; i < 2; i++ )
                    {
                        visibleVertices = getVisibleVertices(Vertices[i], "right");
                        foreach (MyPoint w in visibleVertices)
                            additional.Add(new Segment() { First = new MyPoint() { Vertex = new Point() { X = Vertices[i].Vertex.X, Y = Vertices[i].Vertex.Y } }, Last = new MyPoint() { Vertex = new Point() { X = w.Vertex.X, Y = w.Vertex.Y } } });

                        visibleVertices.Clear();

                        visibleVertices = getVisibleVertices(Vertices[i], "left");
                        foreach (MyPoint w in visibleVertices)
                            additional.Add(new Segment() { First = new MyPoint() { Vertex = new Point() { X = Vertices[i].Vertex.X, Y = Vertices[i].Vertex.Y } }, Last = new MyPoint() { Vertex = new Point() { X = w.Vertex.X, Y = w.Vertex.Y } } });

                        visibleVertices.Clear();
                    }

            HashSet<Segment> all = new HashSet<Segment>(graph);
                             all.UnionWith(additional);

            return all;
        }

        public void Prepair(MyPoint start, MyPoint end)
        {
            pictureBox1.Image = new Bitmap(Properties.Resources.image);
            g = Graphics.FromImage(pictureBox1.Image);

            g.Clear(Color.White);

            List<MyPoint> Point = new List<MyPoint>();
            Vertices.Insert(0, (new MyPoint() { Vertex = new Point() { X = end.Vertex.X, Y = end.Vertex.Y }, Radius = Int32.MaxValue, Poligon = -2}));
            Vertices.Insert(0, (new MyPoint() { Vertex = new Point() { X = start.Vertex.X, Y = start.Vertex.Y }, Radius = 0, Poligon = -1}));

            for (int i = 0; i < Vertices.Count; i++)
            {
                Point.Add(new MyPoint() { Vertex = new Point() { X = Vertices.ElementAt(i).Vertex.X, Y = Vertices.ElementAt(i).Vertex.Y }, Radius = Int32.MaxValue});
            }
            
            int start_point = Point.IndexOf(new MyPoint() { Vertex = new Point() { X = start.Vertex.X, Y = start.Vertex.Y }});
            int end_point = Point.IndexOf(new MyPoint() { Vertex = new Point() { X = end.Vertex.X, Y = end.Vertex.Y }});


            HashSet<Segment> allsg = InsertStart_EndPoint(start, end, graph);

            List<Segment> MinWay = new List<Segment>(Dijkstra(allsg.ToList(), Point, start_point, end_point));

            allsg.Clear();

            Vertices.Remove(new MyPoint() { Vertex = new Point() { X = start.Vertex.X, Y = start.Vertex.Y }});
            Vertices.Remove(new MyPoint() { Vertex = new Point() { X = end.Vertex.X, Y = end.Vertex.Y }});

            g.Clear(Color.White);
            Pen blue_pen = new Pen(Color.DarkCyan, 2);
            Pen black_pen = new Pen(Color.Black, 2);
            Pen gold_pen = new Pen(Color.Gold, 1);
            g.DrawRectangle(blue_pen, start.Vertex.X, start.Vertex.Y, 1, 1);
            g.DrawRectangle(blue_pen, end.Vertex.X, end.Vertex.Y, 1, 1);

            foreach (Segment sg in allsg)
            {
                g.DrawLine(gold_pen, sg.First.Vertex, sg.Last.Vertex);
            }
            foreach (Segment sg in segments)
            {
                g.DrawLine(blue_pen, sg.First.Vertex, sg.Last.Vertex);
            }

            foreach (Segment sg in MinWay)
            {
                g.DrawLine(black_pen, sg.First.Vertex, sg.Last.Vertex);
            }
        }

        public List<Segment> Dijkstra(List<Segment> all, List<MyPoint> point, int start, int end)
        {
            List<MyPoint> Visited_Points = new List<MyPoint>();
            List<Segment> way = new List<Segment>();

            int n = point.Count;

            double[,] matrix = new double[n, n];
            foreach (Segment s in all)
            {
                matrix[point.IndexOf(new MyPoint() { Vertex = new Point() { X = s.First.Vertex.X, Y = s.First.Vertex.Y } }), point.IndexOf(new MyPoint() { Vertex = new Point() { X = s.Last.Vertex.X, Y = s.Last.Vertex.Y } })] = Dist(s.First.Vertex, s.Last.Vertex);
                matrix[point.IndexOf(new MyPoint() { Vertex = new Point() { X = s.Last.Vertex.X, Y = s.Last.Vertex.Y } }), point.IndexOf(new MyPoint() { Vertex = new Point() { X = s.First.Vertex.X, Y = s.First.Vertex.Y } })] = Dist(s.First.Vertex, s.Last.Vertex);
            }
            
	       	SortedSet <Pair> q = new SortedSet<Pair>(new ByDistance());
            point.ElementAt(start).Radius = 0;
            point.ElementAt(start).Poligon = -1;
            q.Add(new Pair() { Distance = point.ElementAt(start).Radius, Vertex = start });
            
            while (q.Count != 0)
            {
               int v = q.ElementAt(0).Vertex;
                q.Remove(q.ElementAt(0));

                    for (int j = 0; j < n; ++j)
                    {
                            if (matrix[v, j] > 0)
                            {
                                int to = j;
                                double len = matrix[v, j];
                                if (point.ElementAt(v).Radius + len < point.ElementAt(to).Radius)
                                {
                                    q.Remove(new Pair() { Distance = point.ElementAt(to).Radius, Vertex = to });
                                    point.ElementAt(to).Radius = point.ElementAt(v).Radius + len;
                                    point.ElementAt(to).Poligon = v;
                                    q.Add(new Pair() { Distance = point.ElementAt(to).Radius, Vertex = to });
                                }
                            }
                    }
               
            }

            for (int v = end; v != start; v = point.ElementAt(v).Poligon)
            {
                Visited_Points.Add(new MyPoint(point.ElementAt(v).Vertex));
            }
                
                Visited_Points.Add(new MyPoint(point.ElementAt(start).Vertex));

            for (int i = 1; i < Visited_Points.Count; i++ )
            {
                way.Add(new Segment(Visited_Points[i-1], Visited_Points[i]));
            }
                return way;
        }

        public void Read()
        {
            
            if(textBox1.Text != "")
            {
                h = Convert.ToInt32(textBox1.Text);
            }
            else { h = 0; }
            if (textBox2.Text != "")
            {
                n = Convert.ToInt32(textBox2.Text);
            }
            else { n = 0; }
            


        }

        private void button1_Click(object sender, EventArgs e)
        {
            Read();
            if (n == 0 || h == 0)
            {
                MessageBox.Show("Спочатку введіть параметри h i n!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (h*3 > n)
            {
                MessageBox.Show("Кількість вершин має бути хоча б втричі більша за кількість фігур!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            textBox1.Visible = false;
            textBox2.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            button1.Visible = false;
            pictureBox1.Image = new Bitmap(Properties.Resources.image);
            g = Graphics.FromImage(pictureBox1.Image);
            g.Clear(Color.White);
            Stopwatch SW = new Stopwatch(); 

            
            SW.Start(); 
            CreateObstacles(h, n);
            graph = new HashSet<Segment>(buildVisibilityGraph());
            SW.Stop(); 
            label1.Text = "Генерація : " + Convert.ToString(SW.ElapsedMilliseconds/1000.0) + " секунд";

            foreach (Segment s in segments)
            {
                g.DrawLine(new Pen(Color.DarkCyan), new Point(s.First.Vertex.X, s.First.Vertex.Y), new Point(s.Last.Vertex.X, s.Last.Vertex.Y));
            }
            start = null;
            end = null;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            textBox1.Visible = true;
            textBox2.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            button1.Visible = true;
            textBox2.Clear();
            label1.Text = "";
            label4.Text = "";
            pictureBox1.Image = null;
            fieldBusy = false;
            n = 0;
            h = 0;
            segments = new HashSet<Segment>();
            rnd = new Random();
            circles = new List<Circle>();
            Vertices = new List<MyPoint>();
            points = new List<Point>();
            graph = new HashSet<Segment>();

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (n == 0 || h == 0 || !fieldBusy)
            {
                MessageBox.Show("Спочатку введіть параметри h i n та згенеруйте перешкоди!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(start ==null && end == null)
            {
                start = new MyPoint(new Point(e.Location.X, e.Location.Y), -1);
            }
            else if(start != null && end == null)
            {
                end = new MyPoint(new Point(e.Location.X, e.Location.Y), -2);
                Graphics g = pictureBox1.CreateGraphics();
                Pen black_pen = new Pen(Color.Black, 2);
                Stopwatch SW = new Stopwatch(); 

                g.DrawRectangle(black_pen, start.Vertex.X, start.Vertex.Y, 1, 1);
                g.DrawRectangle(black_pen, end.Vertex.X, end.Vertex.Y, 1, 1);
                SW.Start(); 
                Prepair(start, end);
                SW.Stop(); 
                label4.Text = "Алгоритм Дейкстри: " + Convert.ToString(SW.ElapsedMilliseconds/1000.0) + " секунд";
                start = null; end = null;
            }
            else if (start == null && end != null)
            {
                start = new MyPoint(new Point(e.Location.X, e.Location.Y), -1);
                Graphics g = pictureBox1.CreateGraphics();
                Pen black_pen = new Pen(Color.Black, 2);
                Stopwatch SW = new Stopwatch(); 

                g.DrawRectangle(black_pen, start.Vertex.X, start.Vertex.Y, 1, 1);
                g.DrawRectangle(black_pen, end.Vertex.X, end.Vertex.Y, 1, 1);
                SW.Start(); 
                Prepair(start, end);
                SW.Stop(); 
                label4.Text = "Алгоритм Дейкстри: " + Convert.ToString(SW.ElapsedMilliseconds / 1000.0) + " секунд";
                start = null; end = null;
            }
        }

    }
}
