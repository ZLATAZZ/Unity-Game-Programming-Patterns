using System;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Brain.Runtime;
using UnityEngine;

namespace CursedForest.Unity.Brain.Definitions.Conditions
{
    [CreateAssetMenu(fileName = "TargetInAttackRange", menuName = "Cursed Forest/AI/Conditions/Target In Attack Range")]
    public sealed class TargetInAttackRangeConditionDefinition : ConditionDefinition
    {
        protected override ICondition<AgentContext> BuildCondition(
            AgentRuntimeDependencies dependencies,
            Func<ConditionDefinition, ICondition<AgentContext>> buildChild)
        {
            if (dependencies.Combat == null)
            {
                throw new InvalidOperationException("TargetInAttackRange requires IAgentCombat on its agent.");
            }

            return new TargetInAttackRangeCondition(dependencies.Combat);
        }
    }
}
