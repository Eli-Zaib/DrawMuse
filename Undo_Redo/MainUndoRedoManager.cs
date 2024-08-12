using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DrawMuse
{
    public class MainUndoRedoManager
    {
        private Stack<IHistoryAction> undoStack;
        private Stack<IHistoryAction> redoStack;
        private Canvas canvas;

        public MainUndoRedoManager(Canvas canvas)
        {
            this.canvas = canvas;
            undoStack = new Stack<IHistoryAction>();
            redoStack = new Stack<IHistoryAction>();
        }

        public void Do(IHistoryAction action)
        {
            undoStack.Push(action);
            redoStack.Clear(); // Clear redo stack on new action
            action.Redo(canvas); // Apply the action to the canvas
        }

        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                var action = undoStack.Pop();
                action.Undo(canvas);
                redoStack.Push(action);
            }
        }

        public void Redo()
        {
            if (redoStack.Count > 0)
            {
                var action = redoStack.Pop();
                action.Redo(canvas);
                undoStack.Push(action);
            }
        }

        public bool CanUndo => undoStack.Count > 0;
        public bool CanRedo => redoStack.Count > 0;
    }
}
