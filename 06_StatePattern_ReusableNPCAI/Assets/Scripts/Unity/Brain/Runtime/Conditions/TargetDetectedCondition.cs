using System;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Agents.Capabilities;
using UnityEngine;

namespace CursedForest.Unity.Brain.Runtime
{
    internal sealed class TargetDetectedCondition : ICondition<AgentContext>
    {
        public bool Evaluate(AgentContext context)
        {
            return context.Perception.CurrentTarget != null;
        }
    }
}
