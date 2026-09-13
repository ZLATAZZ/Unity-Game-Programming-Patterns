using Patterns.Command.Core;
using Patterns.Command.Customization.Core;
using Patterns.Command.Customization.Presets;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Patterns.Command.UI
{
    public sealed class CustomizationPresetPanel : MonoBehaviour
    {
        [SerializeField] private Transform _buttonsRoot;
        [SerializeField] private CustomizationPresetButton _buttonPrefab;

        private CustomizationController _controller;
        private CustomizationPresetCommandFactory _commandFactory;

        private bool _isInitialized;

        public void Initialize(CustomizationController controller, CustomizationPresetCommandFactory commandFactory, IReadOnlyList<CustomizationPreset> presets)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{name} is already initialized.");
            }

            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _commandFactory = commandFactory ?? throw new ArgumentNullException(nameof(commandFactory));

            if (presets == null)
            {
                throw new ArgumentNullException(nameof(presets));
            }

            if (_buttonsRoot == null)
            {
                throw new InvalidOperationException($"{name} has no buttons root.");
            }

            if (_buttonPrefab == null)
            {
                throw new InvalidOperationException($"{name} has no preset button prefab.");
            }

            foreach (CustomizationPreset preset in presets)
            {
                if (preset == null)
                {
                    throw new ArgumentException("Preset collection cannot contain null references.", nameof(presets));
                }

                CustomizationPresetButton button = Instantiate(_buttonPrefab, _buttonsRoot);
                button.Initialize(preset, ApplyPreset);
            }

            _isInitialized = true;
        }

        private void ApplyPreset(CustomizationPreset preset)
        {
            if (!_commandFactory.TryCreateCommand(preset, out ICommand command))
            {
                return;
            }

            _controller.ExecuteCommand(command);
        }
    }
}