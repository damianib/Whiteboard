using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.VisualBasic;


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
        string currentMode = "ink"; // possible values : ink, text
        bool isCreatingATextBox = false;

        Client client;

        Dictionary<int, BoardElement> allBoardElements = new Dictionary<int, BoardElement>();
        Dictionary<IntPtr, int> elementToBoardId = new Dictionary<IntPtr, int>();

        public MainWindow()
        {
            AllocConsole();
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", 5035);
            client = new Client(tcpClient, doAdd, doSelectStroke, doDeselectStroke, doDeleteStroke);
            client.start();

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

        void changeMode(string newMode) // handle all operations related to changing mode between ink, text, selection etc...
        {
            SolidColorBrush selectedButtonColor = new SolidColorBrush(Color.FromRgb(100, 100, 100));

            if (newMode == currentMode) // if a mode is selected two times in a row, go back to ink (deselect button)
            {
                newMode = "ink";
            }

            if (newMode == "ink") // change inkcanvas editing mode and button color
            {
                inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
            }
            else if (newMode == "text")
            {
                inkCanvas.EditingMode = InkCanvasEditingMode.None;
                textButton.Background = selectedButtonColor;
            }
            else if (newMode == "select")
            {
                inkCanvas.EditingMode = InkCanvasEditingMode.Select;
                selectButton.Background = selectedButtonColor;
            }

            if (currentMode == "text") // put last mode button in correct color
            {
                textButton.ClearValue(Button.BackgroundProperty);
            }
            else if (currentMode == "select")
            {
                selectButton.ClearValue(Button.BackgroundProperty);
            }

            currentMode = newMode;
        }

        void clickTextButton(object sender, System.EventArgs e)
        {
            changeMode("text");
        }

        void clickSelectButton(object sender, System.EventArgs e)
        {
            changeMode("select");
        }

        DateTime lastClick = DateTime.Now;
        void clickCanvas(object sender, MouseButtonEventArgs e)
        {
            if ((currentMode == "text") && ((DateTime.Now - lastClick) > new TimeSpan(0, 0, 1)))
            {
                TextBox newTextBox = new TextBox
                {
                    Width = 100,
                    Height = 20,
                    MaxLines = 100,
                };
                newTextBox.LostFocus += new RoutedEventHandler(textBoxModified);
                inkCanvas.Children.Add(newTextBox);
                InkCanvas.SetLeft(newTextBox, e.GetPosition(this).X);
                InkCanvas.SetTop(newTextBox, e.GetPosition(this).Y);
                newTextBox.Focus();
                isCreatingATextBox = true;
                lastClick = DateTime.Now;
            }
        }

        // -----------------------------------------------------------------------------------------
        // EVENTS SENT TO CLIENT

        void strokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e) // send last stroke collected
        {
            client.ask_add(new StrokeElement(e.Stroke));
            inkCanvas.Strokes.Remove(e.Stroke);
        }

        void clickEraseAllButton(object sender, System.EventArgs e) // send erase all
        {
            Console.WriteLine(inkCanvas.Children);
        }

        void textBoxModified(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("HEY");
            TextBox sourceTextBox = (TextBox)e.Source;
            if (isCreatingATextBox)
            {
                client.ask_add(new TextBoxElement(sourceTextBox, InkCanvas.GetLeft(sourceTextBox), InkCanvas.GetRight(sourceTextBox)));
                inkCanvas.Children.Remove(sourceTextBox);
            }
            isCreatingATextBox = false;
        }

        // -----------------------------------------------------------------------------------------
        // FUNCTIONS CALLED FROM CLIENT

        private void doAdd(BoardElement boardElement) // add board element to ink canvas
        {
            if (allBoardElements.ContainsKey(boardElement.id))
            {
                doDelete(boardElement);
            }
            Dispatcher.Invoke(
                () =>
                {
                    
                    boardElement.AddToCanvas(inkCanvas);
                });

            allBoardElements[boardElement.id] = boardElement;
        }

        private void doDelete(BoardElement boardElement) // delete board element from ink canvas
        {
            Dispatcher.Invoke(
                () =>
                {
                    boardElement.DeleteFromCanvas(inkCanvas);
                });
            allBoardElements.Remove(boardElement.id);
        }

        private void doAddStroke(int id, object o) // add a new stroke to canvas
        {
            BoardElement b = (BoardElement)o;
            Dispatcher.Invoke(
                () =>
                {
                    b.AddToCanvas(inkCanvas);
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

        void doAddTextBox(string text, double x, double y) // add a new text box to canvas
        {
            Dispatcher.Invoke(
                () =>
                {
                    
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
