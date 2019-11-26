using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TCPServeur
{
    /// <summary>
    /// Classe correspondant au StrokeElement du client
    /// </summary>
    class StrokeElement : BoardElement
    {
        struct Point
        {
            public double x;
            public double y;

            public Point(double nx, double ny)
            {
                x = nx;
                y = ny;
            }
        }

        List<Point> lstPts = new List<Point>();
        string Color;
        bool FitToCurve;
        double Height;
        bool IgnorePressure;
        bool IsHighlighter;
        int StylusTip;
        double Width;

        /// <summary>
        /// Constructeur faisant la conversion String vers StrokeElement
        /// </summary>
        /// <param name="id">id unique de l'élément</param>
        /// <param name="locval">String contenant les attributs du Stroke pour conversion</param>
        public StrokeElement(int id, string locval)
        {
            this.m_id = id;

            string str = locval;
            
            string[] strlst = locval.Split('#');
            string[] vals;
            Color = strlst[0];
            FitToCurve = Boolean.Parse(strlst[1]);
            Height = Double.Parse(strlst[2]);
            IgnorePressure = Boolean.Parse(strlst[3]);
            IsHighlighter = Boolean.Parse(strlst[4]);
            if (strlst[5] == "Rectangle")
            {
                StylusTip = 0;
            }
            else
            {
                StylusTip = 1;
            }
            Width = Double.Parse(strlst[7]);
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
                    lstPts.Add(new Point(Double.Parse(vals[0]), Double.Parse(vals[1])));
                }

            }


        }

        /// <summary>
        /// Renvoie le StrokeElement sous forme de string pour transmission au client
        /// </summary>
        /// <returns>String représentant l'objet</returns>
        public override string GetString()
        {
            string locval;
            locval = Color + "#" + FitToCurve.ToString() + "#" + Height.ToString() + "#" + IgnorePressure.ToString() + "#" + IsHighlighter.ToString() + "#" + StylusTip.ToString() + "#" + "garbage" + "#" + Width.ToString() + "#";
            locval = locval.TrimStart('#');

            foreach (Point point in lstPts)
            {
                locval += "%" + point.x + ";" + point.y;
            }

            return "str" + locval;
        }
    }
}
