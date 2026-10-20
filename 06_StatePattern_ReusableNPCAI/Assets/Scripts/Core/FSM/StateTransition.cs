
using System;

namespace CursedForest.Core.FSM
{
    public sealed class StateTransition<TContext>
    {
        private readonly Func<TContext, bool> _condition;

        public StateId From { get; }
        public StateId To { get; }

        public int Priority { get; }

        public StateTransition(StateId from, StateId to, int priority, Func<TContext, bool> condition)
        {
            if (from.IsEmpty)
            {
                throw new ArgumentException("Source state ID cannot be empty.", nameof(from));
            }

            if (to.IsEmpty)
            {
                throw new ArgumentException("Target state ID cannot be empty.", nameof(to));
            }

            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            if (priority < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(priority), "Priority must be non-negative.");
            }

            From = from;
            To = to;
            Priority = priority;
            _condition = condition;
        }

        public bool IsSatisfied(TContext context)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return _condition(context);
        }
    }
}
