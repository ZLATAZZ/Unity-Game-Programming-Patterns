using System;
using System.Collections.Generic;
using Patterns.Command.Core;
using Patterns.Command.Customization.Core;

namespace Patterns.Command.Customization.Presets
{
    public sealed class CustomizationPresetCommandFactory
    {
        private readonly IReadOnlyList<CustomizationFeature> _features;

        public CustomizationPresetCommandFactory(IReadOnlyList<CustomizationFeature> features)
        {
            if (features == null)
            {
                throw new ArgumentNullException(nameof(features));
            }

            if (features.Count == 0)
            {
                throw new ArgumentException("At least one customization feature is required.", nameof(features));
            }

            List<CustomizationFeature> snapshot = new(features.Count);

            foreach (CustomizationFeature feature in features)
            {
                if (feature == null)
                {
                    throw new ArgumentException("Customization features cannot contain null references.", nameof(features));
                }

                snapshot.Add(feature);
            }

            _features = snapshot;
        }

        public bool TryCreateCommand(CustomizationPreset preset, out ICommand command)
        {
            if (preset == null)
            {
                throw new ArgumentNullException(nameof(preset));
            }

            if (preset.Options == null || preset.Options.Count == 0)
            {
                throw new InvalidOperationException($"{preset.name} contains no customization options.");
            }

            List<ICommand> commands = new();
            HashSet<CustomizationFeature> targetedFeatures = new();

            foreach (CustomizationOption option in preset.Options)
            {
                if (option == null)
                {
                    throw new InvalidOperationException($"{preset.name} contains a null customization option.");
                }

                CustomizationFeature feature = FindFeatureForOption(option);

                if (!targetedFeatures.Add(feature))
                {
                    throw new InvalidOperationException($"{preset.name} contains more than one option for feature {feature.name}.");
                }

                if (feature.CurrentOption == option)
                {
                    continue;
                }

                commands.Add(new ChangeCustomizationCommand(feature, option));
            }

            if (commands.Count == 0)
            {
                command = null;
                return false;
            }

            command = new CompositeCommand(commands);
            return true;
        }

        private CustomizationFeature FindFeatureForOption(CustomizationOption option)
        {
            CustomizationFeature matchedFeature = null;

            foreach (CustomizationFeature feature in _features)
            {
                if (!feature.IsInitialized)
                {
                    throw new InvalidOperationException($"{feature.name} is not initialized.");
                }

                if (!feature.IsOptionAvailable(option))
                {
                    continue;
                }

                if (matchedFeature != null)
                {
                    throw new InvalidOperationException($"Option {option.name} is available in more than one customization feature.");
                }

                matchedFeature = feature;
            }

            if (matchedFeature == null)
            {
                throw new InvalidOperationException($"No customization feature supports option {option.name}.");
            }

            return matchedFeature;
        }
    }
}