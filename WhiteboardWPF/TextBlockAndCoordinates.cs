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
    class TextBlockAndCoordinates
    {
        public TextBlock BlockT { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public TextBlockAndCoordinates(TextBlock block, double x, double y)
        {
            this.BlockT = block;
            this.X = x;
            this.Y = Y;
        }

        public TextBlockAndCoordinates()
        {

        }
    }
}
