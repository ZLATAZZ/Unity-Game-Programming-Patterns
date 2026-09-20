using System;
using DG.Tweening;
using Patterns.Flyweight.Cards;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Patterns.Flyweight.UI
{
    public sealed class CardDetailsView : MonoBehaviour
    {
        [SerializeField] private GameObject _contentRoot;

        [SerializeField] private Image _artwork;

        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _effectText;
        [SerializeField] private TMP_Text _energyText;
        [SerializeField] private TMP_Text _upgradeText;

        [SerializeField] private Button _playButton;
        [SerializeField] private TMP_Text _playButtonText;

        public event Action PlayRequested;

        private void Awake()
        {
            _playButton.onClick.AddListener(HandlePlayRequested);

            Clear();
        }

        public void Show(CardInstance card, bool canPlay)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            CardDefinition definition = card.Definition;

            _contentRoot.SetActive(true);

            _artwork.sprite = definition.Artwork;
            _nameText.text = definition.DisplayName;
            _descriptionText.text = definition.Description;

            _effectText.text = $"{definition.CardEffect.DisplayName}: {card.EffectiveValue}";

            _energyText.text = $"Energy Cost: {definition.EnergyCost}";

            _upgradeText.text = $"Upgrade Level: {card.UpgradeLevel}/{definition.MaxUpgradeLevel}";

            _playButtonText.text = $"PLAY • {definition.EnergyCost} ENERGY";

            _playButton.interactable = canPlay;
        }

        public void Clear()
        {
            _contentRoot.SetActive(false);
        }

        public void PlayRejectedFeedback()
        {
            _playButton.transform.DOKill();

            _playButton.transform.DOPunchScale(Vector3.one * 0.1f, 0.25f, 8, 0.5f);
        }

        private void HandlePlayRequested()
        {
            PlayRequested?.Invoke();
        }

        private void OnDestroy()
        {
            _playButton.onClick.RemoveListener(HandlePlayRequested);
        }
    }
}