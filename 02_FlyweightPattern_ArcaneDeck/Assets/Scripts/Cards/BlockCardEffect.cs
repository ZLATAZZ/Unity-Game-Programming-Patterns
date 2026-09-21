using Patterns.Flyweight.Battle;
using UnityEngine;

namespace Patterns.Flyweight.Cards
{
    [CreateAssetMenu(fileName = "BlockCardEffect", menuName = "Patterns/Flyweight/Card Effects/Block")]
    public sealed class BlockCardEffect : CardEffect
    {
        public override void Apply(BattleContext context, int amount)
        {
            context.Player.AddBlock(amount);
        }
    }
}