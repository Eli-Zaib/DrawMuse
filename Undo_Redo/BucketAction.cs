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
        private readonly int x;
        private readonly int y;
        private readonly Color targetColor;
        private readonly Color replacementColor;
        private readonly WriteableBitmap bitmap;
        private readonly byte[] pixelBuffer;
        private readonly byte[] originalPixelBuffer;

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
            if (targetColor == replacementColor)
                return;

            // Perform the flood fill algorithm
            FloodFill();

            // Update the bitmap to reflect the changes
            UpdateBitmap();
        }

        public void Undo(Canvas canvas)
        {
            // Restore the original pixel buffer
            Array.Copy(originalPixelBuffer, pixelBuffer, originalPixelBuffer.Length);

            // Update the bitmap to reflect the reverted changes
            UpdateBitmap();
        }

        private void FloodFill()
        {
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;

            if (x < 0 || x >= width || y < 0 || y >= height)
                return;

            Color initialColor = GetColorAtPoint(x, y);
            if (initialColor == replacementColor || initialColor != targetColor)
                return;

            Queue<Point> queue = new Queue<Point>();
            queue.Enqueue(new Point(x, y));

            byte[] replacementPixelData = {
                replacementColor.B,
                replacementColor.G,
                replacementColor.R,
                replacementColor.A
            };

            while (queue.Count > 0)
            {
                Point p = queue.Dequeue();
                int px = (int)p.X;
                int py = (int)p.Y;

                if (px < 0 || px >= width || py < 0 || py >= height)
                    continue;

                int pixelIndex = (py * width + px) * 4;
                if (pixelBuffer[pixelIndex] == replacementPixelData[0] &&
                    pixelBuffer[pixelIndex + 1] == replacementPixelData[1] &&
                    pixelBuffer[pixelIndex + 2] == replacementPixelData[2] &&
                    pixelBuffer[pixelIndex + 3] == replacementPixelData[3])
                    continue; // Skip if pixel is already set to replacement color

                Color currentPixelColor = GetColorAtPoint(px, py);

                if (currentPixelColor != targetColor)
                    continue;

                // Update the pixel in the buffer
                Array.Copy(replacementPixelData, 0, pixelBuffer, pixelIndex, 4);

                // Enqueue neighboring pixels
                if (px > 0) queue.Enqueue(new Point(px - 1, py));
                if (px < width - 1) queue.Enqueue(new Point(px + 1, py));
                if (py > 0) queue.Enqueue(new Point(px, py - 1));
                if (py < height - 1) queue.Enqueue(new Point(px, py + 1));
            }
        }

        private Color GetColorAtPoint(int x, int y)
        {
            if (x < 0 || x >= bitmap.PixelWidth || y < 0 || y >= bitmap.PixelHeight)
                return Colors.Transparent;

            int pixelIndex = (y * bitmap.PixelWidth + x) * 4;
            return Color.FromArgb(
                pixelBuffer[pixelIndex + 3],
                pixelBuffer[pixelIndex + 2],
                pixelBuffer[pixelIndex + 1],
                pixelBuffer[pixelIndex]
            );
        }

        private void UpdateBitmap()
        {
            bitmap.WritePixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), pixelBuffer, bitmap.PixelWidth * 4, 0);
        }
    }
}
