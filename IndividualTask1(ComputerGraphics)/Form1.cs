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
        Bitmap bmp;
        public Form1()
        {
            InitializeComponent();
            points = new LinkedList<PointF>();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = bmp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Jarvis();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            points.Clear();
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

        private void Jarvis()
        {

        }
    }
}
