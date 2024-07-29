using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace DrawMuse
{
    internal class ShapesTools : IShapesTools
    {
        public void Line(Canvas canvas, Point startPoint, Point endPoint)
        {
            var line = new Line
            { 
                 X1 = startPoint.X,
                 Y1 = startPoint.Y,
                 X2 = endPoint.X,
                 Y2 = endPoint.Y,
                 Stroke = Brushes.Black,
                 StrokeThickness = 2
            
            };
            canvas.Children.Add(line);
        }

        public void Cube()
        {

        }

        public void Cylinder()
        {

        }

        public void Circle()
        {

        }

        public void Rectangle()
        {

        }

    }
}
