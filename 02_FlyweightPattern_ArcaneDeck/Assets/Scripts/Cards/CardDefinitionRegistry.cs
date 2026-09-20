using System;
using System.Collections.Generic;

namespace Patterns.Flyweight.Cards
{
    public sealed class CardDefinitionRegistry
    {
        private readonly Dictionary<CardId, CardDefinition> _definitions;

        public CardDefinitionRegistry(IReadOnlyList<CardDefinition> definitions)
        {
            if (definitions == null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            _definitions = new Dictionary<CardId, CardDefinition>(definitions.Count);

            foreach (CardDefinition definition in definitions)
            {
                if (definition == null)
                {
                    throw new ArgumentException("Card definitions cannot contain null values.", nameof(definitions));
                }

                if (!_definitions.TryAdd(definition.CardId, definition))
                {
                    throw new ArgumentException($"Duplicate card definition for {definition.CardId}.", nameof(definitions));
                }
            }
        }

        public CardDefinition Get(CardId cardId)
        {
            if(!_definitions.TryGetValue(cardId, out CardDefinition definition))
            {
                throw new KeyNotFoundException($"Card definition for {cardId} not found.");
            }

            return definition;
        }
    }
}