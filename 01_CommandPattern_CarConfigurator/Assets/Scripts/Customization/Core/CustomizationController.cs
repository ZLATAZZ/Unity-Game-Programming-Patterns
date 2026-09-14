using System;
using Patterns.Command.Core;

namespace Patterns.Command.Customization.Core
{
    public sealed class CustomizationController
    {
        private readonly CommandHistory _commandHistory;

        public bool CanUndo => _commandHistory.CanUndo;
        public bool CanRedo => _commandHistory.CanRedo;

        public event Action HistoryChanged;

        public CustomizationController(CommandHistory commandHistory)
        {
            _commandHistory = commandHistory ?? throw new ArgumentNullException(nameof(commandHistory));
        }

        public void ChangeOption(CustomizationFeature feature, CustomizationOption option)
        {
            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            if (!feature.IsInitialized)
            {
                throw new InvalidOperationException($"{feature.name} is not initialized.");
            }

            if (!feature.IsOptionAvailable(option))
            {
                throw new ArgumentException($"{option.name} is not available for {feature.name}.", nameof(option));
            }

            if (feature.CurrentOption == option)
            {
                return;
            }

            ICommand command = new ChangeCustomizationCommand(feature, option);

            ExecuteCommand(command);
        }

        public void ExecuteCommand(ICommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            _commandHistory.ExecuteCommand(command);

            HistoryChanged?.Invoke();
        }

        public void Undo()
        {
            if (!_commandHistory.CanUndo)
            {
                return;
            }

            _commandHistory.UndoCommand();

            HistoryChanged?.Invoke();
        }

        public void Redo()
        {
            if (!_commandHistory.CanRedo)
            {
                return;
            }

            _commandHistory.RedoCommand();

            HistoryChanged?.Invoke();
        }
    }
}