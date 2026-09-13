using Patterns.Command.Customization.Core;
using UnityEngine;

namespace Patterns.Command.Customization.BodyMaterial
{
    [CreateAssetMenu(fileName = "MaterialOption", menuName = "Patterns/Command/Customization/Material Option")]
    public sealed class MaterialOption : CustomizationOption
    {
        [SerializeField] private Material _material;

        public Material Material => _material;

        private void OnValidate()
        {
            if (_material == null)
            {
                Debug.LogWarning($"{name} has no material assigned.", this);
            }
        }
    }
}