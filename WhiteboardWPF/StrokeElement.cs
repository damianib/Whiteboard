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
    class StrokeElement : BoardElement //Classe contenant un élément Stroke et les méthodes associées
        //Constructeurs : vide, ou initialisée avec un Stroke ou Stoke et id
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
        public override string GetString()  //Renvoie le Stroke sous forme de string pour transmission au serveur, sous la forme : 
            //strDrawingAttributes.Attribut1#DrawingAttributes.Attribut2...#%Point1.X;Point1.Y%....%Pointn.X;Pointn.Y
            //Un code de 3 caractères indiquant le type puis la liste des attributs
        {
            string locval;
            DrawingAttributes attri = Strokeat.DrawingAttributes;
            locval =  attri.Color.ToString() + "#" + attri.FitToCurve.ToString() + "#" + attri.Height.ToString() + "#" + attri.IgnorePressure.ToString() + "#" + attri.IsHighlighter.ToString() + "#" + attri.StylusTip.ToString() + "#" + attri.StylusTipTransform + "#" + attri.Width.ToString() + "#";
            locval =  locval.TrimStart('#');

            foreach (var point in Strokeat.StylusPoints)
            {
                locval += "%" + point.X + ";" + point.Y;
            }

            return "str"+locval;
        }

        public override void AddToCanvas(MainWindow window, InkCanvas ink) //Ajoute l'élément Stroke contenu sur le InkCanvas
        {
            ink.Strokes.Add(Strokeat);
        }

        public override void DeleteFromCanvas(MainWindow window, InkCanvas ink) //Retire du InkCanvas
        {
            ink.Strokes.Remove(Strokeat);
        }

        public override object getElement()
        {
            return this.Strokeat;
        }

        public override void selectInCanvas(MainWindow window, InkCanvas ink)
        {
            StrokeCollection strokeCollection = new StrokeCollection(new List<Stroke>{ this.Strokeat });
            ink.Select(strokeCollection, null);
        }

        internal override void updatePosition(InkCanvas inkCanvas)
        {
            
        }
    }
}
