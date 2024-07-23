﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace DrawMuse
{
   
    public partial class MainWindow : Window
    {
        private IDrawingTools drawingTools;
    
        public MainWindow()
        {
            InitializeComponent();
            drawingTools = new DrawingTools(drawingCanvas);
          
        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            drawingTools.Pencil();
        }

    }

    

    interface IDrawingTools
    {
        void Pencil();
        void Brush();
        void DifferentBrushes(); // Different types of brushes, to be defined later
    }

    interface IShapesTools
    {
        void Line();
        void Cube();
        void Cylinder();
        void Circle();
        void Rectangle();
    }

    interface IUndoRedo
    {
        void Undo();
        void Redo();
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
