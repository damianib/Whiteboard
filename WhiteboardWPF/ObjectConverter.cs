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
    class ObjectConverter
    {
        public static Object getObject(String str)
        {
            return ReconvertStroke(str);
        }

        public static String getString(Object o)
        {
            return ConvertStroke((Stroke)o);
        }
        public static string ConvertStroke(Stroke l_stroke)
        {
            string locval;
            DrawingAttributes attri = l_stroke.DrawingAttributes;
            locval = attri.Color.ToString() + "#" + attri.FitToCurve.ToString() + "#" + attri.Height.ToString() + "#" + attri.IgnorePressure.ToString() + "#" + attri.IsHighlighter.ToString() + "#" + attri.StylusTip.ToString() + "#" + attri.StylusTipTransform + "#" + attri.Width.ToString() + "#";
            locval = locval.TrimStart('#');

            foreach (var point in l_stroke.StylusPoints)
            {
                locval += "%" + point.X + ";" + point.Y;
            }
            return locval;
        }

        public static Stroke ReconvertStroke(string locval)
        {
            string str = locval;
            string[] strlst = locval.Split('#');
            string[] vals;
            Stroke stroke;

            StylusPointCollection collect = new StylusPointCollection();
            DrawingAttributes attri = new DrawingAttributes();


            attri.Color = (Color)ColorConverter.ConvertFromString("#" + strlst[0]);
            attri.FitToCurve = Boolean.Parse(strlst[1]);
            attri.Height = Double.Parse(strlst[2]);
            attri.IgnorePressure = Boolean.Parse(strlst[3]);
            attri.IsHighlighter = Boolean.Parse(strlst[4]);
            if (strlst[5] == "Rectangle")
            {
                attri.StylusTip = StylusTip.Rectangle;
            }
            else
            {
                attri.StylusTip = StylusTip.Ellipse;
            }
            attri.Width = Double.Parse(strlst[7]);

            str = strlst[8].TrimStart('%');
            strlst = str.Split('%');
            foreach (var coor in strlst)
            {
                if (coor is null)
                {
                    break;
                }
                else
                {
                    vals = coor.Split(';');
                    collect.Add(new StylusPoint(Double.Parse(vals[0]), Double.Parse(vals[1])));
                }

            }
            stroke = new Stroke(collect, attri);

            return stroke;
        }

        public static string TextBlockToString(TextBlockAndCoordinates block)
        {
            string str = "";
            char separator = Convert.ToChar(Int16.Parse("feff001f"));

            str += block.BlockT.Text + separator + block.BlockT.Height.ToString() + separator + block.BlockT.Width.ToString() + separator + block.X + separator + block.Y;

            return str;
        }

        public static TextBlockAndCoordinates StringToTextblock(string str)
        {
            TextBlock block = new TextBlock();
            char separator = Convert.ToChar(Int16.Parse("feff001f"));
            string[] strlst = str.Split(separator);

            block.Text = strlst[0];

            block.Height = Double.Parse(strlst[1]);

            block.Width = Double.Parse(strlst[2]);

            TextBlockAndCoordinates blockC = new TextBlockAndCoordinates(block, Double.Parse(strlst[3]), Double.Parse(strlst[4]));

            return blockC;
        }

    }
}
