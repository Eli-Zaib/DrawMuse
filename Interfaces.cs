using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;

namespace DrawMuse
{
    internal class Interfaces
    {

    }

    interface IDrawingTools
    {
        void Pencil();

        void RemovePencil();
        void Brush();
        void DifferentBrushes(); // Different types of brushes, to be defined later

        void Undo();
        void Redo();
    }

    interface IShapesTools
    {
        void Line(Canvas canvas, Point startPoint, Point endPoint);
        void Cube();
        void Cylinder();
        void Circle();
        void Rectangle();
    }

    interface IUndoRedo
    {
        void Execute(Canvas canvas);
        void Undo(Canvas canvas);
        void Redo(Canvas canvas);
    }


    interface IAdjustableSize
    {
        void Resize(int newSize);
    }


    interface ISaveFile
    {
        void Save();
        void HandleSaveError();

        void NameSaveFile();
        void ChooseSaveFilePath();
    }


    interface IEraserTool
    {
        void Eraser(Point position, double size);
    }


    interface IColorTools
    {
        void ColorPalette();
        void ColorPicker();
        void ColorBucket();
    }


    interface IFormat
    {
        void SaveAsPNG();
        void SaveAsJPEG();
    }
}
