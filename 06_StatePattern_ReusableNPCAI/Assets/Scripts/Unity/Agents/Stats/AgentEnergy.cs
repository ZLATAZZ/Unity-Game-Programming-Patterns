using System;
using CursedForest.Unity.Agents.Capabilities;

namespace CursedForest.Unity.Agents.Stats
{
    public sealed class AgentEnergy : IAgentEnergy
    {
        public float Current { get; private set; }
        public float Maximum { get; }

        public AgentEnergy(float maximum) : this(maximum, maximum)
        {
        }

        public AgentEnergy(float maximum, float initial)
        {
            if (float.IsNaN(maximum) || float.IsInfinity(maximum) || maximum <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(maximum), "Maximum energy must be finite and positive.");
            }

            if (float.IsNaN(initial) || float.IsInfinity(initial) || initial < 0f || initial > maximum)
            {
                throw new ArgumentOutOfRangeException(nameof(initial), "Initial energy must be between zero and maximum.");
            }

            Maximum = maximum;
            Current = initial;
        }

        public void Recover(float amount)
        {
            ValidateAmount(amount);
            Current = Math.Min(Maximum, Current + amount);
        }

        public void Spend(float amount)
        {
            ValidateAmount(amount);
            Current = Math.Max(0f, Current - amount);
        }

        private static void ValidateAmount(float amount)
        {
            if (float.IsNaN(amount) || float.IsInfinity(amount) || amount < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Energy amount must be finite and non-negative.");
            }
        }
    }
}