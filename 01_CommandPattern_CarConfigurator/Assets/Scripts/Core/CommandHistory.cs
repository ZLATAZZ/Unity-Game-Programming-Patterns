using System.Collections.Generic;

namespace Patterns.Command.Core
{
    public class CommandHistory
    {
        private readonly Stack<ICommand> undoStack = new();
        private readonly Stack<ICommand> redoStack = new();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();

            undoStack.Push(command);

            redoStack.Clear();
        }

        public void UndoCommand()
        {
            if (undoStack.Count > 0)
            {
                ICommand lastCommand = undoStack.Pop();
                lastCommand.Undo();
                redoStack.Push(lastCommand);
            }
        }

        public void RedoCommand()
        {
            if (redoStack.Count > 0)
            {
                ICommand lastCommand = redoStack.Pop();
                lastCommand.Execute();
                undoStack.Push(lastCommand);
            }
        }
    }
}