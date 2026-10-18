using System;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Brain.Runtime;
using UnityEngine;

namespace CursedForest.Unity.Brain.Definitions.Conditions
{
    [CreateAssetMenu(fileName = "EnergyBelow", menuName = "Cursed Forest/AI/Conditions/Energy Below Threshold")]
    public sealed class EnergyBelowThresholdConditionDefinition : ConditionDefinition
    {
        [SerializeField, Min(0f)] private float _threshold = 20f;

        protected override ICondition<AgentContext> BuildCondition(
            AgentRuntimeDependencies dependencies,
            Func<ConditionDefinition, ICondition<AgentContext>> buildChild)
        {
            if (dependencies.Energy == null)
            {
                throw new InvalidOperationException("EnergyBelowThreshold requires IAgentEnergy on its agent.");
            }

            return new EnergyBelowThresholdCondition(dependencies.Energy, _threshold);
        }
    }
}
