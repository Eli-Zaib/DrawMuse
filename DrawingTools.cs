using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DrawMuse
{
    public class DrawingTools : IDrawingTools
    {
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
                Line line = new Line
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    X1 = previousPoint.X,
                    Y1 = previousPoint.Y,
                    X2 = currentPoint.X,
                    Y2 = currentPoint.Y

                };
                previousPoint = currentPoint;
                drawingCanvas.Children.Add(line);
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDrawing = false;
        }

        public void Brush()
        {

        }

        public void DifferentBrushes()
        {

        }
    }
}
