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
            return y / x; //Math.Atan(y / x);
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
            // Находим вторую точку. Она имеет наименьший положительный полярный угол относительно первой точки как начала координат.
            PointF second = first;
            PointF second2 = first;
            var minAngle = float.MaxValue;
            var maxAngle = float.MinValue;
            foreach (var point in points)
            {
                var localAngle = polarAngleTangent(first, point);
                if (localAngle <= minAngle && localAngle >= 0)
                {
                    second = point;
                    minAngle = localAngle;
                }
                if (localAngle>maxAngle&&localAngle<0)
                {
                    second2 = point;
                    maxAngle = localAngle;
                }
            }
            if (second == first)
                second = second2;
            convexHull.Add(second);
            points.Remove(second);
            PointF prev = first;
            PointF current = second;
            do
            {
                PointF next = current;
                var min = float.MaxValue;
                foreach (var point in points) // Находим следующую точку
                {
                    var localMin = cosValueBetweenVectors(prev, current, point);
                    if (localMin < min) // если точка составляет максимальный угол с текущей (минимальное скалярное произведение)
                    {
                        min = localMin;
                        next = point;
                    }
                }
                prev = current;
                current = next;
                convexHull.Add(next);
                points.Remove(next);
            }
            while (current != first);
        }
    }
}
