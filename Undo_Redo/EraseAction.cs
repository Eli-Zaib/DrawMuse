using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace DrawMuse
{
    public class EraseAction : IHistoryAction
    {
        private List<UIElement> erasedElements;
        private Canvas canvas;

        public EraseAction(List<UIElement> erasedElements, Canvas canvas)
        {
            this.erasedElements = erasedElements;
            this.canvas = canvas;
        }

        public void Undo(Canvas canvas)
        {
            foreach (var element in erasedElements)
            {
                canvas.Children.Add(element);
            }
        }

        public void Redo(Canvas canvas)
        {
            foreach (var element in erasedElements)
            {
                canvas.Children.Remove(element);
            }
        }
    }

}
