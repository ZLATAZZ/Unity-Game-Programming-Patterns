using System.Collections.Generic;
using Patterns.Command.Customization.Core;
using UnityEngine;

namespace Patterns.Command.Customization.Presets
{
    [CreateAssetMenu(fileName = "CustomizationPreset", menuName = "Patterns/Command/Customization/Preset")]
    public sealed class CustomizationPreset : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;
        [SerializeField] private CustomizationOption[] _options;

        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
        public IReadOnlyList<CustomizationOption> Options => _options;

        private void OnValidate()
        {
            if (_options == null || _options.Length == 0)
            {
                Debug.LogWarning($"{name} has no customization options.", this);
                return;
            }

            for (int i = 0; i < _options.Length; i++)
            {
                if (_options[i] == null)
                {
                    Debug.LogWarning($"{name} contains a null customization option.", this);
                    continue;
                }

                for (int j = i + 1; j < _options.Length; j++)
                {
                    if (_options[i] == _options[j])
                    {
                        Debug.LogWarning($"{name} contains duplicate option {_options[i].name}.", this);
                    }
                }
            }
        }
    }
}