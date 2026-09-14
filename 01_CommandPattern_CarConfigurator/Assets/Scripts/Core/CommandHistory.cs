using System.Collections.Generic;

namespace Patterns.Command.Core
{
    public sealed class CommandHistory
    {
        private readonly Stack<ICommand> _undoStack = new();
        private readonly Stack<ICommand> _redoStack = new();

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();

            _undoStack.Push(command);
            _redoStack.Clear();
        }

        public void UndoCommand()
        {
            if (!_undoStack.TryPop(out ICommand command))
            {
                return;
            }

            command.Undo();
            _redoStack.Push(command);
        }

        public void RedoCommand()
        {
            if (!_redoStack.TryPop(out ICommand command))
            {
                return;
            }

            command.Execute();
            _undoStack.Push(command);
        }
    }
}