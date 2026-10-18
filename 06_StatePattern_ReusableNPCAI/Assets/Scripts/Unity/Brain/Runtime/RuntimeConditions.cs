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

    internal sealed class EnergyBelowThresholdCondition : ICondition<AgentContext>
    {
        private readonly IAgentEnergy _energy;
        private readonly float _threshold;

        public EnergyBelowThresholdCondition(IAgentEnergy energy, float threshold)
        {
            _energy = energy ?? throw new ArgumentNullException(nameof(energy));
            ValidateThreshold(threshold);
            _threshold = threshold;
        }

        public bool Evaluate(AgentContext context)
        {
            return _energy.Current < _threshold;
        }

        private static void ValidateThreshold(float threshold)
        {
            if (float.IsNaN(threshold) || float.IsInfinity(threshold) || threshold < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(threshold));
            }
        }
    }

    internal sealed class EnergyAtLeastCondition : ICondition<AgentContext>
    {
        private readonly IAgentEnergy _energy;
        private readonly float _threshold;

        public EnergyAtLeastCondition(IAgentEnergy energy, float threshold)
        {
            _energy = energy ?? throw new ArgumentNullException(nameof(energy));

            if (float.IsNaN(threshold) || float.IsInfinity(threshold) || threshold < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(threshold));
            }

            _threshold = threshold;
        }

        public bool Evaluate(AgentContext context)
        {
            return _energy.Current >= _threshold;
        }
    }

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

    internal sealed class AnyCondition : ICondition<AgentContext>
    {
        private readonly ICondition<AgentContext>[] _children;

        public AnyCondition(ICondition<AgentContext>[] children)
        {
            _children = children ?? throw new ArgumentNullException(nameof(children));
        }

        public bool Evaluate(AgentContext context)
        {
            foreach (ICondition<AgentContext> child in _children)
            {
                if (child.Evaluate(context))
                {
                    return true;
                }
            }

            return false;
        }
    }

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
