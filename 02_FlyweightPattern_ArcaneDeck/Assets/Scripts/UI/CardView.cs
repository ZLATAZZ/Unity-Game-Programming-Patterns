using System;
using Patterns.Flyweight.Cards;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Patterns.Flyweight.UI
{
    public sealed class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _artwork;

        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _effectText;
        [SerializeField] private TMP_Text _energyText;
        [SerializeField] private TMP_Text _upgradeText;

        [SerializeField] private CardViewAnimator _animator;

        private Action<CardInstance> _onSelected;

        public CardInstance Card { get; private set; }

        public void Initialize(CardInstance card, Action<CardInstance> onSelected, int index)
        {
            Card = card ?? throw new ArgumentNullException(nameof(card));
            _onSelected = onSelected ?? throw new ArgumentNullException(nameof(onSelected));

            CardDefinition definition = Card.Definition;

            _artwork.sprite = definition.Artwork;
            _nameText.text = definition.DisplayName;

            _effectText.text = $"{definition.CardEffect.DisplayName}: {Card.EffectiveValue}";

            _energyText.text = $"Energy: {definition.EnergyCost}";

            _upgradeText.text = Card.UpgradeLevel > 0 ? $"Upgrade +{Card.UpgradeLevel}" : "Base";

            _button.onClick.AddListener(HandleClicked);

            _animator.PlayAppear(index);
        }

        public void SetSelected(bool isSelected)
        {
            _animator.SetSelected(isSelected);
        }

        public void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _animator.SetHovered(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _animator.SetHovered(false);
        }

        private void HandleClicked()
        {
            _onSelected.Invoke(Card);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(HandleClicked);
        }
    }
}