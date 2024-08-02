using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DrawMuse
{
    internal class ColorManager : IColorManager
    {
        private SolidColorBrush currentBrush;
        private IDrawingTools drawingTools;
        private Color currentColor;
        private Canvas drawingCanvas;
        private WriteableBitmap bitmap;
        private byte[] pixelBuffer;
        private bool isBucketToolActive;
        public ColorManager(IDrawingTools drawingTools, Canvas canvas)
        {
            this.drawingTools = drawingTools;
            drawingCanvas = canvas;


            int width = (int)canvas.Width;
            int height = (int)canvas.Height;

            bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            pixelBuffer = new byte[width * height * 4];

            Image image = new Image { Source = bitmap };
            drawingCanvas.Children.Add(image);

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

                if(currentBrush != null)
                {
                    SelectedColor(currentBrush.Color);
                }
            }
        }

        public void SelectedColor(Color color)
        {
            currentColor = color;
            currentBrush = new SolidColorBrush(color);
            drawingTools.SetBrush(currentBrush);
        }
      
        public void UpdateBucketColor(Color color)
        {
            currentColor = color;
            currentBrush = new SolidColorBrush(color);
            drawingTools.SetBrush(currentBrush);
        }

        public void ActivateBucket()
        {
            isBucketToolActive = true;
        }
        public void FillArea(Point point)
        {
            if (currentBrush == null)
            {
                MessageBox.Show("Select a color first!");
                return;
            }

            int x = (int)point.X;
            int y = (int)point.Y;

            var targetColor = GetColorAtPoint(x, y);

            if (targetColor.HasValue)
            {
                FloodFill(x, y, targetColor.Value, currentBrush.Color);
                UpdateBitmap(); // Update the bitmap with the pixel buffer
            }
        }

        private Color? GetColorAtPoint(int x, int y)
        {
            if (x < 0 || x >= bitmap.PixelWidth || y < 0 || y >= bitmap.PixelHeight)
                return null;

            int pixelIndex = (y * bitmap.PixelWidth + x) * 4;
            byte b = pixelBuffer[pixelIndex];
            byte g = pixelBuffer[pixelIndex + 1];
            byte r = pixelBuffer[pixelIndex + 2];
            byte a = pixelBuffer[pixelIndex + 3];

            return Color.FromArgb(a, r, g, b);
        }

        private void FloodFill(int x, int y, Color targetColor, Color replacementColor)
        {
            if (targetColor == replacementColor)
                return;

            int width = (int)drawingCanvas.Width;
            int height = (int)drawingCanvas.Height;

            bool[,] visited = new bool[width, height];
            Stack<Point> stack = new Stack<Point>();
            stack.Push(new Point(x, y));

            byte[] replacementPixelData = new byte[] { replacementColor.B, replacementColor.G, replacementColor.R, replacementColor.A };

            while (stack.Count > 0)
            {
                Point p = stack.Pop();
                int px = (int)p.X;
                int py = (int)p.Y;

                if (px < 0 || px >= width || py < 0 || py >= height)
                    continue;

                if (visited[px, py])
                    continue;

                Color currentColor = GetColorAtPoint(px, py).Value;

                if (currentColor != targetColor)
                    continue;

                // Update the pixel in the buffer
                int pixelIndex = (py * width + px) * 4;
                pixelBuffer[pixelIndex] = replacementPixelData[0];
                pixelBuffer[pixelIndex + 1] = replacementPixelData[1];
                pixelBuffer[pixelIndex + 2] = replacementPixelData[2];
                pixelBuffer[pixelIndex + 3] = replacementPixelData[3];

                visited[px, py] = true;

                // Push neighboring pixels to the stack
                if (px > 0) stack.Push(new Point(px - 1, py));
                if (px < width - 1) stack.Push(new Point(px + 1, py));
                if (py > 0) stack.Push(new Point(px, py - 1));
                if (py < height - 1) stack.Push(new Point(px, py + 1));
            }
        }

        private void UpdateBitmap()
        {
            bitmap.WritePixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), pixelBuffer, bitmap.PixelWidth * 4, 0);
        }
    }
}
