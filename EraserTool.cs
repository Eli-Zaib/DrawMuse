using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace DrawMuse
{
    internal class EraserTool
    {
        private Canvas drawingCanvas;
        private List<UIElement> erasedElements;
        private MainUndoRedoManager undoRedoManager;

        public EraserTool(Canvas canvas, MainUndoRedoManager undoRedoManager)
        {
            drawingCanvas = canvas;
            erasedElements = new List<UIElement>();
            this.undoRedoManager = undoRedoManager;
        }

        public void Erase(Point position, double size)
        {
            var toRemove = new List<UIElement>();

            foreach (UIElement element in drawingCanvas.Children)
            {
                if (element is Shape shape)
                {
                    if (IsPointInShape(position, shape))
                    {
                        toRemove.Add(shape);
                    }
                }
            }

            if (toRemove.Count > 0)
            {
                erasedElements.AddRange(toRemove);
                undoRedoManager.Do(new EraseAction(toRemove, drawingCanvas));

                foreach (var item in toRemove)
                {
                    drawingCanvas.Children.Remove(item);
                }
            }
        }

        private bool IsPointInShape(Point position, Shape shape)
        {
            if (shape is Line line)
            {
                const double threshold = 5.0;
                double distance = DistanceFromPointToLine(position, line);
                return distance <= threshold;
            }

            var shapeBounds = shape.RenderedGeometry.Bounds;
            return shapeBounds.Contains(position);
        }

        private double DistanceFromPointToLine(Point point, Line line)
        {
            double a = point.X - line.X1;
            double b = point.Y - line.Y1;
            double c = line.X2 - line.X1;
            double d = line.Y2 - line.Y1;

            double dot = a * c + b * d;
            double len_sq = c * c + d * d;
            double param = (len_sq != 0) ? dot / len_sq : -1;

            double xx, yy;

            if (param < 0)
            {
                xx = line.X1;
                yy = line.Y1;
            }
            else if (param > 1)
            {
                xx = line.X2;
                yy = line.Y2;
            }
            else
            {
                xx = line.X1 + param * c;
                yy = line.Y1 + param * d;
            }

            double dx = point.X - xx;
            double dy = point.Y - yy;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
