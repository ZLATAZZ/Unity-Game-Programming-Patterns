using System;
using System.Collections.Generic;
using UnityEngine;

namespace Patterns.Command.Customization.Core
{
    public abstract class CustomizationFeature : MonoBehaviour
    {
        [SerializeField] private CustomizationOption[] _availableOptions;
        [SerializeField] private CustomizationOption _initialOption;

        private bool _isInitialized;

        public IReadOnlyList<CustomizationOption> AvailableOptions => _availableOptions;
        public CustomizationOption CurrentOption { get; private set; }
        public bool IsInitialized => _isInitialized;

        public void Initialize()
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{name} is already initialized.");
            }

            if (_availableOptions == null || _availableOptions.Length == 0)
            {
                throw new InvalidOperationException($"{name} has no customization options.");
            }

            foreach (CustomizationOption option in _availableOptions)
            {
                if (option == null)
                {
                    throw new InvalidOperationException($"{name} contains a null customization option.");
                }
            }

            if (_initialOption == null)
            {
                throw new InvalidOperationException($"{name} has no initial option.");
            }

            if (!IsOptionAvailable(_initialOption))
            {
                throw new InvalidOperationException($"Initial option {_initialOption.name} is not available for {name}.");
            }

            ApplyValidatedOption(_initialOption);
            _isInitialized = true;
        }

        public bool IsOptionAvailable(CustomizationOption option)
        {
            return option != null && Array.Exists(_availableOptions, available => available == option);
        }

        internal void ApplyOption(CustomizationOption option)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"{name} must be initialized before applying an option.");
            }

            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            if (!IsOptionAvailable(option))
            {
                throw new ArgumentException($"Option {option.name} is not available for {name}.", nameof(option));
            }

            ApplyValidatedOption(option);
        }

        protected virtual void OnValidate()
        {
            if (_availableOptions == null || _availableOptions.Length == 0)
            {
                Debug.LogWarning($"{name} has no customization options.", this);
                return;
            }

            foreach (CustomizationOption option in _availableOptions)
            {
                if (option == null)
                {
                    Debug.LogWarning($"{name} contains a null customization option.", this);
                }
            }

            if (_initialOption == null)
            {
                Debug.LogWarning($"{name} has no initial option.", this);
                return;
            }

            if (!IsOptionAvailable(_initialOption))
            {
                Debug.LogWarning($"Initial option {_initialOption.name} is not included in the available options for {name}.", this);
            }
        }

        protected abstract void ApplyOptionInternal(CustomizationOption option);

        private void ApplyValidatedOption(CustomizationOption option)
        {
            ApplyOptionInternal(option);
            CurrentOption = option;
        }
    }
}