using System;
using CursedForest.Core.FSM;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Agents.Capabilities;
using UnityEngine;

namespace CursedForest.Unity.Behaviors
{
    public sealed class AttackState : IState<AgentContext>
    {
        private readonly IAgentCombat _combat;

        public StateId Id { get; }

        public AttackState(StateId id, IAgentCombat combat)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException("State ID cannot be empty.", nameof(id));
            }

            _combat = combat ?? throw new ArgumentNullException(nameof(combat));
            Id = id;
        }

        public void Enter(AgentContext context)
        {
            context.Movement.Stop();
        }

        public void Tick(AgentContext context, float deltaTime)
        {
            Transform target = context.Perception.CurrentTarget;

            if (target == null || !_combat.IsInRange(target))
            {
                return;
            }

            _combat.TryAttack(target);
        }

        public void Exit(AgentContext context)
        {
        }
    }
}