using System;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Agents.Capabilities;
using UnityEngine;

namespace CursedForest.Unity.Brain.Runtime
{
    internal sealed class EnergyBelowThresholdCondition : ICondition<AgentContext>
    {
        private readonly IAgentEnergy _energy;
        private readonly float _threshold;

        public EnergyBelowThresholdCondition(IAgentEnergy energy, float threshold)
        {
            _energy = energy ?? throw new ArgumentNullException(nameof(energy));
            ValidateThreshold(threshold);
            _threshold = threshold;
        }

        public bool Evaluate(AgentContext context)
        {
            return _energy.Current < _threshold;
        }

        private static void ValidateThreshold(float threshold)
        {
            if (float.IsNaN(threshold) || float.IsInfinity(threshold) || threshold < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(threshold));
            }
        }
    }
}
