using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
        Line,
        Square,
        Rectangle,
        Ellipse,
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
        private ScaleTransform canvasScaleTransform= new ScaleTransform();
        private ShapeTools shapeTools;
        private ToolMode currentToolMode = ToolMode.None;
        private bool isColorBucket;
        private bool isEyeDropper;
        private bool isEraserActive;
        private double zoomFactor = 1.1;
        private double minZoom = 0.15;
        private double maxZoom = 5;
        public MainWindow()
        {
            InitializeComponent();

            mainUndoRedoManager = new MainUndoRedoManager(drawingCanvas);
            drawingTools = new DrawingTools(drawingCanvas , mainUndoRedoManager);
            shapeTools = new ShapeTools(drawingCanvas , mainUndoRedoManager);
            colorBucket = new ColorBucket(drawingCanvas, mainUndoRedoManager);
            colorManager = new ColorManager(drawingTools , drawingCanvas , shapeTools , colorBucket);
            colorManager.CreateColorPalette(ColorPalette);
            colorTools = new ColorTools(drawingTools , EyeDropper);
            eraserTool = new EraserTool(drawingCanvas, mainUndoRedoManager);
            drawingCanvas.RenderTransform = canvasScaleTransform;
            colorTools.ColorSelected += OnColorSelected;
            SizeAdjuster.ValueChanged += SizeAdjuster_ValueChanged;

        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (e.Key == Key.Z)
                {
                    mainUndoRedoManager.Undo();
                    e.Handled = true;
                }
                else if (e.Key == Key.Y)
                {
                    mainUndoRedoManager.Redo();
                    e.Handled = true;
                }
            }
        }
        private void DrawCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                e.Handled = true;

                double scale = canvasScaleTransform.ScaleX;
                double oldScale = scale;

                // Zoom in or out
                if (e.Delta > 0 && scale < maxZoom)
                {
                    scale *= zoomFactor;
                }
                else if (e.Delta < 0 && scale > minZoom)
                {
                    scale /= zoomFactor;
                }

                // Enforce the minimum and maximum zoom levels
                if (scale < minZoom)
                {
                    scale = minZoom;
                }
                else if (scale > maxZoom)
                {
                    scale = maxZoom;
                }

                // Apply the new scale
                canvasScaleTransform.ScaleX = scale;
                canvasScaleTransform.ScaleY = scale;

                var position = e.GetPosition(drawingCanvas);

                double deltaX = (position.X - canvasScaleTransform.CenterX) * (scale / oldScale - 1);
                double deltaY = (position.Y - canvasScaleTransform.CenterY) * (scale / oldScale - 1);

                canvasScaleTransform.CenterX = Clamp(canvasScaleTransform.CenterX - deltaX, 0, drawingCanvas.ActualWidth);
                canvasScaleTransform.CenterY = Clamp(canvasScaleTransform.CenterY - deltaY, 0, drawingCanvas.ActualHeight);
            }
        }
        private double Clamp(double value, double min, double max)
        {
            return Math.Max(min, Math.Min(max, value));
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
            ResetActiveTools();

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

                    case ToolMode.Square:
                    shapeTools.EnableShapeDrawing();
                    shapeTools.SelectedShape = ShapeType.Square;
                    break;

                    case ToolMode.Line:
                    shapeTools.EnableShapeDrawing();
                    shapeTools.SelectedShape = ShapeType.Line;
                    break;

                    case ToolMode.Rectangle:
                    shapeTools.EnableShapeDrawing();
                    shapeTools.SelectedShape = ShapeType.Rectangle;
                    break;

                    case ToolMode.Ellipse:
                    shapeTools.EnableShapeDrawing();
                    shapeTools.SelectedShape = ShapeType.Ellipse;
                    break;

                    case ToolMode.None:
                    break;
            }

        }
        private void ResetActiveTools()
        {
            drawingTools.RemovePencil();
            colorTools.DisableEyeDropper(drawingCanvas);
            shapeTools.DisableShapeDrawing();
            isEraserActive = false;
            isColorBucket = false;
        }
        private void SizeAdjuster_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            drawingTools.SetSize(e.NewValue);
            eraserTool.SetSize(e.NewValue);
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
            shapeTools.SetBrush(new SolidColorBrush(color));
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
                shapeTools.SetBrush(brush);
                colorBucket.UpdateBucketColor(selectedColor);
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp",
                Title = "Save your drawing"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                SaveDrawingToFile(saveFileDialog.FileName);
            }
        }
        private void SaveDrawingToFile(string filePath)
        {
            int width = (int)drawingCanvas.ActualWidth;
            int height = (int)drawingCanvas.ActualHeight;

            if (width > 0 && height > 0)
            {
                drawingCanvas.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                drawingCanvas.Arrange(new Rect(0, 0, width, height));

                RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(width, height, 96d, 96d, PixelFormats.Pbgra32);
                renderTargetBitmap.Render(drawingCanvas);

                BitmapEncoder encoder = null;
                if (filePath.EndsWith(".png"))
                {
                    encoder = new PngBitmapEncoder();
                }
                else if (filePath.EndsWith(".jpg"))
                {
                    encoder = new JpegBitmapEncoder();
                }
                else if (filePath.EndsWith(".bmp"))
                {
                    encoder = new BmpBitmapEncoder();
                }

                if (encoder != null)
                {
                    encoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
                    using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                }
            }
            else
            {
                MessageBox.Show("Canvas size is too small to save.");
            }
        }

    }
}
