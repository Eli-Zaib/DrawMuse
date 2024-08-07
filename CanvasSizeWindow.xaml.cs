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

namespace DrawMuse
{
    public partial class CanvasSizeWindow : Window
    {
        public int CanvasWidth { get; private set; }
        public int CanvasHeight { get; private set; }
        public CanvasSizeWindow()
        {
            InitializeComponent();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(WidthTextBox.Text, out int width) && int.TryParse(HeightTextBox.Text, out int height))
            {
                MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
                if (mainWindow != null)
                {

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
    public class CanvasSizeChangeAction
    {
        public int PreviousWidth { get; }
        public int PreviousHeight { get; }
        public int NewWidth { get; }
        public int NewHeight { get; }

        public CanvasSizeChangeAction(int previousWidth, int previousHeight, int newWidth, int newHeight)
        {
            PreviousWidth = previousWidth;
            PreviousHeight = previousHeight;
            NewWidth = newWidth;
            NewHeight = newHeight;
        }
    }

    public class UndoRedoManager
    {
        private Stack<CanvasSizeChangeAction> undoStack = new Stack<CanvasSizeChangeAction>();
        private Stack<CanvasSizeChangeAction> redoStack = new Stack<CanvasSizeChangeAction>();

        public void Do(CanvasSizeChangeAction action)
        {
            undoStack.Push(action);
            redoStack.Clear(); // Clear redo stack whenever a new action is performed
        }

        public CanvasSizeChangeAction Undo()
        {
            if (undoStack.Count > 0)
            {
                var action = undoStack.Pop();
                redoStack.Push(action);
                return action;
            }
            return null;
        }

        public CanvasSizeChangeAction Redo()
        {
            if (redoStack.Count > 0)
            {
                var action = redoStack.Pop();
                undoStack.Push(action);
                return action;
            }
            return null;
        }

        public bool CanUndo => undoStack.Count > 0;
        public bool CanRedo => redoStack.Count > 0;
    }
}
