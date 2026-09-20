using System;

namespace Patterns.Flyweight.Cards
{
    public sealed class CardInstanceFactory
    {
        private readonly CardDefinitionRegistry _registry;

        private int _nextInstanceId = 1;

        public CardInstanceFactory(CardDefinitionRegistry registry)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        public CardInstance Create(CardId cardId)
        {
            CardDefinition definition = _registry.Get(cardId);
            return new CardInstance(definition, _nextInstanceId++);
        }
    }
}