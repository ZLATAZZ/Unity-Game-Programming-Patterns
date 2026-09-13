using Patterns.Command.Customization.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Patterns.Command.UI
{
    public sealed class CustomizationMenu : MonoBehaviour
    {
        [Header("Containers")]
        [SerializeField] private Transform _featuresRoot;
        [SerializeField] private Transform _optionsRoot;

        [Header("Prefabs")]
        [SerializeField] private CustomizationFeatureButton _featureButtonPrefab;
        [SerializeField] private CustomizationOptionButton _optionButtonPrefab;

        [Header("History")]
        [SerializeField] private Button _undoButton;
        [SerializeField] private Button _redoButton;

        private readonly List<CustomizationFeatureButton> _featureButtons = new();
        private readonly List<CustomizationOptionButton> _optionButtons = new();

        private CustomizationController _controller;
        private IReadOnlyList<CustomizationFeature> _features;
        private CustomizationFeature _selectedFeature;

        private bool _isInitialized;

        private void Awake()
        {
            ValidateReferences();

            _undoButton.onClick.AddListener(Undo);
            _redoButton.onClick.AddListener(Redo);
        }

        private void OnDestroy()
        {
            _undoButton.onClick.RemoveListener(Undo);
            _redoButton.onClick.RemoveListener(Redo);
        }

        public void Initialize(CustomizationController controller, IReadOnlyList<CustomizationFeature> features)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{name} is already initialized.");
            }

            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _features = features ?? throw new ArgumentNullException(nameof(features));

            if (_features.Count == 0)
            {
                throw new InvalidOperationException($"{name} received no customization features.");
            }

            CreateFeatureButtons();

            _isInitialized = true;

            SelectFeature(_features[0]);
            RefreshHistoryControls();
        }

        private void CreateFeatureButtons()
        {
            foreach (CustomizationFeature feature in _features)
            {
                if (feature == null)
                {
                    throw new InvalidOperationException($"{name} received a null customization feature.");
                }

                CustomizationFeatureButton button = Instantiate(_featureButtonPrefab, _featuresRoot);
                button.Initialize(feature, SelectFeature);

                _featureButtons.Add(button);
            }
        }

        private void SelectFeature(CustomizationFeature feature)
        {
            if (feature == null)
            {
                throw new ArgumentNullException(nameof(feature));
            }

            _selectedFeature = feature;

            RebuildOptionButtons();
        }

        private void RebuildOptionButtons()
        {
            ClearOptionButtons();

            foreach (CustomizationOption option in _selectedFeature.AvailableOptions)
            {
                CustomizationOptionButton button = Instantiate(_optionButtonPrefab, _optionsRoot);
                button.Initialize(option, SelectOption);

                _optionButtons.Add(button);
            }
        }

        private void SelectOption(CustomizationOption option)
        {
            if (_selectedFeature == null)
            {
                throw new InvalidOperationException("No customization feature is currently selected.");
            }

            _controller.ChangeOption(_selectedFeature, option);

            RefreshHistoryControls();
        }

        private void Undo()
        {
            _controller.Undo();

            RefreshHistoryControls();
        }

        private void Redo()
        {
            _controller.Redo();

            RefreshHistoryControls();
        }

        private void RefreshHistoryControls()
        {
            _undoButton.interactable = _controller.CanUndo;
            _redoButton.interactable = _controller.CanRedo;
        }

        private void ClearOptionButtons()
        {
            foreach (CustomizationOptionButton button in _optionButtons)
            {
                Destroy(button.gameObject);
            }

            _optionButtons.Clear();
        }

        private void ValidateReferences()
        {
            if (_featuresRoot == null)
            {
                throw new InvalidOperationException($"{name} has no categories root.");
            }

            if (_optionsRoot == null)
            {
                throw new InvalidOperationException($"{name} has no options root.");
            }

            if (_featureButtonPrefab == null)
            {
                throw new InvalidOperationException($"{name} has no category button prefab.");
            }

            if (_optionButtonPrefab == null)
            {
                throw new InvalidOperationException($"{name} has no option button prefab.");
            }

            if (_undoButton == null)
            {
                throw new InvalidOperationException($"{name} has no undo button.");
            }

            if (_redoButton == null)
            {
                throw new InvalidOperationException($"{name} has no redo button.");
            }
        }
    }
}