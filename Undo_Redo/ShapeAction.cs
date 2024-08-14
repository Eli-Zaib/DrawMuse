using DrawMuse;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

public class ShapeAction : IHistoryAction
{
    private Shape shape;
    private Canvas canvas;
    private UIElementCollection originalChildren;
    private int index;

    public ShapeAction(Shape shape, Canvas canvas)
    {
        this.shape = shape;
        this.canvas = canvas;
        originalChildren = canvas.Children;
        index = originalChildren.IndexOf(shape);
    }

    public void Undo(Canvas canvas)
    {
        if (canvas.Children.Contains(shape))
        {
            canvas.Children.Remove(shape);
        }
    }

    public void Redo(Canvas canvas)
    {
        if (!canvas.Children.Contains(shape))
        {
            canvas.Children.Add(shape);
        }
    }
}
