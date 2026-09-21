using System;

namespace Patterns.Flyweight.Cards
{
    public sealed class CardInstance
    {
        public CardDefinition Definition { get;}
        public int InstanceId { get; }

        public int UpgradeLevel { get; private set; }

        public int EffectiveValue => Definition.BaseEffectValue + UpgradeLevel * Definition.UpgradeValue;

        public CardInstance(CardDefinition definition, int instanceId)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
            
            if (instanceId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(instanceId));
            }
            InstanceId = instanceId;
        }

        public bool TryUpgrade()
        {
            if(UpgradeLevel >= Definition.MaxUpgradeLevel)
            {
                return false;
            }

            UpgradeLevel++;
            return true;
        }
    }
}