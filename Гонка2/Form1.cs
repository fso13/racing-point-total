using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Гонка2
{
    public partial class Form1 : Form
    {
        private const int gridSize = 40;
        private List<Point> points = new List<Point>();
        private Point movingPoint;

        public Form1()
        {
            InitializeComponent();
            pictureBox1.Paint += PictureBox1_Paint;
            pictureBox1.MouseDown += PictureBox1_MouseDown;
            pictureBox1.MouseUp += PictureBox1_MouseUp;
            pictureBox1.MouseMove += PictureBox1_MouseMove;
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // Рисуем сетку
            using (Pen gridPen = new Pen(Color.LightGray))
            {
                for (int x = 0; x < pictureBox1.Width; x += gridSize)
                {
                    e.Graphics.DrawLine(gridPen, x, 0, x, pictureBox1.Height);
                }
                for (int y = 0; y < pictureBox1.Height; y += gridSize)
                {
                    e.Graphics.DrawLine(gridPen, 0, y, pictureBox1.Width, y);
                }
            }

            // Рисуем точки
            foreach (Point point in points)
            {
                e.Graphics.FillEllipse(Brushes.Black, point.X - 4, point.Y - 4, 8, 8);
                this.Text = point.X + " " + point.Y;
            }

            using (Pen gridPen = new Pen(Color.Red))
                for (int x = 0; x < points.Count - 1; x++)
                {
                    Point one = points[x];
                    Point two = points[x + 1];

                    e.Graphics.DrawLine(gridPen, one.X, one.Y, two.X, two.Y);
                }
        
        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {

            
            if (e.Button == MouseButtons.Right && movingPoint.IsEmpty)
            {
                int newX = (e.X / gridSize) * gridSize;
                int newY = (e.Y / gridSize) * gridSize;

                movingPoint = new Point(newX, newY);
                this.Text = newX + " " + newY;
            }

            
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && !movingPoint.IsEmpty)
            {
                int newX = (e.X / gridSize) * gridSize;
                int newY = (e.Y / gridSize) * gridSize;


                for (int i = 0; i < points.Count; i++)
                {
                    Point p = points[i];
                    if(p.X == movingPoint.X && p.Y == movingPoint.Y)
                    {
                        points[i] = new Point(newX,newY);

                    }

                }
                

                this.Text = newX + " " + newY;
                pictureBox1.Invalidate(); // Перерисовываем PictureBox
                movingPoint = Point.Empty;

            }

            // Начинаем перемещение точки
            //movingPoint = Point.Empty;
            //foreach (Point point in points)
            //{
            //    if (Math.Abs(point.X - e.X) <= 5 && Math.Abs(point.Y - e.Y) <= 5)
            //    {
            //        movingPoint = point;
            //        break;
            //    }
            //}
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right && !movingPoint.IsEmpty)
            {
                int newX = (e.X / gridSize) * gridSize;
                int newY = (e.Y / gridSize) * gridSize;
                this.Text = newX+ " " + newY;
                

            }

            // Перемещаем точку
            //if (e.Button == MouseButtons.Left && !movingPoint.IsEmpty)
            //{
            //    movingPoint.X = (e.X / gridSize) * gridSize;
            //    movingPoint.Y = (e.Y / gridSize) * gridSize;
            //    pictureBox1.Invalidate(); // Перерисовываем PictureBox
            //}
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Добавляем новую точку на пересечение сетки
                int newX = (e.X / gridSize) * gridSize;
                int newY = (e.Y / gridSize) * gridSize;
                points.Add(new Point(newX, newY));
                listBox1.Items.Add(String.Format("{0}: x={1}, y={2}", points.Count, newX / gridSize, newY / gridSize));
                pictureBox1.Invalidate(); // Перерисовываем PictureBox
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            points.RemoveAt(listBox1.SelectedIndex);

            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            pictureBox1.Invalidate(); // Перерисовываем PictureBox

        }
    }
}