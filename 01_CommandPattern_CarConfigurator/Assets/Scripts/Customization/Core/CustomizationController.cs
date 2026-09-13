using System;
using Patterns.Command.Core;

namespace Patterns.Command.Customization.Core
{
    public sealed class CustomizationController
    {
        private readonly CommandHistory _commandHistory;

        public bool CanUndo => _commandHistory.CanUndo;
        public bool CanRedo => _commandHistory.CanRedo;

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

            var command = new ChangeCustomizationCommand(feature, option);

            _commandHistory.ExecuteCommand(command);
        }

        public void Undo()
        {
            _commandHistory.UndoCommand();
        }

        public void Redo()
        {
            _commandHistory.RedoCommand();
        }
    }
}