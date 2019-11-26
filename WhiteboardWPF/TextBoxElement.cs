using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WhiteboardWPF
{
    /// <summary>
    /// Elément contenant une boîte de texte, les dimensions et les coordonnées
    /// </summary>
    class TextBoxElement : BoardElement
    {
        /// <summary>
        /// Attribut contenant la TextBox correspondante
        /// </summary>
        private TextBox BoxT = null;

        /// <summary>
        /// Abscisse de la TextBox
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Ordonnée de la TextBox
        /// </summary>
        public double Y { get; set; }


        /// <summary>
        /// Texte à afficher
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Hauteur
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Largeur
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// Constructeur sans dimensions
        /// </summary>
        /// <param name="box"> TextBox de l'élément</param>
        /// <param name="x"> Abscisse</param>
        /// <param name="y">Ordonnées</param>
        /// <param name="id">ID unique de l'objet</param>
        public TextBoxElement(TextBox box, double x, double y, int id)
        {
            this.BoxT = box;
            this.X = x;
            this.Y = y;
            this.id = id;
        }

        /// <summary>
        /// Constructeur avec l'ensemble des attributs
        /// </summary>
        /// <param name="height">Hauteur de la TextBox</param>
        /// <param name="width">Largeur</param>
        /// <param name="text">Texte de la TextBox</param>
        /// <param name="x">Abscisse</param>
        /// <param name="y">Ordonnée</param>
        /// <param name="id">ID unique de l'objet</param>
        public TextBoxElement(double height, double width, string text, double x, double y, int id)
        {
            this.Height = height;
            this.Width = width;
            this.Text = text;
            this.X = x;
            this.Y = y;
        }

        public TextBoxElement()
        {
            
        }

        /// <summary>
        /// Renvoir les attributs de l'objet dans un string pour transmission au serveur
        /// </summary>
        /// <returns>String au format Format : txb'\u0000'Texte....  '\u0000' : séparateur pour les attributs, caractère non affichable</returns>
        public override string GetString()
        {
            string str = "";
            char separator = '\u0000';

            str += "txb" + this.BoxT.Text + separator + this.BoxT.Height.ToString() + separator + this.BoxT.Width.ToString() + separator + this.X + separator + this.Y;

            return str;
        }

        /// <summary>
        /// Ajoute la TextBox à l'InkCanvas en entrée
        /// </summary>
        /// <param name="window">Fenêtre du Whiteboard</param>
        /// <param name="ink">InkCanvas cible</param>
        public override void AddToCanvas(MainWindow window, InkCanvas ink)
        {
            BoxT = new TextBox();
            BoxT.Text = this.Text;
            BoxT.Width = this.Width;
            BoxT.Height = this.Height;
            InkCanvas.SetTop(BoxT, this.Y);
            InkCanvas.SetLeft(BoxT, this.X);
            BoxT.LostFocus += new RoutedEventHandler(window.textBoxModified);
            ink.Children.Add(BoxT);
        }

        /// <summary>
        /// Supprime de l'InkCanvas
        /// </summary>
        /// <param name="window"></param>
        /// <param name="ink"></param>
        public override void DeleteFromCanvas(MainWindow window, InkCanvas ink)
        {
            ink.Children.Remove(BoxT);
        }

        /// <summary>
        /// Renvoie la TextBox sous la forme générique Object
        /// </summary>
        /// <returns>(Object) TextBox</returns>
        public override object getElement()
        {
            return this.BoxT;
        }

        /// <summary>
        /// Sélectionne la Textbox dans l'InkCanvas
        /// </summary>
        /// <param name="window"></param>
        /// <param name="ink"></param>
        public override void selectInCanvas(MainWindow window, InkCanvas ink)
        {
            List<UIElement> uIElements = new List<UIElement> { this.BoxT };
            ink.Select(null, uIElements);
        }

        /// <summary>
        /// Met à jour les attributs de posisiton après un déplacement
        /// </summary>
        /// <param name="inkCanvas"></param>
        internal override void updatePosition(InkCanvas inkCanvas)
        {
            Point relativeLocation = BoxT.TranslatePoint(new Point(0, 0), inkCanvas);
            this.X = relativeLocation.X;
            this.Y = relativeLocation.Y;
        }
    }
}
