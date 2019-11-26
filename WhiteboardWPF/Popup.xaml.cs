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
    /// Logique d'interaction pour Popup.xaml
    /// </summary>
    public partial class Popup : Window
    {
        private MainWindow window;
        public Popup(MainWindow window)
        {
            InitializeComponent();
            this.window = window;
            penStyleBox.Items.Add("Create new random board");
            penStyleBox.Items.Add("Create board with name");
            penStyleBox.Items.Add("Join board");
            choiceIp.Text = window.getIp();
            penStyleBox.SelectedIndex = 0;
        }
        public void clickConnect(object sender, System.EventArgs e)
        {
            Console.WriteLine(textChoiceIp.Text);
            if(penStyleBox.SelectedItem.Equals("Create new random board"))
            {
                window.doRestart(false, true, choiceIp.Text, choiceName.Text);
            }
            else if (penStyleBox.SelectedItem.Equals("Create board with name"))
            {
                window.doRestart(true, true, choiceIp.Text, choiceName.Text);
            }
            else if (penStyleBox.SelectedItem.Equals("Join board"))
            {
                window.doRestart(false, false, choiceIp.Text, choiceName.Text);
            }

            this.Close();
        }
        public void clickCancel(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
