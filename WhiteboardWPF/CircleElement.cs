using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Windows.Input;

namespace WhiteboardWPF
{
    class CircleElement : StrokeElement
    {
        public CircleElement(double x, double y)
        {
            double r = 10;
            List<StylusPoint> listStylusPoint = new List<StylusPoint>();
            for (int i = 0; i < 100; i++)
            {
                double newX = x + r*Math.Cos((double)i * 2 * Math.PI / 100);
                double newY = y + r*Math.Sin((double)i * 2 * Math.PI / 100);
                listStylusPoint.Add(new StylusPoint(newX, newY));
            }
            StylusPointCollection pointsCollection = new StylusPointCollection(listStylusPoint);
            this.Strokeat = new Stroke(pointsCollection);
        }
    }
}
