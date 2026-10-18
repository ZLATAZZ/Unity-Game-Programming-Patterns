using System;
using CursedForest.Core.FSM;
using UnityEngine;
using Zenject;

namespace CursedForest.Unity.Agents
{
    public sealed class AgentBrain : IInitializable, ITickable
    {
        private readonly StateMachine<AgentContext> _stateMachine;
        private readonly StateId _initialStateId;

        public AgentBrain(StateMachine<AgentContext> stateMachine, StateId initialStateId)
        {
            _stateMachine = stateMachine ?? throw new ArgumentNullException(nameof(stateMachine));

            if (initialStateId.IsEmpty)
            {
                throw new ArgumentException("Initial state ID cannot be empty.", nameof(initialStateId));
            }

            _initialStateId = initialStateId;
        }

        public void Initialize()
        {
            _stateMachine.Start(_initialStateId);
        }

        public void Tick()
        {
            _stateMachine.Tick(Time.deltaTime);
        }
    }
}
