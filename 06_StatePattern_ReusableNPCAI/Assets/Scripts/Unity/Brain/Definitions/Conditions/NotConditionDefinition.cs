using System;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Brain.Runtime;
using UnityEngine;

namespace CursedForest.Unity.Brain.Definitions.Conditions
{
    [CreateAssetMenu(fileName = "NotCondition", menuName = "Cursed Forest/AI/Conditions/Not")]
    public sealed class NotConditionDefinition : ConditionDefinition
    {
        [SerializeField] private ConditionDefinition _child;

        protected override ICondition<AgentContext> BuildCondition(
            AgentRuntimeDependencies dependencies,
            Func<ConditionDefinition, ICondition<AgentContext>> buildChild)
        {
            return new NotCondition(buildChild(_child));
        }
    }
}
