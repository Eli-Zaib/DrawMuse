using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DrawMuse
{
    public class DrawingTools : IDrawingTools
    {
        private Stack<IUndoRedo> undoStack = new Stack<IUndoRedo>();
        private Stack<IUndoRedo> redoStack = new Stack<IUndoRedo>();

        private bool isDrawing;
        private Point previousPoint;
        private Canvas drawingCanvas;
        private SolidColorBrush currentBrush = Brushes.Black;
        private List<Line> currentStroke;
        public DrawingTools(Canvas canvas)
        {
            drawingCanvas = canvas;
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

            currentStroke  = new List<Line>();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(drawingCanvas);

             
                if (previousPoint != currentPoint)
                {
                    Line line = new Line
                    {
                        Stroke = currentBrush,
                        StrokeThickness = 2,
                        X1 = previousPoint.X,
                        Y1 = previousPoint.Y,
                        X2 = currentPoint.X,
                        Y2 = currentPoint.Y
                    };
                    
                    drawingCanvas.Children.Add(line);
                    currentStroke.Add(line);
                    previousPoint = currentPoint;

                    
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(isDrawing)
            {
                isDrawing = false;

                if(currentStroke.Count > 0)
                {
                    undoStack.Push(new Undo_Redo(currentStroke));
                    redoStack.Clear();
                }
            }
          
        }

        public void SetBrush(SolidColorBrush brush)
        {
            currentBrush = brush;
        }
        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                IUndoRedo lastAction = undoStack.Pop();
                lastAction.Undo(drawingCanvas);
                redoStack.Push(lastAction);
            }
        }

        public void Redo()
        {
            if (redoStack.Count > 0)
            {
                IUndoRedo lastAction = redoStack.Pop();
                lastAction.Redo(drawingCanvas);
                undoStack.Push(lastAction);
            }
        }

     
        public void Brush()
        {
            // Implement Brush functionality
        }

        public void DifferentBrushes()
        {
            // Implement DifferentBrushes functionality
        }
    }
}
