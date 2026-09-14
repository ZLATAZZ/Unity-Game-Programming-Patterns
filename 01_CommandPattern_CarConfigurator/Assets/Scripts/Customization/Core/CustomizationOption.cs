using UnityEngine;

namespace Patterns.Command.Customization.Core
{
    public abstract class CustomizationOption : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;

        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
    }
}