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
    /// <summary>
    /// Classe contenant un élément affichable de type Stroke
    /// </summary>
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

        /// <summary>
        /// Renvoie les attributs du Stroke sous forme de string pour transmission au serveur
        /// </summary>
        /// <returns>String au format : strDrawingAttributes.Attribut1#DrawingAttributes.Attribut2...#%Point1.X;Point1.Y%....%Pointn.X;Pointn.Y</returns>
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

            return "str"+locval;
        }

        /// <summary>
        /// Ajoute le Stroke à l'InkCanvas en entrée
        /// </summary>
        /// <param name="window"></param>
        /// <param name="ink"></param>
        public override void AddToCanvas(MainWindow window, InkCanvas ink) //Ajoute l'élément Stroke contenu sur le InkCanvas
        {
            ink.Strokes.Add(Strokeat);
        }

        /// <summary>
        /// Supprime le Stroke de l'InkCanvas
        /// </summary>
        /// <param name="window"></param>
        /// <param name="ink"></param>
        public override void DeleteFromCanvas(MainWindow window, InkCanvas ink) //Retire du InkCanvas
        {
            ink.Strokes.Remove(Strokeat);
        }

        /// <summary>
        /// Renvoie le Stroke contenu sous la forme générique d'Object
        /// </summary>
        /// <returns>Attribut Stroke en tant qu'Object</returns>
        public override object getElement()
        {
            return this.Strokeat;
        }


        /// <summary>
        /// Sélectionne la Stroke dans l'InkCanvas
        /// </summary>
        /// <param name="window"></param>
        /// <param name="ink"></param>
        public override void selectInCanvas(MainWindow window, InkCanvas ink)
        {
            StrokeCollection strokeCollection = new StrokeCollection(new List<Stroke>{ this.Strokeat });
            ink.Select(strokeCollection, null);
        }

        /// <summary>
        /// Mise à jour de la position. Vide car il ne s'agit pas d'un attribut d'un Stroke
        /// </summary>
        /// <param name="inkCanvas"></param>
        internal override void updatePosition(InkCanvas inkCanvas)
        {
            
        }
    }
}
