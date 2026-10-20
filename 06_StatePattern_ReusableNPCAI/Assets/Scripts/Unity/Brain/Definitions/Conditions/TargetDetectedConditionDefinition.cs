using System;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Brain.Runtime;
using UnityEngine;

namespace CursedForest.Unity.Brain.Definitions.Conditions
{
    [CreateAssetMenu(fileName = "TargetDetected", menuName = "Cursed Forest/AI/Conditions/Target Detected")]
    public sealed class TargetDetectedConditionDefinition : ConditionDefinition
    {
        protected override ICondition<AgentContext> BuildCondition(
            AgentRuntimeDependencies dependencies,
            Func<ConditionDefinition, ICondition<AgentContext>> buildChild)
        {
            return new TargetDetectedCondition();
        }
    }
}
