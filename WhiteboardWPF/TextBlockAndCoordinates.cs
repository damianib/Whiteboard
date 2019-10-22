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
    class TextBlockAndCoordinates : BoardElement
    {
        private TextBlock BlockT { get; set; }
        private double X { get; set; }
        private double Y { get; set; }

        public TextBlockAndCoordinates(TextBlock block, double x, double y)
        {
            this.BlockT = block;
            this.X = x;
            this.Y = Y;
        }

        public TextBlockAndCoordinates()
        {

        }

        public override string GetString()
        {
            string str = "";
            char separator = '\u0000';

            str += "txt" + this.BlockT.Text + separator + this.BlockT.Height.ToString() + separator + this.BlockT.Width.ToString() + separator + this.X + separator + this.Y;

            return str;
        }

        public override void AddToCanvas(InkCanvas ink)
        {
            Canvas.SetTop(BlockT, X);
            Canvas.SetLeft(BlockT, Y);
            ink.Children.Add(BlockT);
        }

        public override void DeleteFromCanvas(InkCanvas ink)
        {
            ink.Children.Remove(BlockT);
        }
    }
}
