using Patterns.Command.Core;
using System;

namespace Patterns.Command.Customization.Core
{
    public sealed class ChangeCustomizationCommand : ICommand
    {
        private readonly CustomizationFeature _feature;

        private readonly CustomizationOption _previousOption;

        private readonly CustomizationOption _newOption;

        public ChangeCustomizationCommand(CustomizationFeature feature, CustomizationOption newOption)
        {
            _feature = feature ?? throw new ArgumentNullException(nameof(feature)); ;
            _previousOption = feature.CurrentOption ?? throw new InvalidOperationException($"{_feature.name} has no current option.");
            _newOption = newOption ?? throw new ArgumentNullException(nameof(newOption));

            if (!_feature.IsInitialized) 
            {
                throw new InvalidOperationException($"{_feature.name} is not initialized.");
            }

            if (!_feature.IsOptionAvailable(_newOption))
            {
                throw new ArgumentException($"{_newOption.name} is not available for {_feature.name}.", nameof(newOption));
            }
        }

        public void Execute()
        {
            _feature.ApplyOption(_newOption);
        }

        public void Undo()
        {
            _feature.ApplyOption(_previousOption);
        }
    }
}