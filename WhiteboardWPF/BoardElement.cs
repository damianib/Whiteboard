using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Ink;

namespace WhiteboardWPF
{
    abstract class BoardElement
    {
        public abstract string GetString();
        public abstract void AddToCanvas(InkCanvas ink);
        public abstract void DeleteFromCanvas(InkCanvas ink);
    }
}
