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
      

        private IDrawingTools drawingTools;
        private bool isDrawing;
        private IColorManager colorManager;

        public MainWindow()
        {
            InitializeComponent();

            drawingTools = new DrawingTools(drawingCanvas);
            colorManager = new ColorManager(drawingTools); 
            colorManager.CreateColorPalette(ColorPalette);

        }

        private void DrawingCanvas_MouseLeftButtonDown(object sender , MouseButtonEventArgs e)   // <<< ColorManager Logic Starts! here
        {
           
        }
        // <<< ColorManager Logic Ends! here



        private void DrawButton_Click(object sender, RoutedEventArgs e)     // <-- Drawing pencil logic Starts! here -->
        {
            isDrawing = !isDrawing;


            if (isDrawing)
            {
             
                drawingTools.Pencil();
                drawButton.Background = Brushes.LightGreen;

            }
            else
            {
               
                drawingTools.RemovePencil();
                drawButton.Background = Brushes.Transparent;
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

    
        public void ColorPickerControl_SelectedColorChanged(object sender , RoutedPropertyChangedEventArgs<Color?> e)
        {
            if(e.NewValue.HasValue)
            {
                SolidColorBrush brush = new SolidColorBrush(e.NewValue.Value);
                drawingTools.SetBrush(brush);
            }
        }
    }


}
