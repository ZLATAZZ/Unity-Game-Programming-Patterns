using UnityEngine;
using Patterns.Flyweight.Battle;

namespace Patterns.Flyweight.Cards
{ 
    public abstract class CardEffect : ScriptableObject
    {
        [SerializeField] private CardEffectType _displayName;

        public string DisplayName => _displayName.ToString();

        public abstract void Apply(BattleContext context, int amount);
    }
}