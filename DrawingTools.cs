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
        private List<Line> currentStroke;
        private double currentSize = 2;

        public DrawingTools(Canvas canvas, MainUndoRedoManager undoRedoManager)
        {
            drawingCanvas = canvas;
            this.undoRedoManager = undoRedoManager;
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
            isDrawing = true;
            previousPoint = e.GetPosition(drawingCanvas);
            currentStroke = new List<Line>();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(drawingCanvas);

                // Boundary checks
                if (currentPoint.X < 0 || currentPoint.X > drawingCanvas.ActualWidth ||
                    currentPoint.Y < 0 || currentPoint.Y > drawingCanvas.ActualHeight)
                {
                    return; // Skip drawing outside canvas
                }

                if (previousPoint != currentPoint)
                {
                    Line line = new Line
                    {
                        Stroke = currentBrush,
                        StrokeThickness = currentSize,
                        X1 = previousPoint.X,
                        Y1 = previousPoint.Y,
                        X2 = currentPoint.X,
                        Y2 = currentPoint.Y,
                        StrokeStartLineCap = PenLineCap.Round,
                        StrokeEndLineCap = PenLineCap.Round
                    };

                    drawingCanvas.Children.Add(line);
                    currentStroke.Add(line);
                    previousPoint = currentPoint;
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrawing)
            {
                isDrawing = false;

                if (currentStroke.Count > 0)
                {
                    IHistoryAction drawAction = new DrawAction(currentStroke, drawingCanvas);
                    undoRedoManager.Do(drawAction);
                }
            }
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
