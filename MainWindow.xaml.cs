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


namespace DrawMuse
{

    public partial class MainWindow : Window
    {
        // <-- Drawing pencil logic Starts! here -->

        private IDrawingTools drawingTools;
        private bool isDrawing;

        // <-- Drawing pencil logic Ends! here -->

        private IColorManager colorManager;

        public MainWindow()
        {
            InitializeComponent();

            drawingTools = new DrawingTools(drawingCanvas); // <<< Pencil Logic

            colorManager = new ColorManager();  // <<< ColorManager Logic
            colorManager.CreateColorPalette(ColorPalette); // <<< ColorManager Logic

        }

        private void DrawingCanvas_MouseLeftButtonDown(object sender , MouseButtonEventArgs e)   // <<< ColorManager Logic Starts! here
        {
            colorManager.OnCanvasClicked(sender , e, drawingCanvas);
        }
        // <<< ColorManager Logic Ends! here



        private void DrawButton_Click(object sender, RoutedEventArgs e)     // <-- Drawing pencil logic Starts! here -->
        {
            isDrawing = !isDrawing;


            if (isDrawing)
            {
                drawButton.Background = Brushes.LightGreen;
                drawingTools.Pencil();

            }
            else
            {
                drawButton.Background = Brushes.Transparent;
                drawingTools.RemovePencil();
            }


        }       // <-- Drawing pencil logic Ends! here -->

        public void UndoButton_Click(object sender , RoutedEventArgs e)
        {
            drawingTools.Undo();
          
        }

        public void RedoButton_Click(object sender , RoutedEventArgs e)
        {
            drawingTools.Redo();
        }
    }


}
