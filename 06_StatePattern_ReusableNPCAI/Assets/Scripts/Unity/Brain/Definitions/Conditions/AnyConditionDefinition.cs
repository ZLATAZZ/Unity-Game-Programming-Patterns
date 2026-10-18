using System;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Brain.Runtime;
using UnityEngine;

namespace CursedForest.Unity.Brain.Definitions.Conditions
{
    [CreateAssetMenu(fileName = "AnyConditions", menuName = "Cursed Forest/AI/Conditions/Any (OR)")]
    public sealed class AnyConditionDefinition : ConditionDefinition
    {
        [SerializeField] private ConditionDefinition[] _children = Array.Empty<ConditionDefinition>();

        protected override ICondition<AgentContext> BuildCondition(
            AgentRuntimeDependencies dependencies,
            Func<ConditionDefinition, ICondition<AgentContext>> buildChild)
        {
            if (_children == null || _children.Length == 0)
            {
                throw new InvalidOperationException($"Any condition '{name}' must have at least one child.");
            }

            ICondition<AgentContext>[] children = new ICondition<AgentContext>[_children.Length];

            for (int i = 0; i < _children.Length; i++)
            {
                children[i] = buildChild(_children[i]);
            }

            return new AnyCondition(children);
        }
    }
}
