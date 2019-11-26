
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
using System.Diagnostics;



namespace WhiteboardWPF
{
    /// <summary>
    /// ObjectConverter gère la reconversion des string reçus du serveur en BoardElement
    /// </summary>
    class ObjectConverter 
    {
        /*public static Object getObject(String str)
        {
            StrokeElement se = (StrokeElement) ReconvertElement(str);
            return se.GetStroke();
        } */

        /// <summary>
        /// Transforme le String en StrokeElement
        /// </summary>
        /// <param name="locval">String contenant les attributs de la Stroke</param>
        /// <returns>StrokeElement correspondant aux atttibuts reçus</returns>
        public static StrokeElement ReconvertStroke(string locval)
        {

            string str = locval;
            string[] strlst = locval.Split('#');
            string[] vals;
            Stroke stroke;
            Debug.WriteLine(locval);
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

            return new StrokeElement(stroke);
        }

        /// <summary>
        /// Conversion en TextBlockAndCoordinates. Obsolète
        /// </summary>
        /// <param name="str">String contenant les attributs du TextBlock</param>
        /// <returns>TextBlockAndCoordinates</returns>
        public static TextBlockAndCoordinates ReconvertTextblock(string str)
        {
            TextBlock block = new TextBlock();
            char separator = '\u0000';
            string[] strlst = str.Split(separator);

            block.Text = strlst[0];

            block.Height = Double.Parse(strlst[1]);

            block.Width = Double.Parse(strlst[2]);

            TextBlockAndCoordinates blockC = new TextBlockAndCoordinates(block, Double.Parse(strlst[3]), Double.Parse(strlst[4]));

            return blockC;
        }

        /// <summary>
        /// Convertit le String en TextBoxElement
        /// </summary>
        /// <param name="str">String contenant les attributs de la TextBox</param>
        /// <returns>TextBoxElement correspondant aux attributs transmis dans le String</returns>
        public static TextBoxElement ReconvertTextBox(string str)
        {
            TextBoxElement BoxT = new TextBoxElement();
            char separator = '\u0000';
            string[] strlst = str.Split(separator);

            BoxT.Text = strlst[0];

            BoxT.Height = Double.Parse(strlst[1]);

            BoxT.Width = Double.Parse(strlst[2]);

            BoxT.X = Double.Parse(strlst[3]);

            BoxT.Y = Double.Parse(strlst[4]);

            return BoxT;
        }

        /// <summary>
        /// Appelle la bonne procédure de conversion selon le code de 3 caractères au début du String reçu.
        /// str : StrokeElement
        /// txt : TextBlockAndCoordinates
        /// txb : TextBoxElement
        /// Exception si le code ne correspond pas à un type implémenté
        /// </summary>
        /// <param name="str">String contenant le code et les paramètres</param>
        /// <returns>BoardElement corrspondant</returns>
        public static BoardElement ReconvertElement(string str)
        {
            
            

            string identifier = str.Substring(0, 3);
            if (identifier.Equals("txt"))
            {
                return ReconvertTextblock(str.Substring(3));
            }
            else if (identifier.Equals("str"))
            {
                
                return ReconvertStroke(str.Substring(3));
            }
            else if (identifier.Equals("txb"))
            {
                return ReconvertTextBox(str.Substring(3));
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /*public static string getString(Object o)
        {
            if (typeof(Stroke).IsInstanceOfType(o)){
                StrokeElement stroke = new StrokeElement((Stroke)o);
                return stroke.GetString();
            }
            else if (typeof(TextBlockAndCoordinates).IsInstanceOfType(o))
            {
                TextBlockAndCoordinates txt = (TextBlockAndCoordinates)o;
                return txt.GetString();
            }
            else if (typeof(TextBoxElement).IsInstanceOfType(o))
            {
                TextBoxElement txt = (TextBoxElement)o;
                return txt.GetString();
            }
            else
            {
                throw new NotImplementedException();
            }
        }*/
    }
}
