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
        string serverName;
        public Info(string ipAdress, string serverName)
        {
            InitializeComponent();
            this.ipAdress = ipAdress;
            this.serverName = serverName;
            ipAdressTextBlock.Text = ipAdress;
            serverNameTextBlock.Text = serverName;
        }

        void ipClick(object server, System.EventArgs e)
        {
            Clipboard.SetText(this.ipAdress);
        }

        void nameClick(object server, System.EventArgs e)
        {
            Clipboard.SetText(this.serverName);
        }
    }
}
