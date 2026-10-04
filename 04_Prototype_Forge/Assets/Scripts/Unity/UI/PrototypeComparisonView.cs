using System;
using PrototypeForge.Core.Enemies;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeForge.Unity.UI
{
    public sealed class PrototypeComparisonView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _prototypeStatsLabel;
        [SerializeField] private TMP_Text _cloneStatsLabel;

        [SerializeField] private Button _cloneButton;
        [SerializeField] private Button _overchargeButton;
        [SerializeField] private Button _clearButton;

        [SerializeField] private string _emptyPrototypeText = "Select a prototype";
        [SerializeField] private string _emptyCloneText = "No clone created";

        private bool _isInitialized;

        public event Action CloneRequested;
        public event Action OverchargeRequested;
        public event Action ClearRequested;

        public void Initialize()
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(PrototypeComparisonView)} has already been initialized.");
            }

            ValidateConfiguration();

            _prototypeStatsLabel.text = _emptyPrototypeText;
            _cloneStatsLabel.text = _emptyCloneText;

            _cloneButton.interactable = false;
            _overchargeButton.interactable = false;

            _cloneButton.onClick.AddListener(HandleCloneClicked);
            _overchargeButton.onClick.AddListener(HandleOverchargeClicked);
            _clearButton.onClick.AddListener(HandleClearClicked);

            _isInitialized = true;
        }

        public void SetSelectedPrototype(EnemyPrototype prototype)
        {
            EnsureInitialized();

            _prototypeStatsLabel.text = FormatStats(prototype);
            _cloneButton.interactable = true;
        }

        public void SetActiveClone(EnemyPrototype clone)
        {
            EnsureInitialized();

            _cloneStatsLabel.text = FormatStats(clone);
            _overchargeButton.interactable = true;
        }

        public void SetCloneAvailable(bool isAvailable)
        {
            EnsureInitialized();

            _cloneButton.interactable = isAvailable;
        }

        public void ClearClone()
        {
            EnsureInitialized();

            _cloneStatsLabel.text = _emptyCloneText;
            _overchargeButton.interactable = false;
        }

        private static string FormatStats(EnemyPrototype prototype)
        {
            return
                $"{prototype.DisplayName}\n" +
                $"HP: {prototype.MaxHealth}\n" +
                $"Speed: {prototype.MoveSpeed:0.#}\n" +
                $"Damage: {prototype.Weapon.Damage}\n" +
                $"Cooldown: {prototype.Weapon.Cooldown:0.##}s";
        }

        private void HandleCloneClicked()
        {
            CloneRequested?.Invoke();
        }

        private void HandleOverchargeClicked()
        {
            OverchargeRequested?.Invoke();
        }

        private void HandleClearClicked()
        {
            ClearRequested?.Invoke();
        }

        private void OnDestroy()
        {
            if (_cloneButton != null)
            {
                _cloneButton.onClick.RemoveListener(HandleCloneClicked);
            }

            if (_overchargeButton != null)
            {
                _overchargeButton.onClick.RemoveListener(HandleOverchargeClicked);
            }
            if (_clearButton != null)
            {
                _clearButton.onClick.RemoveListener(HandleClearClicked);
            }
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(PrototypeComparisonView)} has not been initialized.");
            }
        }

        private void ValidateConfiguration()
        {
            if (_prototypeStatsLabel == null)
            {
                throw new InvalidOperationException($"{nameof(PrototypeComparisonView)} requires a Prototype Stats Label.");
            }

            if (_cloneStatsLabel == null)
            {
                throw new InvalidOperationException($"{nameof(PrototypeComparisonView)} requires a Clone Stats Label.");
            }

            if (_cloneButton == null)
            {
                throw new InvalidOperationException($"{nameof(PrototypeComparisonView)} requires a Clone Button.");
            }

            if (_overchargeButton == null)
            {
                throw new InvalidOperationException($"{nameof(PrototypeComparisonView)} requires an Overcharge Button.");
            }

            if (_clearButton == null)
            {
                throw new InvalidOperationException($"{nameof(PrototypeComparisonView)} requires a Clear Button.");
            }
        }
    }
}