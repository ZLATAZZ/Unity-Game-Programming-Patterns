using System;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Brain.Runtime;
using UnityEngine;

namespace CursedForest.Unity.Brain.Definitions.Conditions
{
    [CreateAssetMenu(fileName = "AllConditions", menuName = "Cursed Forest/AI/Conditions/All (AND)")]
    public sealed class AllConditionDefinition : ConditionDefinition
    {
        [SerializeField] private ConditionDefinition[] _children = Array.Empty<ConditionDefinition>();

        protected override ICondition<AgentContext> BuildCondition(
            AgentRuntimeDependencies dependencies,
            Func<ConditionDefinition, ICondition<AgentContext>> buildChild)
        {
            if (_children == null || _children.Length == 0)
            {
                throw new InvalidOperationException($"All condition '{name}' must have at least one child.");
            }

            ICondition<AgentContext>[] children = new ICondition<AgentContext>[_children.Length];

            for (int i = 0; i < _children.Length; i++)
            {
                children[i] = buildChild(_children[i]);
            }

            return new AllCondition(children);
        }
    }
}
