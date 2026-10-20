using System;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Agents.Capabilities;
using UnityEngine;

namespace CursedForest.Unity.Brain.Runtime
{
    internal sealed class NotCondition : ICondition<AgentContext>
    {
        private readonly ICondition<AgentContext> _child;

        public NotCondition(ICondition<AgentContext> child)
        {
            _child = child ?? throw new ArgumentNullException(nameof(child));
        }

        public bool Evaluate(AgentContext context)
        {
            return !_child.Evaluate(context);
        }
    }
}
