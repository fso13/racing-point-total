using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Racing
{
    internal class Racer
    {
        public List<PointLabel> points = new List<PointLabel>();
        public System.Drawing.Color colorPoint;
        public System.Drawing.Color colorPath;

        public Racer(Color colorPoint, Color colorPath)
        {
            this.colorPoint = colorPoint;
            this.colorPath = colorPath;
        }
    }
}
