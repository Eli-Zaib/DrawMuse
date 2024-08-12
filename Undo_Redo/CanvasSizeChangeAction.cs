using System.Windows;
using System.Windows.Controls;

namespace DrawMuse
{
    public class CanvasSizeChangeAction : IHistoryAction
    {
        public int PreviousWidth { get; }
        public int PreviousHeight { get; }
        public int NewWidth { get; }
        public int NewHeight { get; }

        public CanvasSizeChangeAction(int previousWidth, int previousHeight, int newWidth, int newHeight)
        {
            PreviousWidth = previousWidth;
            PreviousHeight = previousHeight;
            NewWidth = newWidth;
            NewHeight = newHeight;
        }

        public void Undo(Canvas canvas)
        {
            canvas.Width = PreviousWidth;
            canvas.Height = PreviousHeight;
        }

        public void Redo(Canvas canvas)
        {
            canvas.Width = NewWidth;
            canvas.Height = NewHeight;
        }
    }
}
