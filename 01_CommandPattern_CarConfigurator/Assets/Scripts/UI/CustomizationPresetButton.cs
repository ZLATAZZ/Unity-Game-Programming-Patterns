using System;
using Patterns.Command.Customization.Presets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Patterns.Command.UI
{
    public sealed class CustomizationPresetButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Image _icon;

        private CustomizationPreset _preset;
        private Action<CustomizationPreset> _onSelected;

        private void Awake()
        {
            if (_button == null)
            {
                throw new InvalidOperationException($"{name} has no Button assigned.");
            }

            if (_label == null)
            {
                throw new InvalidOperationException($"{name} has no label assigned.");
            }

            _button.onClick.AddListener(HandleClick);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(HandleClick);
        }

        public void Initialize(CustomizationPreset preset, Action<CustomizationPreset> onSelected)
        {
            _preset = preset ?? throw new ArgumentNullException(nameof(preset));
            _onSelected = onSelected ?? throw new ArgumentNullException(nameof(onSelected));

            _label.text = _preset.DisplayName;

            if (_icon != null)
            {
                _icon.sprite = _preset.Icon;
                _icon.enabled = _preset.Icon != null;
            }
        }

        private void HandleClick()
        {
            _onSelected.Invoke(_preset);
        }
    }
}