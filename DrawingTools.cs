using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DrawMuse
{
    public class DrawingTools : IDrawingTools
    {
        private MainUndoRedoManager undoRedoManager;
        private Canvas drawingCanvas;
        private bool isDrawing;
        private Point previousPoint;
        private SolidColorBrush currentBrush = Brushes.Black;
        private Polyline currentStroke;
        private double currentSize = 2;

        public DrawingTools(Canvas canvas, MainUndoRedoManager undoRedoManager)
        {
            drawingCanvas = canvas;
            this.undoRedoManager = undoRedoManager;

            drawingCanvas.MouseEnter += Canvas_MouseEnter;
            drawingCanvas.MouseLeave += Canvas_MouseLeave;
        }

        public void SetSize(double size)
        {
            currentSize = size;
        }

        public void Pencil()
        {
            drawingCanvas.MouseDown += Canvas_MouseDown;
            drawingCanvas.MouseMove += Canvas_MouseMove;
            drawingCanvas.MouseUp += Canvas_MouseUp;
        }

        public void RemovePencil()
        {
            drawingCanvas.MouseDown -= Canvas_MouseDown;
            drawingCanvas.MouseMove -= Canvas_MouseMove;
            drawingCanvas.MouseUp -= Canvas_MouseUp;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                StartDrawing(e.GetPosition(drawingCanvas));
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(drawingCanvas);

                if (previousPoint != currentPoint)
                {
                    currentStroke.Points.Add(currentPoint);
                    previousPoint = currentPoint;
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;

                if (currentStroke.Points.Count > 0)
                {
                    IHistoryAction drawAction = new DrawAction(new List<Polyline> { currentStroke }, drawingCanvas);
                    undoRedoManager.Do(drawAction);
                }
            }
        }

        private void Canvas_MouseLeave(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;
                // Add the drawn Polyline to undo stack
                if (currentStroke != null && currentStroke.Points.Count > 0)
                {
                    IHistoryAction drawAction = new DrawAction(new List<Polyline> { currentStroke }, drawingCanvas);
                    undoRedoManager.Do(drawAction);
                }
            }
        }

        private void Canvas_MouseEnter(object sender, MouseEventArgs e)
        {
            Point currentPoint = Mouse.GetPosition(drawingCanvas);

            if (Mouse.LeftButton == MouseButtonState.Pressed &&
                currentPoint.X >= 0 && currentPoint.X <= drawingCanvas.ActualWidth &&
                currentPoint.Y >= 0 && currentPoint.Y <= drawingCanvas.ActualHeight)
            {
                StartDrawing(currentPoint);
            }
        }

        private void StartDrawing(Point startPoint)
        {
            isDrawing = true;
            previousPoint = startPoint;

            // Initialize the Polyline for the current stroke
            currentStroke = new Polyline
            {
                Stroke = currentBrush,
                StrokeThickness = currentSize
            };
            currentStroke.Points.Add(startPoint);
            drawingCanvas.Children.Add(currentStroke);
        }

        public void SetBrush(SolidColorBrush brush)
        {
            currentBrush = brush;
        }

        public void Undo()
        {
            undoRedoManager.Undo();
        }

        public void Redo()
        {
            undoRedoManager.Redo();
        }
    }
}
