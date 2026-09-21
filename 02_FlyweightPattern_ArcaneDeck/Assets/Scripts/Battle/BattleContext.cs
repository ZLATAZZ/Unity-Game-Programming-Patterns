using System;
using Patterns.Flyweight.Enemy;
using Patterns.Flyweight.Player;

namespace Patterns.Flyweight.Battle
{
    public sealed class BattleContext
    {
        public PlayerState Player { get; }
        public EnemyState Enemy { get; }

        public BattleContext(PlayerState player, EnemyState enemy)
        {
            Player = player ?? throw new ArgumentNullException(nameof(player));
            Enemy = enemy ?? throw new ArgumentNullException(nameof(enemy));
        }
    }
}