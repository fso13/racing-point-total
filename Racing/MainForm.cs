using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection.Emit;
using System.Windows.Forms;
using Label = System.Windows.Forms.Label;

namespace Racing
{
    public partial class MainForm : Form
    {
        private const int gridSize = 30;
        private List<PointLabel> points = new List<PointLabel>();
        private Point movingPoint;

        public MainForm()
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
            for (int i = 0; i < points.Count; i++)
            {
                PointLabel point = points[i];
                e.Graphics.FillEllipse(Brushes.Coral, point.point.X - 4, point.point.Y - 4, 8, 8);
                this.Text = point.point.X + " " + point.point.Y;
                point.label.Name = "label_1_" + i;
                point.label.Text = (i + 1).ToString();
                this.Controls.Add(point.label);
                point.label.BringToFront();
            }
            Double total = 0;

            using (Pen gridPen = new Pen(Color.Red))
                for (int x = 0; x < points.Count - 1; x++)
                {
                    Point one = points[x].point;
                    Point two = points[x + 1].point;

                    e.Graphics.DrawLine(gridPen, one.X, one.Y, two.X, two.Y);

                    total += Math.Round(Math.Sqrt(Math.Pow(one.X / gridSize - two.X / gridSize, 2) + Math.Pow(one.Y / gridSize - two.Y / gridSize, 2)), 3);

                }
            label1.Text = Math.Round(total, 3).ToString();

        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {

            
            if (e.Button == MouseButtons.Right && movingPoint.IsEmpty)
            {
                int newX = e.X / gridSize * gridSize;
                int newY = e.Y / gridSize * gridSize;

                movingPoint = new Point(newX, newY);
                this.Text = newX + " " + newY;
            }

            
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && !movingPoint.IsEmpty)
            {
                int newX = e.X / gridSize* gridSize;
                int newY = e.Y / gridSize * gridSize;


                for (int i = 0; i < points.Count; i++)
                {
                    Point p = points[i].point;
                    if(p.X == movingPoint.X && p.Y == movingPoint.Y)
                    {
                        points[i] = new PointLabel(new Point(newX,newY), points[i].label);

                    }

                }
                

                this.Text = newX + " " + newY;
                pictureBox1.Invalidate(); // Перерисовываем PictureBox
                movingPoint = Point.Empty;

            }

        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right && !movingPoint.IsEmpty)
            {
                int newX = e.X / gridSize * gridSize;
                int newY = e.Y / gridSize * gridSize;
                this.Text = newX+ " " + newY;
                

            }

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // Добавляем новую точку на пересечение сетки
                int newX = e.X / gridSize * gridSize;
                int newY = e.Y / gridSize * gridSize;


                Label label = new Label();
                label.Name = "label_1_" + points.Count;
                label.Text = (points.Count+1).ToString();

                label.Size = new Size(20, 15);
                points.Add(new PointLabel(new Point(newX, newY), label));
                listBox1.Items.Add(String.Format("{0}: x={1}, y={2}", points.Count, newX / gridSize, newY / gridSize));
                pictureBox1.Invalidate(); // Перерисовываем PictureBox
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex >= 0)
            {
                PointLabel pl = points[listBox1.SelectedIndex];

                this.Controls.Remove(pl.label);
                points.RemoveAt(listBox1.SelectedIndex);

                listBox1.Items.RemoveAt(listBox1.SelectedIndex);

                for (int i = 0; i < points.Count; i++)
                {
                    listBox1.Items[i] = (String.Format("{0}: x={1}, y={2}", i + 1, points[i].point.X / gridSize, points[i].point.Y / gridSize));
                }
                pictureBox1.Invalidate(); // Перерисовываем PictureBox
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;

                    //Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();

                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        fileContent = reader.ReadToEnd();
                    }
                }
            }

            pictureBox1.BackgroundImage = Image.FromFile(filePath);
        }
    }
}