using Patterns.Command.Core;
using Patterns.Command.Customization.Core;
using Patterns.Command.Customization.Presets;
using Patterns.Command.UI;
using System;
using UnityEngine;

namespace Patterns.Command.Game
{
    public sealed class CarCustomizationBootstrapper : MonoBehaviour
    {
        [SerializeField] private CustomizationFeature[] _customizationFeatures;
        [SerializeField] private CustomizationPreset[] _customizationPresets;

        [SerializeField] private CustomizationMenu _customizationMenu;
        [SerializeField] private CustomizationPresetPanel _presetPanel;

        public CustomizationController Controller { get; private set; }

        private CustomizationPresetCommandFactory _presetCommandFactory;

        private void Awake()
        {
            ValidateConfiguration();

            CommandHistory commandHistory = new();

            Controller = new CustomizationController(commandHistory);
            _presetCommandFactory = new CustomizationPresetCommandFactory(_customizationFeatures);
        }

        private void Start()
        {
            InitializeFeatures();
            InitializeUI();
        }

        private void InitializeFeatures()
        {
            foreach (CustomizationFeature feature in _customizationFeatures)
            {
                feature.Initialize();
            }
        }

        private void InitializeUI()
        {
            _customizationMenu.Initialize(Controller, _customizationFeatures);
            _presetPanel.Initialize(Controller, _presetCommandFactory, _customizationPresets);
        }

        private void ValidateConfiguration()
        {
            if (_customizationFeatures == null || _customizationFeatures.Length == 0)
            {
                throw new InvalidOperationException("No customization features are configured.");
            }

            foreach (CustomizationFeature feature in _customizationFeatures)
            {
                if (feature == null)
                {
                    throw new InvalidOperationException("Customization features contain a null reference.");
                }
            }

            if (_customizationPresets == null || _customizationPresets.Length == 0)
            {
                throw new InvalidOperationException("No customization presets are configured.");
            }

            foreach (CustomizationPreset preset in _customizationPresets)
            {
                if (preset == null)
                {
                    throw new InvalidOperationException("Customization presets contain a null reference.");
                }
            }

            if (_customizationMenu == null)
            {
                throw new InvalidOperationException("No customization menu is configured.");
            }

            if (_presetPanel == null)
            {
                throw new InvalidOperationException("No customization preset panel is configured.");
            }
        }
    }
}