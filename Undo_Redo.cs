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
        private List<Line> lines;
        public Undo_Redo(List<Line> lines)
        {
            this.lines = lines;   
        }
        public void Execute(Canvas canvas)
        {
            foreach(var line in lines)
            {
                canvas.Children.Add(line);
            }
           
        }
        public void Undo(Canvas canvas)
        {
            foreach(var line in lines)
            {
                canvas.Children.Remove(line);
            }
          
        }

        public void Redo(Canvas canvas)
        {
           Execute(canvas);
          
        }
    }
}
