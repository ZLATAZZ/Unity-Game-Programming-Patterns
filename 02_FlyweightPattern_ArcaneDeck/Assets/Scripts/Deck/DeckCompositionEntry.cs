using System;
using Patterns.Flyweight.Cards;
using UnityEngine;

namespace Patterns.Flyweight.Deck
{
    [Serializable]
    public sealed class DeckCompositionEntry
    {
        [SerializeField] private CardId _cardId;
        [SerializeField, Min(1)] private int _count = 1;

        public CardId CardId => _cardId;
        public int Count => _count;
    }
}