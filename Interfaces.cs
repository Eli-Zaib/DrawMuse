using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;

namespace DrawMuse
{
    internal class Interfaces
    {

    }

    public interface IDrawingTools
    {
        void Pencil();    
        void SetBrush(SolidColorBrush brush);
        void RemovePencil();
        void SetSize(double size);
        void Undo();
        void Redo();
    }

    public interface IAdjustableSize
    {
        void Resize(int newSize);
    }
    public interface ISaveFile
    {
        void Save();
        void HandleSaveError();

        void NameSaveFile();
        void ChooseSaveFilePath();
    }


    public interface IEraserTool
    {
        void Eraser(Point position, double size);
    }


    public interface IColorManager
    {

        void CreateColorPalette(Panel palletPanel);
        void OnColorSelected(object sender, System.Windows.Input.MouseButtonEventArgs e);
        void SelectedColor(Color color);

    }

    public interface IColorTools
    {
        void UseEyeDropper(Canvas canvas);
        void DisableEyeDropper(Canvas canvas);

    }


    public interface IFormat
    {
        void SaveAsPNG();
        void SaveAsJPEG();
    }

    public  interface IHistoryAction
    {
        void Undo(Canvas canvas);
        void Redo(Canvas canvas);

    }

}
