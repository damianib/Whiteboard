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
        public int id;
        public abstract string GetString();
        public abstract void AddToCanvas(MainWindow window, InkCanvas ink);
        public abstract void DeleteFromCanvas(MainWindow window, InkCanvas ink);

        public abstract void selectInCanvas(MainWindow window, InkCanvas ink);
        public abstract Object getElement();
        internal abstract void updatePosition(InkCanvas inkCanvas);
    }
}
