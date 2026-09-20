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

        private readonly Dictionary<CardInstance, CardView> _activeViews = new();
        private readonly Stack<CardView> _pooledViews = new();

        private readonly List<CardInstance> _releaseBuffer = new();
        private readonly HashSet<CardInstance> _handBuffer = new();

        public event Action<CardInstance> CardSelected;

        public void Render(IReadOnlyList<CardInstance> cards, CardInstance selectedCard)
        {
            SynchronizeActiveViews(cards);

            for (int i = 0; i < cards.Count; i++)
            {
                CardInstance card = cards[i];

                if (_activeViews.TryGetValue(card, out CardView existingView))
                {
                    existingView.Refresh();
                    existingView.transform.SetSiblingIndex(i);
                    existingView.SetSelected(ReferenceEquals(card, selectedCard));

                    continue;
                }

                CardView newView = GetView();

                newView.transform.SetSiblingIndex(i);
                newView.Bind(card, HandleCardSelected, i);

                if (ReferenceEquals(card, selectedCard))
                {
                    newView.SetSelected(true);
                }

                _activeViews.Add(card, newView);
            }
        }

        public void SetSelected(CardInstance selectedCard)
        {
            foreach (KeyValuePair<CardInstance, CardView> pair in _activeViews)
            {
                pair.Value.SetSelected(
                    ReferenceEquals(pair.Key, selectedCard));
            }
        }

        public void SetInteractable(bool interactable)
        {
            foreach (CardView view in _activeViews.Values)
            {
                view.SetInteractable(interactable);
            }
        }

        private void SynchronizeActiveViews(IReadOnlyList<CardInstance> cards)
        {
            _handBuffer.Clear();

            foreach (CardInstance card in cards)
            {
                _handBuffer.Add(card);
            }

            _releaseBuffer.Clear();

            foreach (CardInstance activeCard in _activeViews.Keys)
            {
                if (!_handBuffer.Contains(activeCard))
                {
                    _releaseBuffer.Add(activeCard);
                }
            }

            foreach (CardInstance card in _releaseBuffer)
            {
                ReleaseView(card);
            }
        }

        private CardView GetView()
        {
            if (_pooledViews.Count > 0)
            {
                CardView pooledView = _pooledViews.Pop();
                pooledView.gameObject.SetActive(true);

                return pooledView;
            }

            return Instantiate(_cardPrefab, _cardsRoot);
        }

        private void ReleaseView(CardInstance card)
        {
            if (!_activeViews.Remove(card, out CardView view))
            {
                return;
            }

            view.Unbind();
            view.gameObject.SetActive(false);

            _pooledViews.Push(view);
        }

        private void HandleCardSelected(CardInstance card)
        {
            CardSelected?.Invoke(card);
        }
    }
}