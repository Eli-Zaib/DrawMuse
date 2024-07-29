using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DrawMuse
{
    internal class ColorManager : IColorManager
    {
        private SolidColorBrush currentBrush;
        public void CreateColorPalette(Panel palletPanel)
        {
            Color[] colors = new Color[]
            {
               Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Orange, Colors.Purple,
                Colors.Brown, Colors.Black, Colors.White, Colors.Gray, Colors.Pink, Colors.Cyan
            };

            foreach (var color in colors)
            {
                Rectangle rectangle = new Rectangle
                {
                    Width = 20,
                    Height = 20,
                    Fill = new SolidColorBrush(color),
                    Margin = new Thickness(5)
                };
                rectangle.MouseLeftButtonDown += OnColorSelected;
                palletPanel.Children.Add(rectangle);
            }
        }
        public void OnColorSelected(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            if(rectangle != null)
            {
                currentBrush = rectangle.Fill as SolidColorBrush;
            }
        }

        public void OnCanvasClicked(object sender, System.Windows.Input.MouseButtonEventArgs e, Canvas drawingCanvas)
        {
            if(currentBrush != null)
            {
                Ellipse ellipse = new Ellipse
                { 
                   Width = 10,
                   Height = 10,
                   Fill = currentBrush
                
                };
                Canvas.SetLeft(ellipse, e.GetPosition(drawingCanvas).X);
                Canvas.SetTop(ellipse , e.GetPosition(drawingCanvas).Y);
                drawingCanvas.Children.Add(ellipse);
            }
        }
        public void ColorPicker()
        {

        }

        public void ColorBucket()
        {

        }
    }
}
