using System;
using System.Windows;
using System.Windows.Controls;

namespace DrawMuse
{
    public partial class CanvasSizeWindow : Window
    {
        public int CanvasWidth { get; private set; }
        public int CanvasHeight { get; private set; }
        private MainUndoRedoManager undoRedoManager;

        public CanvasSizeWindow(MainUndoRedoManager undoRedoManager)
        {
            InitializeComponent();
            this.undoRedoManager = undoRedoManager;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(WidthTextBox.Text, out int width) && int.TryParse(HeightTextBox.Text, out int height))
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {
                    var previousWidth = (int)mainWindow.drawingCanvas.Width;
                    var previousHeight = (int)mainWindow.drawingCanvas.Height;

                    var sizeChangeAction = new CanvasSizeChangeAction(previousWidth, previousHeight, width, height);

                    undoRedoManager.Do(sizeChangeAction);

                    mainWindow.drawingCanvas.Width = width;
                    mainWindow.drawingCanvas.Height = height;
                }

                CanvasWidth = width;
                CanvasHeight = height;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Please enter valid numbers for width and height.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
