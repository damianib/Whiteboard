using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WhiteboardWPF
{
    class TextBoxElement : BoardElement
    {
        private TextBox BoxT;
        private double X;
        private double Y;

        public TextBoxElement(TextBox box, double x, double y)
        {
            this.BoxT = box;
            this.X = x;
            this.Y = y;
        }

        public override string GetString()
        {
            string str = "";
            char separator = '\u0000';

            str += "txb" + this.BoxT.Text + separator + this.BoxT.Height.ToString() + separator + this.BoxT.Width.ToString() + separator + this.X + separator + this.Y;

            return str;
        }

        public override void AddToCanvas(InkCanvas ink)
        {
            throw new NotImplementedException();
        }
    }
}
