using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IndividualTask1_ComputerGraphics_
{
    public partial class Form1 : Form
    {
        LinkedList<PointF> points;
        LinkedList<PointF> convexHull;
        Bitmap bmp;
        public Form1()
        {
            InitializeComponent();
            points = new LinkedList<PointF>();
            convexHull = new LinkedList<PointF>();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (points.Count > 1)
            {
                Jarvis();
                DrawConvexHull();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            points.Clear();
            convexHull.Clear();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            SolidBrush solidBrush = new SolidBrush(Color.Red);
            Graphics g = Graphics.FromImage(bmp);
            g.FillEllipse(solidBrush, e.X, e.Y, 3, 3);
            g.DrawEllipse(new Pen(Color.Red), e.X, e.Y, 3, 3);
            pictureBox1.Image = bmp;
            points.AddLast(e.Location);
        }

        private void DrawConvexHull()
        {
            Graphics g = Graphics.FromImage(bmp);
            Pen p = new Pen(Color.Black);
            var cur = convexHull.First;
            if (cur != convexHull.Last)
                g.DrawLine(p, convexHull.First.Value, convexHull.Last.Value);
            while (cur != convexHull.Last)
            {
                g.DrawLine(p, cur.Value, cur.Next.Value);
                cur = cur.Next;
            }
            pictureBox1.Image = bmp;
        }

        /// <summary>
        /// Точка p1 ниже p2? Если координаты y совпадают, то p1 левее p2?
        /// </summary>
        private bool pointBellowLeft(PointF p1, PointF p2)
        {
            if (p1.Y == p2.Y) // если находятся на одной высоте
            {
                return p1.X < p2.X; // p1 левее p2?
            }
            else return p1.Y > p2.Y; // первая точка ниже второй?
        }

        /// <summary>
        /// Значение косинуса между векторами
        /// </summary>
        private float cosValueBetweenVectors(PointF p1, PointF p2, PointF p3)
        {
            // координаты первого вектора
            var x1 = p1.X - p2.X;
            var y1 = p1.Y - p2.Y;
            // координаты второго вектора
            var x2 = p3.X - p2.X;
            var y2 = p3.Y - p2.Y;
            var scalarProd = x1 * x2 + y1 * y2; // скалярное произведение векторов
            var len1 = Math.Sqrt(x1 * x1 + y1 * y1);
            var len2 = Math.Sqrt(x2 * x2 + y2 * y2);
            return (float)(scalarProd / (len1 * len2)); // косинус угла между векторами
        }

        // тангенс полярного угла точки p2 относительно точки p1 как начала координат
        private float polarAngleTangent(PointF p1, PointF p2)
        {
            if (Math.Abs(p1.X - p2.X) < 0.001) // если угол равен 90 градусам
            {
                return float.MaxValue;
            }
            var x = p2.X - p1.X;
            var y = p1.Y - p2.Y;
            var val = y / x;
            if (val < 0)
                return (float)(Math.Atan(y / x)+360*Math.PI/180);
            else return (float)Math.Atan(y / x);
        }

        private void Jarvis()
        {
            LinkedListNode<PointF> first = points.First;
            LinkedListNode<PointF> cur = points.First;
            while (cur!=points.Last.Next) // Находим самую нижнюю точку. Если их несколько, то самую левую
            {
                if (pointBellowLeft(cur.Value, first.Value))
                    first = cur;
                cur = cur.Next;
            }
            convexHull.AddLast(first.Value);

            // Находим вторую точку. Она имеет наименьший полярный угол относительно первой точки как начала координат.
            LinkedListNode<PointF> second = first;
            var minAngle = float.MaxValue;
            cur = points.First;
            while (cur != points.Last.Next)
            {
                var localMinAngle = polarAngleTangent(first.Value, cur.Value);
                if (localMinAngle <= minAngle)
                {
                    second = cur;
                    minAngle = localMinAngle;
                }
                cur = cur.Next;
            }
            convexHull.AddLast(second.Value);
            LinkedListNode<PointF> prev = first;
            LinkedListNode<PointF>  current = new LinkedListNode<PointF>(second.Value);
            points.Remove(second);

            do
            {
                LinkedListNode<PointF> next = current;
                var min = float.MaxValue;
                cur = points.First;
                while (cur!=points.Last.Next)
                {
                    var localMin = cosValueBetweenVectors(prev.Value, current.Value, cur.Value);
                    if (localMin < min) // если точка составляет максимальный угол с текущей (косинус угла минимален)
                    {
                        min = localMin;
                        next = cur;
                    }
                    cur = cur.Next;
                }
                prev = new LinkedListNode<PointF>(current.Value);
                current = new LinkedListNode<PointF>(next.Value);
                convexHull.AddLast(next.Value);
                points.Remove(next);
            }
            while (current.Value != first.Value);
        }
    }
}
