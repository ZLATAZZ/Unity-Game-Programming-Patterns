using Patterns.Flyweight.Battle;
using UnityEngine;

namespace Patterns.Flyweight.Cards
{
    [CreateAssetMenu(fileName = "DamageCardEffect", menuName = "Patterns/Flyweight/Card Effects/Damage")]
    public sealed class DamageCardEffect : CardEffect
    {
        public override void Apply(BattleContext context, int amount)
        {
            context.Enemy.TakeDamage(amount);
        }
    }
}