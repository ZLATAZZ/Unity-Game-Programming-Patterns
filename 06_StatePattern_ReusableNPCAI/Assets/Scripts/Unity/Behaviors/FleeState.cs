using System;
using CursedForest.Core.FSM;
using CursedForest.Unity.Agents;
using CursedForest.Unity.Agents.Capabilities;
using UnityEngine;

namespace CursedForest.Unity.Behaviors
{
    public sealed class FleeState : IState<AgentContext>
    {
        private readonly IEscapePlanner _escapePlanner;
        private readonly float _escapeDistance;
        private readonly float _repathInterval;

        private float _repathTimer;

        public StateId Id { get; }

        public FleeState(StateId id, IEscapePlanner escapePlanner, float escapeDistance, float repathInterval)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException("State ID cannot be empty.", nameof(id));
            }

            _escapePlanner = escapePlanner ?? throw new ArgumentNullException(nameof(escapePlanner));

            if (float.IsNaN(escapeDistance) || float.IsInfinity(escapeDistance) || escapeDistance <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(escapeDistance));
            }

            if (float.IsNaN(repathInterval) || float.IsInfinity(repathInterval) || repathInterval <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(repathInterval));
            }

            Id = id;
            _escapeDistance = escapeDistance;
            _repathInterval = repathInterval;
        }

        public void Enter(AgentContext context)
        {
            _repathTimer = 0f;
            UpdateEscapeDestination(context);
        }

        public void Tick(AgentContext context, float deltaTime)
        {
            if (context.Perception.CurrentTarget == null)
            {
                context.Movement.Stop();
                return;
            }

            _repathTimer += deltaTime;

            if (_repathTimer < _repathInterval)
            {
                return;
            }

            _repathTimer = 0f;
            UpdateEscapeDestination(context);
        }

        public void Exit(AgentContext context)
        {
            context.Movement.Stop();
        }

        private void UpdateEscapeDestination(AgentContext context)
        {
            Transform threat = context.Perception.CurrentTarget;

            if (threat == null)
            {
                context.Movement.Stop();
                return;
            }

            if (_escapePlanner.TryPlanEscape(context.Movement.Position, threat.position, _escapeDistance, out Vector3 destination))
            {
                context.Movement.MoveTo(destination);
            }
            else
            {
                context.Movement.Stop();
            }
        }
    }
}