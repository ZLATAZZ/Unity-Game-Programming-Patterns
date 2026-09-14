using Patterns.Command.Customization.Core;
using UnityEngine;

namespace Patterns.Command.Customization.Variants
{
    [CreateAssetMenu(fileName = "VariantOption", menuName = "Patterns/Command/Customization/Variant Option")]
    public sealed class VariantOption : CustomizationOption
    {
        [SerializeField] private GameObject _prefab;
        [SerializeField] private bool _allowEmpty;

        public GameObject Prefab => _prefab;
        public bool IsEmpty => _prefab == null;

        private void OnValidate()
        {
            if (_prefab == null && !_allowEmpty)
            {
                Debug.LogWarning($"{name} has no prefab assigned. Enable Allow Empty only for an intentionally empty variant.", this);
            }

            if (_prefab != null && _allowEmpty)
            {
                Debug.LogWarning($"{name} has a prefab assigned, so Allow Empty is unnecessary.", this);
            }
        }
    }
}