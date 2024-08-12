using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DrawMuse
{
    internal class ColorBucket
    {
        private Stack<byte[]> undoStack;
        private Stack<byte[]> redoStack;
        private byte[] pixelBuffer;
        private WriteableBitmap bitmap;
        private bool isBucketToolActive;
        private Color currentColor;
        private Canvas drawingCanvas;
        private MainUndoRedoManager undoRedoManager; // Add this field

        public ColorBucket(Canvas canvas, MainUndoRedoManager undoRedoManager)
        {
            drawingCanvas = canvas;
            this.undoRedoManager = undoRedoManager; // Initialize undoRedoManager

            undoStack = new Stack<byte[]>();
            redoStack = new Stack<byte[]>();

            int width = (int)canvas.Width;
            int height = (int)canvas.Height;

            bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            pixelBuffer = new byte[width * height * 4];

            Image image = new Image { Source = bitmap };
            drawingCanvas.Children.Add(image);
        }

        public void UpdateBucketColor(Color color)
        {
            currentColor = color;
        }

        public void ActivateBucket()
        {
            isBucketToolActive = true;
        }

        public void FillArea(Point point)
        {
            if (bitmap == null || pixelBuffer == null)
            {
                MessageBox.Show("Bitmap or pixelBuffer is not initialized.");
                return;
            }

            if (isBucketToolActive)
            {
                SaveState();

                int x = (int)point.X;
                int y = (int)point.Y;

                var targetColor = GetColorAtPoint(x, y);

                if (targetColor.HasValue)
                {
                    var bucketAction = new BucketAction(x, y, targetColor.Value, currentColor, bitmap, pixelBuffer);
                    undoRedoManager.Do(bucketAction); // Use Do method to handle actions
                }
            }
        }

        private Color? GetColorAtPoint(int x, int y)
        {
            if (x < 0 || x >= bitmap.PixelWidth || y < 0 || y >= bitmap.PixelHeight)
                return null;

            int pixelIndex = (y * bitmap.PixelWidth + x) * 4;
            if (pixelBuffer == null || pixelIndex >= pixelBuffer.Length)
                return null;

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

                Color currentPixelColor = GetColorAtPoint(px, py).Value;

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

            UpdateBitmap(); // Update the bitmap with the pixel buffer
        }

        private void UpdateBitmap()
        {
            if (bitmap == null || pixelBuffer == null)
            {
                MessageBox.Show("Bitmap or pixelBuffer is not initialized.");
                return;
            }

            bitmap.WritePixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), pixelBuffer, bitmap.PixelWidth * 4, 0);
        }

        public void SaveState()
        {
            if (pixelBuffer == null)
            {
                MessageBox.Show("PixelBuffer is not initialized.");
                return;
            }

            byte[] state = new byte[pixelBuffer.Length];
            Array.Copy(pixelBuffer, state, pixelBuffer.Length);
            undoStack.Push(state);
            redoStack.Clear();
        }

        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                byte[] previousState = undoStack.Pop();
                byte[] currentState = new byte[pixelBuffer.Length];
                Array.Copy(pixelBuffer, currentState, pixelBuffer.Length);
                redoStack.Push(currentState);

                Array.Copy(previousState, pixelBuffer, pixelBuffer.Length);
                UpdateBitmap();
            }
        }

        public void Redo()
        {
            if (redoStack.Count > 0)
            {
                byte[] nextState = redoStack.Pop();
                byte[] currentState = new byte[pixelBuffer.Length];
                Array.Copy(pixelBuffer, currentState, pixelBuffer.Length);
                undoStack.Push(currentState);

                Array.Copy(nextState, pixelBuffer, pixelBuffer.Length);
                UpdateBitmap();
            }
        }
    }
}
