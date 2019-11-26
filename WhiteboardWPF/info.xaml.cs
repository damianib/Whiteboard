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
using System.Windows.Shapes;

namespace WhiteboardWPF
{
    /// <summary>
    /// Logique d'interaction pour info.xaml
    /// </summary>
    public partial class Info : Window
    {
        string ipAdress;
        string whiteboardName;

        /// <summary>
        /// Create an information popup
        /// </summary>
        /// <param name="ipAdress">IP adress in the popup</param>
        /// <param name="whiteboardName">Whiteboard name in the popup</param>
        public Info(string ipAdress, string whiteboardName)
        {
            InitializeComponent();
            this.ipAdress = ipAdress;
            this.whiteboardName = whiteboardName;
            ipAdressTextBlock.Text = ipAdress;
            serverNameTextBlock.Text = whiteboardName;
        }

        /// <summary>
        /// Copy the IP to the clipboard
        /// </summary>
        /// <param name="server"></param>
        /// <param name="e"></param>
        void ipClick(object server, System.EventArgs e)
        {
            Clipboard.SetText(this.ipAdress);
        }

        /// <summary>
        /// Copy the whiteboard's name to the clipboard
        /// </summary>
        /// <param name="server"></param>
        /// <param name="e"></param>
        void nameClick(object server, System.EventArgs e)
        {
            Clipboard.SetText(this.whiteboardName);
        }
    }
}
