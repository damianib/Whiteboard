
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
    class ObjectConverter //Objet gérant la conversion des strings reçus du serveur en BoardElement
    {
        /*public static Object getObject(String str)
        {
            StrokeElement se = (StrokeElement) ReconvertElement(str);
            return se.GetStroke();
        } */


        public static StrokeElement ReconvertStroke(string locval) //Transforme en StrokeElement
            //Récupère les attributs séparés selon a convention et renvoi le StrokeElement correspondant
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

        public static TextBoxElement ReconvertTextBox(string str) //Transforme en TextBoxElement
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


        public static BoardElement ReconvertElement(string str) //Procédure appelée pour une reconversion
            //Appelle la bonne procédure selon le code de type (les 3 premiers caractères)
        {
            
            

            string identifier = str.Substring(0, 3);
            if (identifier.Equals("txt")) //Pour un TextBlockAndCoordinates (Obsolète)
            {
                return ReconvertTextblock(str.Substring(3));
            }
            else if (identifier.Equals("str")) //Pour un StrokeElement
            {
                
                return ReconvertStroke(str.Substring(3));
            }
            else if (identifier.Equals("txb")) //Pour un TextBoxElement
            {
                return ReconvertTextBox(str.Substring(3));
            }
            else //Erreur si le code ne correspond pas à un type implémenté
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
