using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
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

        TextBlock texting = new TextBlock();
        List<String> availableColorsStr = new List<String>() { "Black", "Red", "Green", "Blue" };
        List<Color> availableColors = new List<Color>() { Color.FromRgb(0, 0, 0), Color.FromRgb(255, 0, 0), Color.FromRgb(0, 255, 0),
            Color.FromRgb(0, 0, 255) };
        string currentMode = "ink"; // possible values : ink, text
        bool isCreatingATextBox = false;
        bool isSelectionFromServer = false;

        Client client;

        ObjectIDGenerator objectIDGenerator = new ObjectIDGenerator();
        Dictionary<int, BoardElement> allBoardElements = new Dictionary<int, BoardElement>();
        Dictionary<long, int> objectIdToBoardId = new Dictionary<long, int>();
        public int getBoardIdFromObject(Object obj)
        {
            bool firstTime = false;
            long objectId = objectIDGenerator.GetId(obj, out firstTime);
            return objectIdToBoardId[objectId];
        }

        public MainWindow()
        {
            AllocConsole();
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect("127.0.0.1", 5035);
            client = new Client(tcpClient, doAdd, doSelect, doDeselect, doDelete, doEraseAll);
            client.m_nomServer = "coucou";
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
            inkCanvas.Children.Add(texting);
            texting.Text = "Initial text";
        }


        // -----------------------------------------------------------------------------------------
        // LOCAL CHANGES

        void selectedPenStyle(object sender, System.EventArgs e) //switch between pen and eraser
        {
            if ((string)penStyleBox.SelectedItem == "Pen")
            {
                inkCanvas.DefaultDrawingAttributes.Color = availableColors[colorBox.SelectedIndex];
            }
            else if ((string)penStyleBox.SelectedItem == "Eraser")
            {
                inkCanvas.DefaultDrawingAttributes.Color = Color.FromRgb(255, 255, 255);
            }
        }

        void selectedColor(object sender, System.EventArgs e) // change pen color
        {
            if ((string)penStyleBox.SelectedItem == "Pen")
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
            client.ask_clear_all();
        }

        public void textBoxModified(object sender, RoutedEventArgs e)
        {
            TextBox sourceTextBox = (TextBox)e.Source;
            if (isCreatingATextBox)
            {
                client.ask_add(new TextBoxElement(sourceTextBox, InkCanvas.GetLeft(sourceTextBox), InkCanvas.GetTop(sourceTextBox),0));
                inkCanvas.Children.Remove(sourceTextBox);
                isCreatingATextBox = false;
            }
            texting.Text = "HALP";
        }

        void selectionChanged(object sender, System.EventArgs e)
        {
            ReadOnlyCollection<UIElement> selectedElements = inkCanvas.GetSelectedElements();
            StrokeCollection selectedStrokes = inkCanvas.GetSelectedStrokes();
            int boardId = -1;

            if (selectedElements.Count > 0)
            {
                boardId = getBoardIdFromObject(selectedElements[0]);
            }
            else if (selectedStrokes.Count > 0)
            {
                boardId = getBoardIdFromObject(selectedStrokes[0]);
            }

            if ((boardId != -1) && (isSelectionFromServer == false))
            {
                client.ask_select(boardId);
                inkCanvas.Select(null, null);
            }

            texting.Text = "CHANGED";
        }


        void selectionMoving(object sender, InkCanvasSelectionEditingEventArgs e)
        {

        }

        // -----------------------------------------------------------------------------------------
        // FUNCTIONS CALLED FROM CLIENT

        private void doAdd(BoardElement boardElement) // add board element to ink canvas
        {
            
            if (allBoardElements.ContainsKey(boardElement.id))
            {
                Dispatcher.Invoke(() =>
                {
                    texting.Text = Convert.ToString(boardElement.id);
                }
                );
                doDelete(boardElement.id);
            }

            Dispatcher.Invoke(
                () =>
                {
                    boardElement.AddToCanvas(this, inkCanvas);
                    allBoardElements[boardElement.id] = boardElement;
                    bool firstTime = true;
                    long objectid = objectIDGenerator.GetId(boardElement.getElement(), out firstTime);
                    objectIdToBoardId.Add(objectid, boardElement.id);
                    texting.Text = objectid.ToString();
                });
        }

        private void doDelete(int id) // delete board element from ink canvas
        {
            if (allBoardElements.ContainsKey(id))
            {
                BoardElement boardElement = allBoardElements[id];
                Dispatcher.Invoke(
                () =>
                {
                    boardElement.DeleteFromCanvas(this, inkCanvas);
                });
                allBoardElements.Remove(id);
            }
        }

     
        void doEraseAll() // clear ink from canvas
        {
            Dispatcher.Invoke(
                () =>
                {
                    foreach(int key in allBoardElements.Keys)
                    {
                        allBoardElements[key].DeleteFromCanvas(this, inkCanvas);
                    }
                    allBoardElements.Clear();
                });
        }
        private void doSelect(int id) 
        {
            Dispatcher.Invoke(
                () =>
                {
                    isSelectionFromServer = true;
                    allBoardElements[id].selectInCanvas(this, inkCanvas);
                    isSelectionFromServer = false;
                });
        }
        private void doDeselect() 
        {
            Dispatcher.Invoke(
                () =>
                {
                    inkCanvas.Select(null, null);
                });
        }


        // -----------------------------------------------------------------------------------------
        // CONSOLE

        [DllImport("Kernel32")] public static extern void AllocConsole();

        [DllImport("Kernel32")] public static extern void FreeConsole();
    }
}
