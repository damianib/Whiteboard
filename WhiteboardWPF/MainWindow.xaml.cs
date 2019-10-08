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
using System.Net;
using System.Net.Sockets;
using System.Windows.Ink;
using System.Runtime.InteropServices;


namespace WhiteboardWPF
{
    
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<String> availableColorsStr = new List<String>() { "Black", "Red", "Green", "Blue" };
        List<Color> availableColors = new List<Color>() { Color.FromRgb(0, 0, 0), Color.FromRgb(255, 0, 0), Color.FromRgb(0, 255, 0), 
            Color.FromRgb(0, 0, 255) };
        bool isTextMode = false;

        Client client;

        public MainWindow()
        {
            AllocConsole();
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", 5035);
            client = new Client(tcpClient, doAddStroke, doSelectStroke, doDeselectStroke, doDeleteStroke, doModifStroke);
            client.start();

            InitializeComponent();
            inkCanvas.AddHandler(InkCanvas.MouseDownEvent, new MouseButtonEventHandler(clickCanvas), true);

            penStyleBox.Items.Add("Pen");
            penStyleBox.Items.Add("Eraser");

            for(int i = 0; i<availableColors.Count; i++)
            {
                var textBlockColor = new TextBlock();
                textBlockColor.Foreground = new SolidColorBrush(availableColors[i]);
                textBlockColor.Text = availableColorsStr[i];
                colorBox.Items.Add(textBlockColor);
            }

            colorBox.SelectedIndex = 0;
            penStyleBox.SelectedIndex = 0;

            inkCanvas.UseCustomCursor = true;
            inkCanvas.DefaultDrawingAttributes.StylusTip = System.Windows.Ink.StylusTip.Ellipse;
        }

        

        private void doAddStroke(int id, object o)
        {
            Dispatcher.Invoke(
                () =>
                {
                    textBlock.Text = "Coucou";
                    inkCanvas.Strokes.Add((Stroke)o);
                }
                );
        }

        private void doSelectStroke(int id) { }
        private void doDeselectStroke(int id) { }
        private void doDeleteStroke(int id) { }

        private void doModifStroke(int id, object o) { }

        void selectedPenStyle(object sender, System.EventArgs e) //switch between pen and eraser
        {
            if (penStyleBox.SelectedItem == "Pen")
            {
                inkCanvas.DefaultDrawingAttributes.Color = availableColors[colorBox.SelectedIndex];
            }
            else if (penStyleBox.SelectedItem == "Eraser")
            {
                inkCanvas.DefaultDrawingAttributes.Color = Color.FromRgb(255, 255, 255);
            }
        }

        void selectedColor(object sender, System.EventArgs e) // change pen color
        {
            if (penStyleBox.SelectedItem == "Pen")
            {
                inkCanvas.DefaultDrawingAttributes.Color = availableColors[colorBox.SelectedIndex];
            }
        }

        void clearAll(object sender, System.EventArgs e) // clear ink from canvas
        {
            inkCanvas.Strokes.Clear();
        }

        void strokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e) // collect last stroke collected
        {
            showStrokes();
            sendNewStroke(e.Stroke);
            inkCanvas.Strokes.Remove(e.Stroke);
        }

        void sendNewStroke(System.Windows.Ink.Stroke newStroke)
        {
            // to complete
        }

        void addNewStroke(System.Windows.Ink.Stroke newStroke)
        {
            inkCanvas.Strokes.Add(newStroke);
        }

        void sliderValueChanged(object sender, System.EventArgs e) // change width of stylus
        {
            inkCanvas.DefaultDrawingAttributes.Width = widthSlider.Value;
            inkCanvas.DefaultDrawingAttributes.Height = widthSlider.Value;
        }

        public void showStrokes()
        {
            string text = "| ";
            foreach (var stroke in inkCanvas.Strokes)
            {
                text += stroke.StylusPoints.First().ToPoint().ToString() + " -> " + stroke.StylusPoints.Last().ToPoint().ToString() + " | ";
            }
            textBlock.Text = text;
        }

        void clickTextButton(object sender, System.EventArgs e)
        {
            isTextMode = !(isTextMode);
            if (isTextMode)
            {
                textButton.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            else
            {
                textButton.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            }
        }

        void clickCanvas(object sender, MouseEventArgs e)
        {
            textBlock.Text = "HA";
            if (isTextMode)
            {
                TextBlock newTextBlock = new TextBlock();
                newTextBlock.Text = "Hello";
                inkCanvas.Children.Add(newTextBlock);
                InkCanvas.SetTop(newTextBlock, e.GetPosition(this).Y);
                InkCanvas.SetLeft(newTextBlock, e.GetPosition(this).X);
                InkCanvas.GetLeft(newTextBlock);
            }
        }

        [DllImport("Kernel32")] public static extern void AllocConsole();

        [DllImport("Kernel32")] public static extern void FreeConsole();
    }
}
