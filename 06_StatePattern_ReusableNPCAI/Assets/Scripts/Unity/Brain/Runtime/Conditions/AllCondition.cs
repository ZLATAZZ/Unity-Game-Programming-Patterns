using System;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Agents.Capabilities;
using UnityEngine;

namespace CursedForest.Unity.Brain.Runtime
{
    internal sealed class AllCondition : ICondition<AgentContext>
    {
        private readonly ICondition<AgentContext>[] _children;

        public AllCondition(ICondition<AgentContext>[] children)
        {
            _children = children ?? throw new ArgumentNullException(nameof(children));
        }

        public bool Evaluate(AgentContext context)
        {
            foreach (ICondition<AgentContext> child in _children)
            {
                if (!child.Evaluate(context))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
