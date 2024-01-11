using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Racing
{
    internal class PointLabel

    {
        public Point point;
        public Label label;

        public PointLabel(Point point, Label label)
        {
            this.point = point;
            this.label = label;

            this.label.Location = new Point(point.X-25, point.Y - 15);
        }
    }
}
