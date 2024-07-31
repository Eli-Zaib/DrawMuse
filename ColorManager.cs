﻿using System;
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
        private IDrawingTools drawingTools;

        public ColorManager(IDrawingTools drawingTools)
        {
            this.drawingTools = drawingTools;
        }
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
                drawingTools.SetBrush(currentBrush);
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
