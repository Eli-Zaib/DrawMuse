using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace DrawMuse
{
    internal class Undo_Redo : IUndoRedo
    {
      
        private Point startPoint;
        private Point endPoint;
        private Line line;

        public Undo_Redo(Point startPoint, Point endPoint, Line line)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.line = line;
        }
        public void Execute(Canvas canvas)
        {
            canvas.Children.Add(line);
        }
        public void Undo(Canvas canvas)
        {
            if(canvas.Children.Contains(line))
            {
                canvas.Children.Remove(line);
            }
          
        }

        public void Redo(Canvas canvas)
        {
            if(!canvas.Children.Contains(line))
            {
                canvas.Children.Add(line);
            }
          
        }
    }
}
