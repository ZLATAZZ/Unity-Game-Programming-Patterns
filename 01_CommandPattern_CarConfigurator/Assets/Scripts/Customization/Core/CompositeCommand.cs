using System;
using System.Collections.Generic;
using Patterns.Command.Core;

namespace Patterns.Command.Customization.Core
{
    public sealed class CompositeCommand : ICommand
    {
        private readonly IReadOnlyList<ICommand> _commands;

        public CompositeCommand(IReadOnlyList<ICommand> commands)
        {
            if (commands == null)
            {
                throw new ArgumentNullException(nameof(commands));
            }

            if (commands.Count == 0)
            {
                throw new ArgumentException("Composite command must contain at least one command.", nameof(commands));
            }

            List<ICommand> snapshot = new(commands.Count);

            foreach (ICommand command in commands)
            {
                if (command == null)
                {
                    throw new ArgumentException("Composite command cannot contain null commands.", nameof(commands));
                }

                snapshot.Add(command);
            }

            _commands = snapshot;
        }

        public void Execute()
        {
            foreach (ICommand command in _commands)
            {
                command.Execute();
            }
        }

        public void Undo()
        {
            for (int i = _commands.Count - 1; i >= 0; i--)
            {
                _commands[i].Undo();
            }
        }
    }
}