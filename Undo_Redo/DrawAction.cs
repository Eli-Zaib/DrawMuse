using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace DrawMuse
{
    public class DrawAction : IHistoryAction
    {
        private List<Line> lines;
        private Canvas canvas;

        public DrawAction(List<Line> lines, Canvas canvas)
        {
            this.lines = lines;
            this.canvas = canvas;
        }

        public void Undo(Canvas canvas)
        {
            foreach (var line in lines)
            {
                if (canvas.Children.Contains(line))
                {
                    canvas.Children.Remove(line);
                }
            }
        }

        public void Redo(Canvas canvas)
        {
            foreach (var line in lines)
            {
                if (!canvas.Children.Contains(line))
                {
                    canvas.Children.Add(line);
                }
            }
        }
    }
}
