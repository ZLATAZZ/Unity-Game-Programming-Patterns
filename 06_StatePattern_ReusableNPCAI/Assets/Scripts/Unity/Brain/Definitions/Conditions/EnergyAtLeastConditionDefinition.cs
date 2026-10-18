using System;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Brain.Runtime;
using UnityEngine;

namespace CursedForest.Unity.Brain.Definitions.Conditions
{
    [CreateAssetMenu(fileName = "EnergyAtLeast", menuName = "Cursed Forest/AI/Conditions/Energy At Least")]
    public sealed class EnergyAtLeastConditionDefinition : ConditionDefinition
    {
        [SerializeField, Min(0f)] private float _threshold = 80f;

        protected override ICondition<AgentContext> BuildCondition(
            AgentRuntimeDependencies dependencies,
            Func<ConditionDefinition, ICondition<AgentContext>> buildChild)
        {
            if (dependencies.Energy == null)
            {
                throw new InvalidOperationException("EnergyAtLeast requires IAgentEnergy on its agent.");
            }

            return new EnergyAtLeastCondition(dependencies.Energy, _threshold);
        }
    }
}
