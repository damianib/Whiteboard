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
        protected Stroke Strokeat;

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


        public StrokeElement(String shape, double x, double y, double x2, double y2)
        {
            if (shape.Equals("Circle"))
            {
                double xCenter = (x + x2) / 2;
                double yCenter = (y + y2) / 2;
                double xSize = (x - x2) / 2;
                double ySize = (y - y2) / 2;
                List<StylusPoint> listStylusPoint = new List<StylusPoint>();
                for (int i = 0; i < 101; i++)
                {
                    double newX = xCenter + xSize * Math.Cos((double)i * 2 * Math.PI / 100);
                    double newY = yCenter + ySize * Math.Sin((double)i * 2 * Math.PI / 100);
                    listStylusPoint.Add(new StylusPoint(newX, newY));
                }
                StylusPointCollection pointsCollection = new StylusPointCollection(listStylusPoint);
                this.Strokeat = new Stroke(pointsCollection);
            }
            if (shape.Equals("Rectangle"))
            {
                List<StylusPoint> listStylusPoint = new List<StylusPoint>();
                listStylusPoint.Add(new StylusPoint(x, y));
                listStylusPoint.Add(new StylusPoint(x, y2));
                listStylusPoint.Add(new StylusPoint(x2, y2));
                listStylusPoint.Add(new StylusPoint(x2, y));
                listStylusPoint.Add(new StylusPoint(x, y));

                StylusPointCollection pointsCollection = new StylusPointCollection(listStylusPoint);
                this.Strokeat = new Stroke(pointsCollection);
                this.Strokeat.DrawingAttributes.FitToCurve = false;
            }
        }
        
        public StrokeElement(List<StylusPoint> stylusPoints)
        {
            List<StylusPoint> listStylusPoint = new List<StylusPoint>();
            foreach(StylusPoint st in stylusPoints)
            {
                listStylusPoint.Add(st);
            }
            listStylusPoint.Add(stylusPoints[0]);
            StylusPointCollection pointsCollection = new StylusPointCollection(listStylusPoint);
            this.Strokeat = new Stroke(pointsCollection);
            this.Strokeat.DrawingAttributes.FitToCurve = false;
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

        public static void changeStroke(Stroke stroke, String shape, double x, double y, double x2, double y2)
        {
            {
                if (shape.Equals("Circle"))
                {
                    double xCenter = (x + x2) / 2;
                    double yCenter = (y + y2) / 2;
                    double xSize = (x - x2) / 2;
                    double ySize = (y - y2) / 2;
                    List<StylusPoint> listStylusPoint = new List<StylusPoint>();
                    for (int i = 0; i < 101; i++)
                    {
                        double newX = xCenter + xSize * Math.Cos((double)i * 2 * Math.PI / 100);
                        double newY = yCenter + ySize * Math.Sin((double)i * 2 * Math.PI / 100);
                        listStylusPoint.Add(new StylusPoint(newX, newY));
                    }
                    StylusPointCollection pointsCollection = new StylusPointCollection(listStylusPoint);
                    stroke.StylusPoints = pointsCollection;
                    stroke.DrawingAttributes.FitToCurve = true;
                }
                if (shape.Equals("Rectangle"))
                {
                    List<StylusPoint> listStylusPoint = new List<StylusPoint>();
                    listStylusPoint.Add(new StylusPoint(x, y));
                    listStylusPoint.Add(new StylusPoint(x, y2));
                    listStylusPoint.Add(new StylusPoint(x2, y2));
                    listStylusPoint.Add(new StylusPoint(x2, y));
                    listStylusPoint.Add(new StylusPoint(x, y));

                    StylusPointCollection pointsCollection = new StylusPointCollection(listStylusPoint);
                    stroke.StylusPoints = pointsCollection;
                    stroke.DrawingAttributes.FitToCurve = false;
                }
            }
        }

        public static void changeStroke(Stroke stroke, List<StylusPoint> stylusPoints)
        {
            List<StylusPoint> listStylusPoint = new List<StylusPoint>();
            foreach (StylusPoint st in stylusPoints)
            {
                listStylusPoint.Add(st);
            }
            //listStylusPoint.Add(stylusPoints[0]);
            StylusPointCollection pointsCollection = new StylusPointCollection(listStylusPoint);

            stroke.StylusPoints = pointsCollection;

            
        }
        public static void emtyStroke(Stroke stroke)
        {
            stroke.StylusPoints = new StylusPointCollection();
        }


    }
}
