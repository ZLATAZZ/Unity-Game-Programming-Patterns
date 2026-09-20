using System;
using System.Collections.Generic;
using Patterns.Flyweight.Cards;
using UnityEngine;

namespace Patterns.Flyweight.UI
{
    public sealed class HandView : MonoBehaviour
    {
        [SerializeField] private Transform _cardsRoot;
        [SerializeField] private CardView _cardPrefab;

        private readonly List<CardView> _cardViews = new();

        public event Action<CardInstance> CardSelected;

        public void Render(IReadOnlyList<CardInstance> cards, CardInstance selectedCard)
        {
            Clear();

            for (int i = 0; i < cards.Count; i++)
            {
                CardInstance card = cards[i];

                CardView view = Instantiate(_cardPrefab, _cardsRoot);

                view.Initialize(card, HandleCardSelected, i);

                view.SetSelected(ReferenceEquals(card, selectedCard));

                _cardViews.Add(view);
            }
        }

        public void SetSelected(CardInstance selectedCard)
        {
            foreach (CardView view in _cardViews)
            {
                view.SetSelected(ReferenceEquals(view.Card, selectedCard));
            }
        }

        public void SetInteractable(bool interactable)
        {
            foreach (CardView view in _cardViews)
            {
                view.SetInteractable(interactable);
            }
        }

        private void HandleCardSelected(CardInstance card)
        {
            CardSelected?.Invoke(card);
        }

        private void Clear()
        {
            foreach (CardView view in _cardViews)
            {
                if (view == null)
                {
                    continue;
                }

                view.gameObject.SetActive(false);
                Destroy(view.gameObject);
            }

            _cardViews.Clear();
        }
    }
}