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
            modeBox.Items.Add("Create or join board");
            modeBox.Items.Add("Create new random board");
            modeBox.Items.Add("Create board with name");
            modeBox.Items.Add("Join board");
            choiceIp.Text = window.getIp();
            modeBox.SelectedIndex = 0;
        }
        public void clickConnect(object sender, System.EventArgs e)
        {
            Console.WriteLine(textChoiceIp.Text);
            if(modeBox.SelectedItem.Equals("Create new random board"))
            {
                window.doRestart(false, true, choiceIp.Text, choiceName.Text);
            }
            else if (modeBox.SelectedItem.Equals("Create board with name"))
            {
                window.doRestart(true, true, choiceIp.Text, choiceName.Text);
            }
            else if (modeBox.SelectedItem.Equals("Join board"))
            {
                window.doRestart(false, false, choiceIp.Text, choiceName.Text);
            }
            else if(modeBox.SelectedItem.Equals("Create or join board"))
            {
                window.doRestart(choiceIp.Text, choiceName.Text);
            }

            this.Close();
        }
        public void clickCancel(object sender, System.EventArgs e)
        {
            this.Close();
        }

        void modeBoxChanged(object sender, System.EventArgs e)
        {
            if (modeBox.SelectedItem.Equals("Create new random board"))
            {
                choiceName.IsEnabled = false;
                choiceName.Background = new SolidColorBrush(Color.FromRgb(100, 100, 100));
            }
            else
            {
                choiceName.IsEnabled = true;
                choiceName.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }
    }
}