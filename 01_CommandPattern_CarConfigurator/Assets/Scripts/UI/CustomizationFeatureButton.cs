using System;
using Patterns.Command.Customization.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Patterns.Command.UI
{
    public sealed class CustomizationFeatureButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _label;

        private CustomizationFeature _feature;
        private Action<CustomizationFeature> _onSelected;

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

        public void Initialize(CustomizationFeature feature, Action<CustomizationFeature> onSelected)
        {
            _feature = feature ?? throw new ArgumentNullException(nameof(feature));
            _onSelected = onSelected ?? throw new ArgumentNullException(nameof(onSelected));

            _label.text = _feature.DisplayName;
        }

        private void HandleClick()
        {
            _onSelected.Invoke(_feature);
        }
    }
}