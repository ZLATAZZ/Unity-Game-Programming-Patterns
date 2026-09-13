using System;
using Patterns.Command.Customization.Core;
using UnityEngine;

namespace Patterns.Command.Customization.BodyMaterial
{
    public sealed class MaterialFeature : CustomizationFeature
    {
        [SerializeField] private Renderer _targetRenderer;

        protected override void ApplyOptionInternal(CustomizationOption option)
        {
            if (_targetRenderer == null)
            {
                throw new InvalidOperationException($"{name} has no target renderer.");
            }

            if (option is not MaterialOption materialOption)
            {
                throw new InvalidOperationException($"Option {option.name} is not a MaterialOption.");
            }

            if (materialOption.Material == null)
            {
                throw new InvalidOperationException($"Material option {materialOption.name} has no material assigned.");
            }

            _targetRenderer.sharedMaterial = materialOption.Material;
        }

        protected override void OnValidate()
        {
            base.OnValidate();

            if (_targetRenderer == null)
            {
                Debug.LogWarning($"{name} has no target renderer.", this);
            }

            foreach (CustomizationOption option in AvailableOptions)
            {
                if (option != null && option is not MaterialOption)
                {
                    Debug.LogWarning($"Option {option.name} assigned to {name} is not a MaterialOption.", this);
                }
            }
        }
    }
}