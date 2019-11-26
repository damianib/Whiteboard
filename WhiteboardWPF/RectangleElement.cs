using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Ink;

namespace WhiteboardWPF
{
    class SquareElement : StrokeElement
    {

        public SquareElement()
        {

        }

        public SquareElement(StylusPoint A)
        {
            List<StylusPoint> listStylusPoint = new List<StylusPoint>();
            StylusPointCollection pointsCollection = new StylusPointCollection(listStylusPoint);
            pointsCollection.Add(A);
            for(int i = 1; i < 10; i++)
            {
                pointsCollection.Add(new StylusPoint(A.X + i, A.Y));
                pointsCollection.Add(new StylusPoint(A.X, A.Y + i));
            }
            this.Strokeat = new Stroke(pointsCollection);
        }
    }
}
