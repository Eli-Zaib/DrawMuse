using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DrawMuse
{
    public class BucketAction : IHistoryAction
    {
        private int x;
        private int y;
        private Color targetColor;
        private Color replacementColor;
        private byte[] pixelBuffer;
        private WriteableBitmap bitmap;
        private byte[] originalPixelBuffer;

        public BucketAction(int x, int y, Color targetColor, Color replacementColor, WriteableBitmap bitmap, byte[] pixelBuffer)
        {
            this.x = x;
            this.y = y;
            this.targetColor = targetColor;
            this.replacementColor = replacementColor;
            this.bitmap = bitmap;
            this.pixelBuffer = pixelBuffer;
            this.originalPixelBuffer = (byte[])pixelBuffer.Clone(); // Store the original state
        }

        public void Redo(Canvas canvas)
        {
            FloodFill();
            UpdateBitmap();
        }

        public void Undo(Canvas canvas)
        {
            // Restore the original state
            Array.Copy(originalPixelBuffer, pixelBuffer, originalPixelBuffer.Length);
            UpdateBitmap();
        }

        private void FloodFill()
        {
            if (targetColor == replacementColor)
                return;

            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;

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

                Color currentPixelColor = GetColorAtPoint(px, py);

                if (currentPixelColor != targetColor)
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

        private Color GetColorAtPoint(int x, int y)
        {
            if (x < 0 || x >= bitmap.PixelWidth || y < 0 || y >= bitmap.PixelHeight)
                return Colors.Transparent;

            int pixelIndex = (y * bitmap.PixelWidth + x) * 4;

            byte b = pixelBuffer[pixelIndex];
            byte g = pixelBuffer[pixelIndex + 1];
            byte r = pixelBuffer[pixelIndex + 2];
            byte a = pixelBuffer[pixelIndex + 3];

            return Color.FromArgb(a, r, g, b);
        }

        private void UpdateBitmap()
        {
            bitmap.WritePixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), pixelBuffer, bitmap.PixelWidth * 4, 0);
        }
    }
}
