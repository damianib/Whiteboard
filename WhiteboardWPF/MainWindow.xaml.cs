using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;


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
        bool online = true;

        public MainWindow()
        {
            AllocConsole();
            if (online)
            {
                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect("127.0.0.1", 5035);
                client = new Client(tcpClient, doAddStroke, doSelectStroke, doDeselectStroke, doDeleteStroke, doModifStroke);
                client.start();
            }

            InitializeComponent();
            inkCanvas.AddHandler(InkCanvas.MouseDownEvent, new MouseButtonEventHandler(clickCanvas), true);

            penStyleBox.Items.Add("Pen");
            penStyleBox.Items.Add("Eraser");

            for (int i = 0; i < availableColors.Count; i++)
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


        // -----------------------------------------------------------------------------------------
        // LOCAL CHANGES

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

        void selectedStylusWidth(object sender, System.EventArgs e) // change width of stylus
        {
            inkCanvas.DefaultDrawingAttributes.Width = widthSlider.Value;
            inkCanvas.DefaultDrawingAttributes.Height = widthSlider.Value;
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
                TextBlock newTextBlock = new TextBlock
                {
                    Text = "Hello"
                };
                inkCanvas.Children.Add(newTextBlock);
                InkCanvas.SetTop(newTextBlock, e.GetPosition(this).Y);
                InkCanvas.SetLeft(newTextBlock, e.GetPosition(this).X);
            }
        }


        // -----------------------------------------------------------------------------------------
        // EVENTS SENT TO CLIENT

        void strokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e) // collect last stroke collected
        {
            if (online)
            {
                client.ask_add(e.Stroke);
                inkCanvas.Strokes.Remove(e.Stroke);
            }
        }

        void clickEraseAllButton(object sender, System.EventArgs e)
        {
        }


        // -----------------------------------------------------------------------------------------
        // FUNCTIONS CALLED FROM CLIENT

        private void doAddStroke(int id, object o) // add a new stroke to canvas
        {
            Dispatcher.Invoke(
                () =>
                {
                    textBlock.Text = "Coucou";
                    inkCanvas.Strokes.Add((Stroke)o);
                }
                );
        }

        void doEraseAll() // clear ink from canvas
        {
            Dispatcher.Invoke(
                () =>
                {
                    inkCanvas.Strokes.Clear();
                });
        }

        void doAddTextBlock(string text, int x, int y) // add a new text block to canvas at specified position
        {
            Dispatcher.Invoke(
                () =>
                {
                    TextBlock newTextBlock = new TextBlock
                    {
                        Text = text
                    };
                    inkCanvas.Children.Add(newTextBlock);
                    InkCanvas.SetLeft(newTextBlock, x);
                    InkCanvas.SetTop(newTextBlock, y);
                });
        }

        private void doDeleteStroke(int id) // delete given stroke from canvas
        {
            Dispatcher.Invoke(
                () =>
                {
                    inkCanvas.Strokes.Remove(inkCanvas.Strokes[id]);
                });
        }

        private void doSelectStroke(int id) { }
        private void doDeselectStroke(int id) { }
        private void doModifStroke(int id, object o) { }


        // -----------------------------------------------------------------------------------------
        // CONSOLE

        [DllImport("Kernel32")] public static extern void AllocConsole();

        [DllImport("Kernel32")] public static extern void FreeConsole();
    }
}
