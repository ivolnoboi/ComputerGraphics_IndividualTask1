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
        /*LinkedList<PointF> points;
        LinkedList<PointF> convexHull;*/
        List<PointF> points;
        List<PointF> convexHull;
        Bitmap bmp;
        public Form1()
        {
            InitializeComponent();
            /*points = new LinkedList<PointF>();
            convexHull = new LinkedList<PointF>();*/
            points = new List<PointF>();
            convexHull = new List<PointF>();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Jarvis();
            DrawConvexHull();
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
            //points.AddLast(e.Location);
            points.Add(e.Location);
        }

        private void DrawConvexHull()
        {
            Graphics g = Graphics.FromImage(bmp);
            Pen p = new Pen(Color.Black);
            var prev = convexHull[0];
            for (int i = 1; i < convexHull.Count; i++)
            {
                g.DrawLine(p, prev, convexHull[i]);
                prev = convexHull[i];
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

        private float scalar_product(PointF p1, PointF p2)
        {
            return (p1.X*p2.X+p1.Y*p2.Y);
        }

        private void Jarvis()
        {/*
            PointF first = points.First.Value;
            foreach (var point in points) // Находим самую нижнюю точку. Если их несколько, то самую левую
            {
                if (pointBellowLeft(point, first))
                    first = point;
            }
            var current = points.Find(first);
            var firstVal = current.Value;
            convexHull.AddFirst(current.Value);
            points.Remove(current);*/
            PointF first = points[0];
            foreach (var point in points) // Находим самую нижнюю точку. Если их несколько, то самую левую
            {
                if (pointBellowLeft(point, first))
                    first = point;
            }
            convexHull.Add(first);
            //points.Remove(first);
            PointF current = first;
            do
            {
                PointF next = current;
                var min = float.MaxValue;
                foreach (var point in points) // Находим следующую точку
                {
                    var localMin = scalar_product(current, point);
                    if (localMin < min) // если точка составляет максимальный угол с текущей (минимальное скалярное произведение)
                    {
                        min = localMin;
                        next = point;
                    }
                }
                current = next;
                convexHull.Add(current);
                points.Remove(current);
            } 
            while (current != first);
        }
    }
}
