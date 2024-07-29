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
        private Stack<IUndoRedo> undoStack = new Stack<IUndoRedo>();
        private Stack<IUndoRedo> redoStack = new Stack<IUndoRedo>();

        private bool isDrawing = false;
        private Point previousPoint;
        private Canvas drawingCanvas;

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
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(drawingCanvas);

                // Create a new line only if it is not the first move
                if (previousPoint != currentPoint)
                {
                    Line line = new Line
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 2,
                        X1 = previousPoint.X,
                        Y1 = previousPoint.Y,
                        X2 = currentPoint.X,
                        Y2 = currentPoint.Y
                    };

                    // Create an action for the undo/redo functionality
                    Undo_Redo action = new Undo_Redo(previousPoint, currentPoint, line);

                    // Execute the action (draw the line)
                    action.Execute(drawingCanvas);

                    // Push the action to the undo stack
                    undoStack.Push(action);
                    redoStack.Clear(); // Clear the redo stack

                    // Update the previous point to the current point
                    previousPoint = currentPoint;

                    
                }
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
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
