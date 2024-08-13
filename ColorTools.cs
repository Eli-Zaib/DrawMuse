using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace DrawMuse
{
    internal class ColorTools : IColorTools
    {
        private IDrawingTools drawingTools;
        public bool isEyeDropperActive = false;
        private ToggleButton eyeDropperButton;
        public Action<Color> ColorSelected { get; set; }



        public ColorTools(IDrawingTools drawingTools, ToggleButton eyedropperButton)
        {
            this.drawingTools = drawingTools;
            this.eyeDropperButton = eyedropperButton;
        }


        public void UseEyeDropper(Canvas canvas)
        {
            if (!isEyeDropperActive)
            {
                canvas.MouseLeftButtonDown += Canvas_MouseLeftButtonDown;
                isEyeDropperActive = true;
            }

        }

        public void DisableEyeDropper(Canvas canvas)
        {
            if (isEyeDropperActive)
            {
                canvas.MouseLeftButtonDown -= Canvas_MouseLeftButtonDown;
                isEyeDropperActive = false;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    eyeDropperButton.Background = Brushes.Transparent;
                });
            }

        }


        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Canvas canvas = sender as Canvas;
            if (canvas != null)
            {
                Point clickPosition = e.GetPosition(canvas);
                var bitmap = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
                bitmap.Render(canvas);

                int x = (int)clickPosition.X;
                int y = (int)clickPosition.Y;

                var croppedBitmap = new CroppedBitmap(bitmap, new Int32Rect(x, y, 1, 1));
                var pixels = new byte[4];
                croppedBitmap.CopyPixels(pixels, 4, 0);

                Color selectedColor = Color.FromArgb(pixels[3], pixels[2], pixels[1], pixels[0]);
            
                ColorSelected?.Invoke(selectedColor);
            

                DisableEyeDropper(canvas);
              

            }
        }



    }
}
