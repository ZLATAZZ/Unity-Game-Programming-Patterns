using System;
using System.Collections.Generic;
using Patterns.Command.Customization.Core;
using UnityEngine;

namespace Patterns.Command.Customization.Variants
{
    public sealed class VariantFeature : CustomizationFeature
    {
        [SerializeField] private Transform _variantRoot;

        private readonly Dictionary<VariantOption, GameObject> _instances = new();

        private GameObject _activeInstance;

        protected override void ApplyOptionInternal(CustomizationOption option)
        {
            if (_variantRoot == null)
            {
                throw new InvalidOperationException($"{name} has no variant root.");
            }

            if (option is not VariantOption variantOption)
            {
                throw new InvalidOperationException($"Option {option.name} is not a VariantOption.");
            }

            if (_activeInstance != null)
            {
                _activeInstance.SetActive(false);
            }

            if (variantOption.IsEmpty)
            {
                _activeInstance = null;
                return;
            }

            GameObject instance = GetOrCreateInstance(variantOption);

            instance.SetActive(true);
            _activeInstance = instance;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_variantRoot == null)
            {
                Debug.LogWarning($"{name} has no variant root.", this);
            }

            foreach (CustomizationOption option in AvailableOptions)
            {
                if (option != null && option is not VariantOption)
                {
                    Debug.LogWarning($"Option {option.name} assigned to {name} is not a VariantOption.", this);
                }
            }
        }

        private GameObject GetOrCreateInstance(VariantOption option)
        {
            if (_instances.TryGetValue(option, out GameObject instance))
            {
                return instance;
            }

            if (option.Prefab == null)
            {
                throw new InvalidOperationException($"Variant option {option.name} has no prefab.");
            }

            instance = Instantiate(option.Prefab, _variantRoot, false);
            instance.name = option.DisplayName;

            _instances.Add(option, instance);

            return instance;
        }
    }
}