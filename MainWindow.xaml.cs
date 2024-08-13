using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DrawMuse
{
    public enum ToolMode
    {
        None,
        Pencil,
        Eraser,
        ColorBucket,
        EyeDropperColorPicker
    }
    public partial class MainWindow : Window
    {
        private IDrawingTools drawingTools;
        private IColorManager colorManager;
        private ColorTools colorTools;
        private ColorBucket colorBucket;
        private EraserTool eraserTool;
        private MainUndoRedoManager mainUndoRedoManager;
        private ToolMode currentToolMode = ToolMode.None;
        private bool isColorBucket;
        private bool isEyeDropper;
        private bool isEraserActive;
        public MainWindow()
        {
            InitializeComponent();

            mainUndoRedoManager = new MainUndoRedoManager(drawingCanvas);
            drawingTools = new DrawingTools(drawingCanvas , mainUndoRedoManager);
            colorManager = new ColorManager(drawingTools , drawingCanvas);
            colorBucket = new ColorBucket(drawingCanvas, mainUndoRedoManager);
            colorManager.CreateColorPalette(ColorPalette);
            colorTools = new ColorTools(drawingTools , EyeDropper);
            eraserTool = new EraserTool(drawingCanvas, mainUndoRedoManager);
            colorTools.ColorSelected += OnColorSelected;

        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton checkedButton)
            {
                ToolMode newMode = (ToolMode)checkedButton.Tag;

                foreach (var child in ((Grid)this.Content).Children)
                {
                    if (child is ToggleButton button && button != checkedButton)
                    {
                        button.IsChecked = false;
                    }
                }

                currentToolMode = newMode;

                HandleToolModeChange(currentToolMode);
            }
        }
        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton uncheckedButton && (ToolMode)uncheckedButton.Tag == currentToolMode)
            {
                currentToolMode = ToolMode.None;
                HandleToolModeChange(currentToolMode);
            }
        }
        private void HandleToolModeChange(ToolMode toolMode)
        {
            switch (toolMode)
            {
                    case ToolMode.Pencil:
                    drawingTools.Pencil();
                    break;

                    case ToolMode.Eraser:
                    isEraserActive = true;
                    break;

                    case ToolMode.ColorBucket:
                    isColorBucket = true;
                    colorBucket.ActivateBucket();
                    break;

                    case ToolMode.EyeDropperColorPicker:
                    colorTools.UseEyeDropper(drawingCanvas);
                    break;

                    case ToolMode.None:
                    drawingTools.RemovePencil();
                    colorTools.DisableEyeDropper(drawingCanvas);
                    isEraserActive = false;
                    isColorBucket = false;
                    break;
            }

        }
        private void Canvas_MouseMove(object sender , MouseEventArgs e)
        {
            if(isEraserActive)
            {
                EraserButton.Background = Brushes.LightBlue;
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point position = e.GetPosition(drawingCanvas);

                    double eraserSize = 20;
                    eraserTool.Erase(position, eraserSize);


                }
            }
            else
            {
                EraserButton.Background = Brushes.Transparent;
            }
        }
        private void SetCanvasSize_Click(object sender, RoutedEventArgs e)
        {
            CanvasSizeWindow sizeWindow = new CanvasSizeWindow(mainUndoRedoManager);
            sizeWindow.Owner = this;

            if (sizeWindow.ShowDialog() == true)
            {
                int newWidth = sizeWindow.CanvasWidth;
                int newHeight = sizeWindow.CanvasHeight;

                int previousWidth = (int)drawingCanvas.Width;
                int previousHeight = (int)drawingCanvas.Height;

                drawingCanvas.Width = newWidth;
                drawingCanvas.Height = newHeight;

                var action = new CanvasSizeChangeAction(previousWidth, previousHeight, newWidth, newHeight);
                mainUndoRedoManager.Do(action);
            }
        }
        private void OnColorSelected(Color color)
        {
            colorBucket.UpdateBucketColor(color);
            drawingTools.SetBrush(new SolidColorBrush(color));
        }
        private void DrawingCanvas_MouseLeftButtonDown(object sender , MouseButtonEventArgs e)  
        {
            if(isColorBucket)
            {
                var point = e.GetPosition(drawingCanvas);
                colorBucket.FillArea(point);
            }
        }
        public void UndoButton_Click(object sender , RoutedEventArgs e)
        {
            mainUndoRedoManager.Undo();
        }
        public void RedoButton_Click(object sender , RoutedEventArgs e)
        {
            mainUndoRedoManager.Redo();
        }
        public void ColorPickerButton_Click(object sender , RoutedEventArgs e)
        {
            ColorPickerControl.IsOpen = true;
        }
        public void ColorPickerControl_SelectedColorChanged(object sender , RoutedPropertyChangedEventArgs<Color?> e)
        {
            if(e.NewValue.HasValue)
            {
                Color selectedColor = e.NewValue.Value;
                SolidColorBrush brush = new SolidColorBrush(e.NewValue.Value);
                drawingTools.SetBrush(brush);

                colorBucket.UpdateBucketColor(selectedColor);
            }
        }
    }
}
