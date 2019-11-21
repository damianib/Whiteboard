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
    class StrokeElement : BoardElement
    {
        private Stroke Strokeat;

        public StrokeElement()
        {

        }

        public StrokeElement(Stroke stroke)
        {
            this.Strokeat = stroke;
        }

        public StrokeElement(Stroke stroke, int id)
        {
            this.Strokeat = stroke;
            this.id = id;
        }

        public Stroke GetStroke()
        {
            return Strokeat;
        }
        public override string GetString()
        {
            string locval;
            DrawingAttributes attri = Strokeat.DrawingAttributes;
            locval =  attri.Color.ToString() + "#" + attri.FitToCurve.ToString() + "#" + attri.Height.ToString() + "#" + attri.IgnorePressure.ToString() + "#" + attri.IsHighlighter.ToString() + "#" + attri.StylusTip.ToString() + "#" + attri.StylusTipTransform + "#" + attri.Width.ToString() + "#";
            locval =  locval.TrimStart('#');

            foreach (var point in Strokeat.StylusPoints)
            {
                locval += "%" + point.X + ";" + point.Y;
            }

            //locval = "str" + locval;
            return "str"+locval;
        }

        public override void AddToCanvas(MainWindow window, InkCanvas ink)
        {
            ink.Strokes.Add(Strokeat);
        }

        public override void DeleteFromCanvas(MainWindow window, InkCanvas ink)
        {
            ink.Strokes.Remove(Strokeat);
        }

        public override object getElement()
        {
            return this.Strokeat;
        }
    }
}
