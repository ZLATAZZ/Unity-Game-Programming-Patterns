using Patterns.Flyweight.Cards;
using System;
using System.Collections.Generic;

namespace Patterns.Flyweight.Deck
{
    public sealed class DeckState
    {
        private readonly List<CardInstance> _drawPile;
        private readonly List<CardInstance> _hand;
        private readonly List<CardInstance> _discardPile;
        private readonly Random _random;

        public IReadOnlyList<CardInstance> DrawPile => _drawPile;
        public IReadOnlyList<CardInstance> Hand => _hand;
        public IReadOnlyList<CardInstance> DiscardPile => _discardPile;

        public DeckState(CardInstance[] initialDrawPile, Random random)
        {
            if (initialDrawPile == null)
            {
                throw new ArgumentNullException(nameof(initialDrawPile));
            }

            _random = random ?? throw new ArgumentNullException(nameof(random));

            foreach (CardInstance card in initialDrawPile)
            {
                if (card == null)
                {
                    throw new ArgumentException("Initial draw pile cannot contain null cards.", nameof(initialDrawPile));
                }
            }

            _drawPile = new List<CardInstance>(initialDrawPile);
            _hand = new List<CardInstance>();
            _discardPile = new List<CardInstance>();

            ShuffleDrawPile();
        }

        public CardInstance DrawCard()
        {
            if (_drawPile.Count == 0)
            {
                ReshuffleDiscardPile();

                if (_drawPile.Count == 0)
                {
                    throw new InvalidOperationException("Cannot draw a card because both draw and discard piles are empty.");
                }
            }

            int topIndex = _drawPile.Count - 1;
            CardInstance drawnCard = _drawPile[topIndex];

            _drawPile.RemoveAt(topIndex);
            _hand.Add(drawnCard);

            return drawnCard;
        }

        public void Discard(CardInstance card)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            if (!_hand.Remove(card))
            {
                throw new InvalidOperationException("Cannot discard a card that is not in hand.");
            }

            _discardPile.Add(card);
        }

        public void DiscardHand()
        {
            _discardPile.AddRange(_hand);
            _hand.Clear();
        }

        public bool IsInHand(CardInstance card)
        {
            if (card == null)
            {
                return false;
            }

            return _hand.Contains(card);
        }

        private void ReshuffleDiscardPile()
        {
            _drawPile.AddRange(_discardPile);
            _discardPile.Clear();

            ShuffleDrawPile();
        }

        private void ShuffleDrawPile()
        {
            for (int i = _drawPile.Count - 1; i > 0; i--)
            {
                int j = _random.Next(0, i + 1);

                CardInstance temp = _drawPile[i];
                _drawPile[i] = _drawPile[j];
                _drawPile[j] = temp;
            }
        }
    }
}