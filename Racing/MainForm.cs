using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection.Emit;
using System.Windows.Forms;
using Image = System.Drawing.Image;
using Label = System.Windows.Forms.Label;

namespace Racing
{
    public partial class MainForm : Form
    {
        private float zoom = 1.0f;

        List<Racer> racers = new List<Racer>();
        private int gridSize = 30;
        private int newGridSize = 30;
        private Point movingPoint;
        private Image image;
        Color gridColor = Color.LightGray;
        List<Label> formLabelsX = new List<Label>();
        List<Label> formLabelsY = new List<Label>();

        public MainForm()
        {
            InitializeComponent();
            pictureBox1.Paint += PictureBox1_Paint;
            pictureBox1.MouseDown += PictureBox1_MouseDown;
            pictureBox1.MouseUp += PictureBox1_MouseUp;
            pictureBox1.MouseMove += PictureBox1_MouseMove;
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;

        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            for(int i = 0; i < formLabelsX.Count; i++)
            {
                this.Controls.Remove(formLabelsX[i]);
            }


            for (int i = 0; i < formLabelsY.Count; i++)
            {
                this.Controls.Remove(formLabelsY[i]);
            }
            // Рисуем сетку
            using (Pen gridPen = new Pen(gridColor,1))
            {
                for (int x = 0; x < pictureBox1.Width; x += newGridSize)
                {
                    e.Graphics.DrawLine(gridPen, x, 0, x, pictureBox1.Height);                    

                    Label label = new Label();
                    label.Name = "label_x_" +x;
                    label.Text = (x / newGridSize).ToString();
                    label.Location = new Point(x , 5);
                    label.Size = new Size(20, 15);
                    formLabelsX.Add(label);
                    this.Controls.Add(label);
                    label.BringToFront();
                }
                for (int y = 0; y < pictureBox1.Height; y += newGridSize)
                {
                    e.Graphics.DrawLine(gridPen, 0, y, pictureBox1.Width, y);

                    Label label = new Label();
                    label.Name = "label_y_" + y;
                    label.Text = (y / newGridSize).ToString();
                    label.Location = new Point(5, y +newGridSize / 2);
                    label.Size = new Size(20, 15);
                    formLabelsY.Add(label);
                    this.Controls.Add(label);
                    label.BringToFront();
                }
            }

            foreach (Racer rac in racers)
            {
                // Рисуем точки
                for (int i = 0; i < rac.points.Count; i++)
                {
                    PointLabel point = rac.points[i];
                    point.point.X = point.point.X / gridSize * newGridSize;
                    point.point.Y = point.point.Y / gridSize * newGridSize;
                    e.Graphics.FillEllipse(new SolidBrush(rac.colorPoint), point.point.X / gridSize * newGridSize - 4, point.point.Y - 4, 8, 8);
                    this.Text = point.point.X + " " + point.point.Y;
                    point.label.Name = "label_1_" + i;
                    point.label.Text = (i + 1).ToString();
                    point.label.Location = new Point(point.point.X - 25, point.point.Y - 15);
                    this.Controls.Add(point.label);
                    point.label.BringToFront();


                }

                gridSize = newGridSize;
                Double total = 0;

                using (Pen gridPen = new Pen(rac.colorPath))
                    for (int x = 0; x < rac.points.Count - 1; x++)
                    {
                        Point one = rac.points[x].point;
                        Point two = rac.points[x + 1].point;

                        e.Graphics.DrawLine(gridPen, one.X, one.Y, two.X, two.Y);

                        total += Math.Round(Math.Sqrt(Math.Pow(one.X / gridSize - two.X / gridSize, 2) + Math.Pow(one.Y / gridSize - two.Y / gridSize, 2)), 3);

                    }

                tabControl1.TabPages[racers.Count - 1].Controls[1].Text = "Расстояние: " + getTotal(rac) + "  |  Скороcть: " + getSpeed(rac);
                tabControl1.TabPages[racers.Count - 1].Text = String.Format("{0}: N={1} S={2} S/N={3}", rac.name, rac.points.Count, getTotal(rac), getSpeed(rac));
            }
          


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
            if (e.Button == MouseButtons.Right && !movingPoint.IsEmpty && !button3.Enabled)
            {
                int newX = e.X / gridSize * gridSize;
                int newY = e.Y / gridSize * gridSize;

                Racer racer = GetRacer();
                for (int i = 0; i < racer.points.Count; i++)
                {
                    Point p = racer.points[i].point;
                    if (p.X == movingPoint.X && p.Y == movingPoint.Y)
                    {
                        racer.points[i] = new PointLabel(new Point(newX, newY), racer.points[i].label);

                    }

                }


                this.Text = newX + " " + newY;
                pictureBox1.Invalidate(); // Перерисовываем PictureBox
                movingPoint = Point.Empty;

            }

        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right && !movingPoint.IsEmpty && !button3.Enabled)
            {
                int newX = e.X / gridSize * gridSize;
                int newY = e.Y / gridSize * gridSize;
                this.Text = newX + " " + newY;


            }

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !button3.Enabled)
            {
                // Добавляем новую точку на пересечение сетки
                int newX = e.X / gridSize * gridSize;
                int newY = e.Y / gridSize * gridSize;

                Racer racer = GetRacer();

                Label label = new Label();
                label.Name = "label_1_" + racer.points.Count;
                label.Text = (racer.points.Count + 1).ToString();

                label.Size = new Size(20, 15);
                racer.points.Add(new PointLabel(new Point(newX, newY), label));

                ListBox listBox1 = (ListBox)tabControl1.TabPages[racers.Count-1].Controls[0];


                listBox1.Items.Add(String.Format("{0}: x={1}, y={2}", racer.points.Count, newX / gridSize, newY / gridSize));
                pictureBox1.Invalidate(); // Перерисовываем PictureBox
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            ListBox listBox1 = (ListBox)tabControl1.TabPages[tabControl1.SelectedIndex].Controls[0];
            if (listBox1.SelectedIndex >= 0)
            {
                Racer racer = racers[tabControl1.SelectedIndex];


                PointLabel pl = racer.points[listBox1.SelectedIndex];

                this.Controls.Remove(pl.label);
                racer.points.RemoveAt(listBox1.SelectedIndex);

                listBox1.Items.RemoveAt(listBox1.SelectedIndex);

                for (int i = 0; i < racer.points.Count; i++)
                {
                    listBox1.Items[i] = (String.Format("{0}: x={1}, y={2}", i + 1, racer.points[i].point.X / gridSize, racer.points[i].point.Y / gridSize));
                }
                pictureBox1.Invalidate(); // Перерисовываем PictureBox
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("1.Добавить гонщика\n2.Начать гонку.\n3. Завершить гонку.\nПовторить п.п. 1-3 для каждого гонщика.\nВыберите цвет сетки");
            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // установка цвета формы
            gridColor = colorDialog1.Color;

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

            if(filePath != string.Empty)
            {
                image = Image.FromFile(filePath);

                pictureBox1.Image = image;

            }

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            newGridSize = ((int)numericUpDown1.Value);

            pictureBox1.Invalidate(); // Перерисовываем PictureBox

        }


        private void button3_Click(object sender, EventArgs e)
        {
            NewRacerForm newRacerForm = new NewRacerForm();
            newRacerForm.ShowDialog();
            racers.Add(newRacerForm.Racer);
            button4.Enabled = true;



            TabPage tabPage = new TabPage(GetRacer().name);
            ListBox listBox = new ListBox();
            listBox.Location = new Point(10, 10);
            listBox.Dock = DockStyle.Fill;
            tabPage.Tag = GetRacer();
            // Добавление ListBox на вкладку
            tabPage.Controls.Add(listBox);

            // Добавление вкладки в TabControl
            tabControl1.TabPages.Add(tabPage);
            tabControl1.SelectedTab = tabPage;

            Label label = new Label();
            label.Text = "Расстояние: 0 | Скороcть: 0";
            label.Dock = DockStyle.Bottom;
            tabPage.Controls.Add(label);

        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (button3.Enabled)
            {
                pictureBox1.Enabled = true;
                button3.Enabled = false;
                button4.Text = "Закончить гонку";
            }
            else
            {
                pictureBox1.Enabled = false;
                button3.Enabled = true;
                button4.Text = "Начать гонку";
                button4.Enabled = false;

                Racer racer = GetRacer();
                string s =String.Format("Гонщик: {0} | Расстояние:{1} | Скороcть:{2}", racer.name, getTotal(racer), getSpeed(racer));

                ListBox listBox = (ListBox)tabControl1.TabPages[racers.Count - 1].Controls[0];
                listBox.Items.Add(s);
            }

        }

        private Double getTotal(Racer racer)
        {
            Double total = 0;
            for (int x = 0; x < racer.points.Count - 1; x++)
            {
                Point one = racer.points[x].point;
                Point two = racer.points[x + 1].point;
                total += Math.Round(Math.Sqrt(Math.Pow(one.X / gridSize - two.X / gridSize, 2) + Math.Pow(one.Y / gridSize - two.Y / gridSize, 2)), 3);

            }

            return Math.Round(total,3);
        }


        private Double getSpeed(Racer racer)
        {
            return Math.Round(getTotal(racer) / racer.points.Count, 3);
        }


        private Racer GetRacer()
        {
            return racers[racers.Count - 1];
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                zoom += 0.01f;
            }
            else
            {
                zoom -= 0.01f;
            }

            if (zoom < 0.01f)
            {
                zoom = 0.01f;
            }

            UpdateImage();
        }

        private void UpdateImage()
        {
            if (image != null)
            {
                int newWidth = (int)(image.Width * zoom);
                int newHeight = (int)(image.Height * zoom);

                Bitmap bmp = new Bitmap(newWidth, newHeight);
                Graphics g = Graphics.FromImage(bmp);
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight));

                pictureBox1.Image = bmp;
            }
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            pictureBox1.Focus();
        }
    }
}