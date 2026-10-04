using System;
using PrototypeForge.Core.Enemies;
using PrototypeForge.Core.Prototypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PrototypeForge.Unity.UI
{
    public sealed class PrototypeCardView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _nameLabel;
        [SerializeField] private TMP_Text _statsLabel;

        private PrototypeId _prototypeId;
        private bool _isInitialized;

        public event Action<PrototypeId> Selected;

        public void Initialize(EnemyPrototype prototype, Sprite icon)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(PrototypeCardView)} has already been initialized.");
            }

            ValidateConfiguration();

            if (prototype == null)
            {
                throw new ArgumentNullException(nameof(prototype));
            }

            if (icon == null)
            {
                throw new ArgumentNullException(nameof(icon));
            }

            _prototypeId = prototype.Id;

            _icon.sprite = icon;
            _nameLabel.text = prototype.DisplayName;

            _statsLabel.text =
                $"HP {prototype.MaxHealth}   SPD {prototype.MoveSpeed:0.#}\n" +
                $"DMG {prototype.Weapon.Damage}   CD {prototype.Weapon.Cooldown:0.##}s";

            _button.onClick.AddListener(HandleClicked);

            _isInitialized = true;
        }

        private void HandleClicked()
        {
            Selected?.Invoke(_prototypeId);
        }

        private void OnDestroy()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(HandleClicked);
            }
        }

        private void ValidateConfiguration()
        {
            if (_button == null)
            {
                throw new InvalidOperationException($"{nameof(PrototypeCardView)} requires a Button reference.");
            }

            if (_icon == null)
            {
                throw new InvalidOperationException($"{nameof(PrototypeCardView)} requires an Icon reference.");
            }

            if (_nameLabel == null)
            {
                throw new InvalidOperationException($"{nameof(PrototypeCardView)} requires a Name Label reference.");
            }

            if (_statsLabel == null)
            {
                throw new InvalidOperationException($"{nameof(PrototypeCardView)} requires a Stats Label reference.");
            }
        }
    }
}