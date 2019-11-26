using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

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
        bool isSelectionFromServer = false;
        int selectedObject = -1;

        Client client;

        ObjectIDGenerator objectIDGenerator = new ObjectIDGenerator(); // local ids
        Dictionary<int, BoardElement> allBoardElements = new Dictionary<int, BoardElement>(); // server ids
        Dictionary<long, int> objectIdToBoardId = new Dictionary<long, int>(); // local id to server id
        /// <summary>
        /// return server id given object
        /// </summary>
        /// <param name="obj">object attached to inkcanvas</param>
        /// <returns></returns>
        public int getBoardIdFromObject(Object obj)
        {
            bool firstTime = false;
            long objectId = objectIDGenerator.GetId(obj, out firstTime);
            return objectIdToBoardId[objectId];
        }

        public MainWindow()
        {
            client = new Client("127.0.0.1", this);
            

            InitializeComponent();
            inkCanvas.AddHandler(InkCanvas.MouseDownEvent, new MouseButtonEventHandler(clickCanvas), true);

            for (int i = 0; i < availableColors.Count; i++) // create color combo box
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
            texting.Text = "Initial text";

            this.clickRestart();
        }

        // -----------------------------------------------------------------------------------------
        // LOCAL CHANGES

        /// <summary>
        /// Switch between pen and eraser if combobox selection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void selectedPenStyle(object sender, System.EventArgs e)
        {
            changeMode("ink");
            if (penStyleBox.SelectedIndex == 0)
            {
                inkCanvas.DefaultDrawingAttributes.Color = availableColors[colorBox.SelectedIndex];
            }
            else if (penStyleBox.SelectedIndex == 1)
            {
                inkCanvas.DefaultDrawingAttributes.Color = Color.FromRgb(255, 255, 255);
            }
        }

        /// <summary>
        /// Change color if combobox selection changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void selectedColor(object sender, System.EventArgs e) // change pen color
        {
            if (penStyleBox.SelectedIndex == 0)
            {
                inkCanvas.DefaultDrawingAttributes.Color = availableColors[colorBox.SelectedIndex];
            }
        }

        /// <summary>
        /// Change stylus width if slider value changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void selectedStylusWidth(object sender, System.EventArgs e) // change width of stylus
        {
            inkCanvas.DefaultDrawingAttributes.Width = widthSlider.Value;
            inkCanvas.DefaultDrawingAttributes.Height = widthSlider.Value;
        }

        /// <summary>
        /// handle all operations related to changing mode between ink, text, selection etc...
        /// </summary>
        /// <param name="newMode"></param>
        void changeMode(string newMode)
        {
            SolidColorBrush selectedButtonColor = new SolidColorBrush(Color.FromRgb(100, 100, 100));

            if (currentMode == "text") // put last mode button in correct color
            {
                textButton.ClearValue(Button.BackgroundProperty);
            }
            else if (currentMode == "select")
            {
                selectButton.ClearValue(Button.BackgroundProperty);
            }
            else if (currentMode == "ink")
            {
                penStyleBox.ClearValue(ComboBox.BackgroundProperty);
            }

            if (newMode == currentMode) // if a mode is selected two times in a row, go back to ink (deselect button)
            {
                newMode = "ink";
            }

            if (newMode == "ink") // change inkcanvas editing mode and button color
            {
                inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
                penStyleBox.Background = selectedButtonColor;
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

            currentMode = newMode;
        }

        /// <summary>
        /// Change to text mode if click on text button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clickTextButton(object sender, System.EventArgs e)
        {
            changeMode("text");
        }

        /// <summary>
        /// Change to select mode if click on select button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clickSelectButton(object sender, System.EventArgs e)
        {
            changeMode("select");
        }

        DateTime lastClick = DateTime.Now;
        /// <summary>
        /// Handle everything related to a click on the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clickCanvas(object sender, MouseButtonEventArgs e)
        {
            if(selectedObject != -1) // update selected object if click somewhere else on the canvas
            {
                allBoardElements[selectedObject].updatePosition(inkCanvas);
                client.ask_modif(selectedObject, allBoardElements[selectedObject]);
            }

            if ((currentMode == "text") && ((DateTime.Now - lastClick) > new TimeSpan(0, 0, 1))) //add new text block if text mode
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

                changeMode("select");
            }
        }

        /// <summary>
        /// Handle deletion of selected object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void previewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Delete)
            {
                if (allBoardElements.ContainsKey(selectedObject))
                {
                    client.ask_delete(selectedObject);
                    selectedObject = -1;
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Save canvas in jpg format if save button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clickSaveButton(object sender, System.EventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Filter = "jpg files (*.jpg)|*.jpg";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.ShowDialog();

            try
            {
                myStream = saveFileDialog.OpenFile();
                texting.Text = myStream.ToString();
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)inkCanvas.ActualWidth, (int)inkCanvas.ActualHeight, 96d, 96d, PixelFormats.Default);
                rtb.Render(inkCanvas);
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                encoder.Save(myStream);
                myStream.Close();
            }
            catch (System.InvalidOperationException)
            {
                texting.Text = "no file";
            }
        }

        /// <summary>
        /// Prevent user from resizing selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void selectionResizing(object sender, InkCanvasSelectionEditingEventArgs e)
        {
            e.Cancel = true;
        }

        // -----------------------------------------------------------------------------------------
        // EVENTS SENT TO CLIENT

        /// <summary>
        /// Send new stroke to server when a stroke is collected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void strokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e) // send last stroke collected
        {
            client.ask_add(new StrokeElement(e.Stroke));
            inkCanvas.Strokes.Remove(e.Stroke);
        }

        /// <summary>
        /// Ask server to erase everything when click on erase all button 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clickEraseAllButton(object sender, System.EventArgs e) // send erase all
        {
            client.ask_clear_all();
        }

        /// <summary>
        /// Open connection popup when click on connect button
        /// </summary>
        void clickRestart() // send erase all
        {
            Popup pop = new Popup(this);
            pop.Show();
        }

        /// <summary>
        /// Open connection popup when click on connect button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clickRestart(object sender, System.EventArgs e) // send erase all
        {
            Popup pop = new Popup(this);
            pop.Show();
        }

        /// <summary>
        /// Open info popup when click on info button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clickInfo(object sender, System.EventArgs e) // send erase all
        {
            MessageBoxResult result = MessageBox.Show("IP adress\n"+getIp()+"\nWhiteboard name:\n"+client.m_nomServer+ "\n\nDo you want to copy the name on the clipboard ","Informations", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                Clipboard.SetText(client.m_nomServer);
                MessageBox.Show("Name copied to the cliboard");
            }
            //infos.Text = "Server name" + client.m_nomServer;
            
        }

        /// <summary>
        /// Send new text box to server when text box is modified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void textBoxModified(object sender, RoutedEventArgs e)
        {
            TextBox sourceTextBox = (TextBox)e.Source;
            
            if (isCreatingATextBox)
            {
                client.ask_add(new TextBoxElement(sourceTextBox, InkCanvas.GetLeft(sourceTextBox), InkCanvas.GetTop(sourceTextBox),0));
                inkCanvas.Children.Remove(sourceTextBox);
                isCreatingATextBox = false;
            }
        }

        /// <summary>
        /// Ask server to select object when object is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                /*if (selectedObject != -1)
                {
                    texting.Text = Convert.ToString(selectedObject) + allBoardElements[selectedObject].GetString();
                    client.ask_modif(selectedObject, allBoardElements[selectedObject]);
                } */
                client.ask_select(boardId);
                inkCanvas.Select(null, null);
            }

            //texting.Text = "CHANGED";
        }

        /// <summary>
        /// Prevent user from selecting eraser strokes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void selectionChanging(object sender, InkCanvasSelectionChangingEventArgs e)
        {
            StrokeCollection selectedStrokes = e.GetSelectedStrokes();
            StrokeCollection actuallySelectedStrokes = new StrokeCollection();
            foreach (Stroke stroke in selectedStrokes)
            {
                if (stroke.DrawingAttributes.Color != Color.FromRgb(255, 255, 255))
                {
                    actuallySelectedStrokes.Add(stroke);
                }
            }
            e.SetSelectedStrokes(actuallySelectedStrokes);
        }

        // -----------------------------------------------------------------------------------------
        // FUNCTIONS CALLED FROM CLIENT

        /// <summary>
        /// Add new element to canvas
        /// </summary>
        /// <param name="boardElement">element to add</param>
        public void doAdd(BoardElement boardElement) // add board element to ink canvas
        {
            if (selectedObject != boardElement.id)
            {
                if (allBoardElements.ContainsKey(boardElement.id) && selectedObject != boardElement.id)
                {
                    
                    Dispatcher.Invoke(() =>
                    {
                        texting.Text = "ICI";
                        //texting.Text = Convert.ToString(boardElement.id);
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
                    });
                
            }
        }

        /// <summary>
        /// Delete element from canvas
        /// </summary>
        /// <param name="id">id of element to delete</param>
        public void doDelete(int id) // delete board element from ink canvas
        {
            if (allBoardElements.ContainsKey(id))
            {
                
                Dispatcher.Invoke(
                () =>
                {
                    bool obol;
                    BoardElement boardElement = allBoardElements[id];
                    boardElement.DeleteFromCanvas(this, inkCanvas);
                    objectIdToBoardId.Remove(objectIDGenerator.GetId(allBoardElements[id].getElement(),out obol));
                    allBoardElements.Remove(id);
                });
                
            }
        }

        /// <summary>
        /// Clear all canvas
        /// </summary>
        public void doClearAll() // clear ink from canvas
        {
            Dispatcher.Invoke(
                () =>
                {
                    foreach(int key in allBoardElements.Keys)
                    {
                        allBoardElements[key].DeleteFromCanvas(this, inkCanvas);
                    }
                    allBoardElements.Clear();
                    objectIdToBoardId.Clear();
                    selectedObject = -1;
                });
        }

        /// <summary>
        /// Select element from canvas
        /// </summary>
        /// <param name="id">id of element to select</param>
        public void doSelect(int id) 
        {
            Dispatcher.Invoke(
                () =>
                {
                    selectedObject = id;
                    isSelectionFromServer = true;
                    allBoardElements[id].selectInCanvas(this, inkCanvas);
                    isSelectionFromServer = false;
                });
        }

        /// <summary>
        /// Delesect everything
        /// </summary>
        public void doDeselect() 
        {
            Dispatcher.Invoke(
                () =>
                {
                    selectedObject = -1;
                    inkCanvas.Select(null, null);
                });
        }

        /// <summary>
        /// Show error messagebox
        /// </summary>
        /// <param name="error">error to show</param>
        public void doShowError(String error)
        {
            MessageBox.Show("Error:\n" + error);
        }

        /// <summary>
        /// Switch to new board
        /// </summary>
        /// <param name="newBoard"></param>
        /// <param name="newAleaBoard"></param>
        /// <param name="ip"></param>
        /// <param name="boardName"></param>
        public void doRestart(bool newBoard, bool newAleaBoard, String ip, String boardName)
        {
            client.changeIP(ip);

            if (newBoard)
            {
                client.createBoard(boardName);
            }
            else if (newAleaBoard)
            {
                client.createBoard();
            }
            else
            {
                client.joinBoard(boardName, false);
            }



        }
        public void doRestart(String ip, String boardName)
        {
            
            client.joinBoard(boardName, true);

        }

        void ink_KeyUp(object sender, KeyEventArgs e)
        {
            
            
        }

        

        public void changeIP(String ip)
        {
            client.changeIP(ip);
        }

        public string getIp()
        {
            return client.getIp();
        }
        
    }
}
