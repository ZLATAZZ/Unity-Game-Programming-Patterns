using System;
using CursedForest.Core.FSM;
using CursedForest.Unity.Agents;

namespace CursedForest.Unity.Brain.Runtime
{
    public sealed class BrainRuntime
    {
        public StateMachine<AgentContext> StateMachine { get; }
        public StateId InitialStateId { get; }

        public BrainRuntime(StateMachine<AgentContext> stateMachine, StateId initialStateId)
        {
            StateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));

            if (initialStateId.IsEmpty)
            {
                throw new ArgumentException("Initial state ID cannot be empty.", nameof(initialStateId));
            }

            InitialStateId = initialStateId;
        }
    }
}
