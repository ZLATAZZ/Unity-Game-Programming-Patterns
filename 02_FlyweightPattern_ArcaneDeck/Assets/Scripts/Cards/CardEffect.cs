using UnityEngine;
using Patterns.Flyweight.Battle;

namespace Patterns.Flyweight.Cards
{ 
    public abstract class CardEffect : ScriptableObject
    {
       public abstract void Apply(BattleContext context, int amount);
    }
}