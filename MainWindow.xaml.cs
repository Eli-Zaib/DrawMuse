﻿using System;
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
        private IColorManager colorManager;
        private ColorTools colorTools;
        private ColorBucket colorBucket;
        private EraserTool eraserTool;
        private UndoRedoManager undoRedoManager = new UndoRedoManager();
        private bool isColorBucket;
        private bool isDrawing;
        private bool isEyeDropper;
        private bool isEraserActive;
        public MainWindow()
        {
            InitializeComponent();

            drawingTools = new DrawingTools(drawingCanvas);
            colorManager = new ColorManager(drawingTools , drawingCanvas);
            colorBucket = new ColorBucket(drawingCanvas);
            colorManager.CreateColorPalette(ColorPalette);
            colorTools = new ColorTools(drawingTools , EyeDropper);
            eraserTool = new EraserTool(drawingCanvas);
            colorTools.ColorSelected += OnColorSelected;

        }

        private void EraserButton_Click(object sender , RoutedEventArgs e)
        {
            isEraserActive = !isEraserActive;
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
        private void SetCanvasSize_Click(object sender , RoutedEventArgs e)
        {
            CanvasSizeWindow sizeWindow = new CanvasSizeWindow();
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
                undoRedoManager.Do(action);
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

     

        private void EyedropperButton_Click(object sender, RoutedEventArgs e)
        {

            isEyeDropper = !isEyeDropper;
            if (isEyeDropper)
            {

                colorTools.UseEyeDropper(drawingCanvas);
                EyeDropper.Background = Brushes.LightBlue;

            }
            else
            {
                colorTools.DisableEyeDropper(drawingCanvas);
                EyeDropper.Background = Brushes.Transparent;
            }

        }
        private void ColorBucket_Click(object sender , RoutedEventArgs e)
        {
            isColorBucket = !isColorBucket;
            if(isColorBucket)
            {
                colorBucket.ActivateBucket();
                ColorBucket.Background = Brushes.LightBlue;
            }
            else
            {
                ColorBucket.Background = Brushes.Transparent;
            }
        }


        private void DrawButton_Click(object sender, RoutedEventArgs e)    
        {
            isDrawing = !isDrawing;
            if (isDrawing)
            {
                drawingTools.Pencil();
                drawButton.Background = Brushes.LightBlue;
            }
            else
            {
                drawingTools.RemovePencil();
                drawButton.Background = Brushes.Transparent;
            }


        }    

        public void UndoButton_Click(object sender , RoutedEventArgs e)
        {
            if (undoRedoManager.CanUndo)
            {
                var action = undoRedoManager.Undo();
                if (action != null)
                {
                    drawingCanvas.Width = action.PreviousWidth;
                    drawingCanvas.Height = action.PreviousHeight;
                }
            }

            drawingTools.Undo();
            colorBucket.Undo();
            eraserTool.Undo();
        }

        public void RedoButton_Click(object sender , RoutedEventArgs e)
        {
            if (undoRedoManager.CanRedo)
            {
                var action = undoRedoManager.Redo();
                if (action != null)
                {
                    drawingCanvas.Width = action.NewWidth;
                    drawingCanvas.Height = action.NewHeight;
                }
            }

            drawingTools.Redo();
            colorBucket.Redo();
            eraserTool.Redo();

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
