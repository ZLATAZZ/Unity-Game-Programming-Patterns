using System;
using Patterns.Command.Customization.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Patterns.Command.UI
{
    public sealed class CustomizationOptionButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Image _icon;

        private CustomizationOption _option;
        private Action<CustomizationOption> _onSelected;

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

        public void Initialize(CustomizationOption option, Action<CustomizationOption> onSelected)
        {
            _option = option ?? throw new ArgumentNullException(nameof(option));
            _onSelected = onSelected ?? throw new ArgumentNullException(nameof(onSelected));

            _label.text = _option.DisplayName;

            if (_icon != null)
            {
                _icon.sprite = _option.Icon;
                _icon.enabled = _option.Icon != null;
            }
        }

        public void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
        }

        private void HandleClick()
        {
            _onSelected.Invoke(_option);
        }
    }
}