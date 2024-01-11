using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Racing
{
    public partial class NewRacerForm : Form
    {

        public Color colorPoint = Color.Black;
        public Color colorPath=Color.Black;
        private Racer racer = null;

        public NewRacerForm()
        {
            InitializeComponent();
        }

        internal Racer Racer { get => racer; set => racer = value; }

        private void button1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // установка цвета формы
            this.BackColor = colorDialog1.Color;
            colorPoint = colorDialog1.Color;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            // установка цвета формы
            this.BackColor = colorDialog1.Color;
            colorPath = colorDialog1.Color;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Racer = new Racer(colorPoint, colorPath, textBox1.Text);
            Close();
        }
    }
}
