using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace DrawMuse
{
    public class DrawAction : IHistoryAction
    {
        private List<Polyline> polylines;
        private Canvas canvas;

        public DrawAction(List<Polyline> polylines, Canvas canvas)
        {
            this.polylines = polylines;
            this.canvas = canvas;
        }

        public void Undo(Canvas canvas)
        {
            foreach (var polyline in polylines)
            {
                if (canvas.Children.Contains(polyline))
                {
                    canvas.Children.Remove(polyline);
                }
            }
        }

        public void Redo(Canvas canvas)
        {
            foreach (var polyline in polylines)
            {
                if (!canvas.Children.Contains(polyline))
                {
                    canvas.Children.Add(polyline);
                }
            }
        }
    }
}
