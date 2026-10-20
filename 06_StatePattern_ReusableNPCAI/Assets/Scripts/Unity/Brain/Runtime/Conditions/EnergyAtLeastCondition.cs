using System;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Agents.Capabilities;
using UnityEngine;

namespace CursedForest.Unity.Brain.Runtime
{
    internal sealed class EnergyAtLeastCondition : ICondition<AgentContext>
    {
        private readonly IAgentEnergy _energy;
        private readonly float _threshold;

        public EnergyAtLeastCondition(IAgentEnergy energy, float threshold)
        {
            _energy = energy ?? throw new ArgumentNullException(nameof(energy));

            if (float.IsNaN(threshold) || float.IsInfinity(threshold) || threshold < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(threshold));
            }

            _threshold = threshold;
        }

        public bool Evaluate(AgentContext context)
        {
            return _energy.Current >= _threshold;
        }
    }
}
