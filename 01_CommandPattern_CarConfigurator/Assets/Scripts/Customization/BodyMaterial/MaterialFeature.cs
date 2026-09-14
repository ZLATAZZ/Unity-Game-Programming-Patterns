using System;
using System.Collections.Generic;
using Patterns.Command.Customization.Core;
using UnityEngine;

namespace Patterns.Command.Customization.BodyMaterial
{
    public sealed class MaterialFeature : CustomizationFeature
    {
        [Header("Runtime Targets")]
        [SerializeField] private MaterialSlotTarget[] _targets;

        [Header("Editor Target Collection")]
        [SerializeField] private Transform _collectionRoot;
        [SerializeField] private Material _sourceMaterial;

        private readonly Dictionary<Renderer, Material[]> _materialsByRenderer = new();

        protected override void ApplyOptionInternal(CustomizationOption option)
        {
            if (option is not MaterialOption materialOption)
            {
                throw new InvalidOperationException($"Option {option.name} is not a MaterialOption.");
            }

            if (materialOption.Material == null)
            {
                throw new InvalidOperationException($"Material option {materialOption.name} has no material assigned.");
            }

            ValidateTargets();
            ApplyMaterial(materialOption.Material);
        }

        private void ApplyMaterial(Material material)
        {
            _materialsByRenderer.Clear();

            foreach (MaterialSlotTarget target in _targets)
            {
                Renderer renderer = target.Renderer;

                if (!_materialsByRenderer.TryGetValue(renderer, out Material[] materials))
                {
                    materials = renderer.sharedMaterials;
                    _materialsByRenderer.Add(renderer, materials);
                }

                materials[target.MaterialIndex] = material;
            }

            foreach (KeyValuePair<Renderer, Material[]> entry in _materialsByRenderer)
            {
                entry.Key.sharedMaterials = entry.Value;
            }
        }

        private void ValidateTargets()
        {
            if (_targets == null || _targets.Length == 0)
            {
                throw new InvalidOperationException($"{name} has no material slot targets.");
            }

            foreach (MaterialSlotTarget target in _targets)
            {
                if (target == null)
                {
                    throw new InvalidOperationException($"{name} contains a null material slot target.");
                }

                if (target.Renderer == null)
                {
                    throw new InvalidOperationException($"{name} contains a material slot target without a renderer.");
                }

                int materialCount = target.Renderer.sharedMaterials.Length;

                if (target.MaterialIndex < 0 || target.MaterialIndex >= materialCount)
                {
                    throw new InvalidOperationException(
                        $"Material index {target.MaterialIndex} is invalid for renderer {target.Renderer.name}. " +
                        $"The renderer contains {materialCount} material slots.");
                }
            }
        }
    }
}