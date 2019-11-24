using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WhiteboardWPF
{
    class TextBoxElement : BoardElement
    {
        private TextBox BoxT = null;
        public double X { get; set; }
        public double Y { get; set; }

        public string Text { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

        public TextBoxElement(TextBox box, double x, double y, int id)
        {
            this.BoxT = box;
            this.X = x;
            this.Y = y;
            this.id = id;
        }

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

        public override string GetString()
        {
            string str = "";
            char separator = '\u0000';

            str += "txb" + this.BoxT.Text + separator + this.BoxT.Height.ToString() + separator + this.BoxT.Width.ToString() + separator + this.X + separator + this.Y;

            return str;
        }

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

        public override void DeleteFromCanvas(MainWindow window, InkCanvas ink)
        {
            if (ink.Children.Contains(BoxT))
                ink.Children.Remove(BoxT);
        }

        public override object getElement()
        {
            return this.BoxT;
        }

        public override void selectInCanvas(MainWindow window, InkCanvas ink)
        {
            List<UIElement> uIElements = new List<UIElement> { this.BoxT };
            ink.Select(null, uIElements);
        }

        internal override void updatePosition(InkCanvas inkCanvas)
        {
            Point relativeLocation = BoxT.TranslatePoint(new Point(0, 0), inkCanvas);
            this.X = relativeLocation.X;
            this.Y = relativeLocation.Y;
        }

        public override bool isContained(InkCanvas ink)
        {
            return ink.Children.Contains(this.BoxT);
        }
    }
}
