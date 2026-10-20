using System;
using System.Collections.Generic;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Brain.Runtime;
using UnityEngine;

namespace CursedForest.Unity.Brain.Definitions.Conditions
{
    public abstract class ConditionDefinition : ScriptableObject
    {
        internal ICondition<AgentContext> CreateRuntimeCondition(
            AgentRuntimeDependencies dependencies,
            HashSet<ConditionDefinition> activeDefinitions)
        {
            if (dependencies == null)
            {
                throw new ArgumentNullException(nameof(dependencies));
            }

            if (activeDefinitions == null)
            {
                throw new ArgumentNullException(nameof(activeDefinitions));
            }

            if (!activeDefinitions.Add(this))
            {
                throw new InvalidOperationException($"Cyclic condition reference detected at '{name}'.");
            }

            try
            {
                ICondition<AgentContext> condition = BuildCondition(
                    dependencies,
                    child =>
                    {
                        if (child == null)
                        {
                            throw new InvalidOperationException($"Condition '{name}' has an unassigned child.");
                        }

                        return child.CreateRuntimeCondition(dependencies, activeDefinitions);
                    });

                if (condition == null)
                {
                    throw new InvalidOperationException($"Condition '{name}' returned null.");
                }

                return condition;
            }
            finally
            {
                activeDefinitions.Remove(this);
            }
        }

        protected abstract ICondition<AgentContext> BuildCondition(
            AgentRuntimeDependencies dependencies,
            Func<ConditionDefinition, ICondition<AgentContext>> buildChild);
    }
}
