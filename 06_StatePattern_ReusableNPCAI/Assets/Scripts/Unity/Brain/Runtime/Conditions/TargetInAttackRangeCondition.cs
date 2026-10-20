using System;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Agents.Capabilities;
using UnityEngine;

namespace CursedForest.Unity.Brain.Runtime
{
    internal sealed class TargetInAttackRangeCondition : ICondition<AgentContext>
    {
        private readonly IAgentCombat _combat;

        public TargetInAttackRangeCondition(IAgentCombat combat)
        {
            _combat = combat ?? throw new ArgumentNullException(nameof(combat));
        }

        public bool Evaluate(AgentContext context)
        {
            Transform target = context.Perception.CurrentTarget;
            return target != null && _combat.IsInRange(target);
        }
    }
}
