using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace DrawMuse
{
    public enum ShapeType
    {
        Line,
        Rectangle,
        Square,
        Ellipse
    }

    public class ShapeTools
    {
        private Point startPoint;
        private Shape currentShape;
        private Canvas drawingCanvas;
        private SolidColorBrush currentBrush;
        private MainUndoRedoManager undoRedoManager;

        public ShapeType SelectedShape { get; set; }

        public ShapeTools(Canvas canvas, MainUndoRedoManager undoRedoManager)
        {
            drawingCanvas = canvas;
            this.undoRedoManager = undoRedoManager;
            SelectedShape = ShapeType.Line; 
            currentBrush = Brushes.Black;
        }

        public void SetBrush(SolidColorBrush brush)
        {
            currentBrush = brush;
        }

        public void EnableShapeDrawing()
        {
            drawingCanvas.MouseDown += Canvas_MouseDown;
            drawingCanvas.MouseMove += Canvas_MouseMove;
            drawingCanvas.MouseUp += Canvas_MouseUp;
        }

        public void DisableShapeDrawing()
        {
            drawingCanvas.MouseDown -= Canvas_MouseDown;
            drawingCanvas.MouseMove -= Canvas_MouseMove;
            drawingCanvas.MouseUp -= Canvas_MouseUp;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                startPoint = e.GetPosition(drawingCanvas);

                switch (SelectedShape)
                {
                    case ShapeType.Line:
                        currentShape = new Line
                        {
                            Stroke = currentBrush,
                            StrokeThickness = 2,
                            X1 = startPoint.X,
                            Y1 = startPoint.Y
                        };
                        break;
                    case ShapeType.Rectangle:
                    case ShapeType.Square:
                        currentShape = new Rectangle
                        {
                            Stroke = currentBrush,
                            StrokeThickness = 2,
                            Fill = Brushes.Transparent
                        };
                        break;
                    case ShapeType.Ellipse:
                        currentShape = new Ellipse
                        {
                            Stroke = currentBrush,
                            StrokeThickness = 2,
                            Fill = Brushes.Transparent
                        };
                        break;
                }

                if (currentShape != null)
                {
                    drawingCanvas.Children.Add(currentShape);
                }
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && currentShape != null)
            {
                Point currentPosition = e.GetPosition(drawingCanvas);

                switch (SelectedShape)
                {
                    case ShapeType.Line:
                        var line = currentShape as Line;
                        if (line != null)
                        {
                            line.X2 = currentPosition.X;
                            line.Y2 = currentPosition.Y;
                        }
                        break;
                    case ShapeType.Rectangle:
                    case ShapeType.Square:
                        var rect = currentShape as Rectangle;
                        if (rect != null)
                        {
                            double width = Math.Abs(currentPosition.X - startPoint.X);
                            double height = Math.Abs(currentPosition.Y - startPoint.Y);

                            if (SelectedShape == ShapeType.Square)
                            {
                                width = height = Math.Min(width, height);
                            }

                            rect.Width = width;
                            rect.Height = height;

                            Canvas.SetLeft(rect, Math.Min(currentPosition.X, startPoint.X));
                            Canvas.SetTop(rect, Math.Min(currentPosition.Y, startPoint.Y));
                        }
                        break;
                    case ShapeType.Ellipse:
                        var ellipse = currentShape as Ellipse;
                        if (ellipse != null)
                        {
                            double width = Math.Abs(currentPosition.X - startPoint.X);
                            double height = Math.Abs(currentPosition.Y - startPoint.Y);

                            ellipse.Width = width;
                            ellipse.Height = height;

                            Canvas.SetLeft(ellipse, Math.Min(currentPosition.X, startPoint.X));
                            Canvas.SetTop(ellipse, Math.Min(currentPosition.Y, startPoint.Y));
                        }
                        break;
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (currentShape != null)
            {
                var shapeAction = new ShapeAction(currentShape, drawingCanvas);
                undoRedoManager.Do(shapeAction);

                currentShape = null;
            }
        }
    }


}
