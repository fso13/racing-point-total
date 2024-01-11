using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Racing
{
    internal class Racer
    {
        public List<PointLabel> points = new List<PointLabel>();
        public Color colorPoint;
        public Color colorPath;
        public string name;

        public Racer(Color colorPoint, Color colorPath, string name)
        {
            this.colorPoint = colorPoint;
            this.colorPath = colorPath;
            this.name = name;
        }
    }
}
