using System;
using CursedForest.Core.FSM;
using CursedForest.Unity.Agents;
using UnityEngine;

namespace CursedForest.Unity.Behaviors
{
    public sealed class ChaseState : IState<AgentContext>
    {
        private readonly float _repathInterval;
        private readonly float _minimumTargetMovement;

        private float _repathTimer;
        private Vector3 _lastDestination;
        private Transform _lastTarget;
        private bool _hasDestination;

        public StateId Id { get; }

        public ChaseState(StateId id, float repathInterval, float minimumTargetMovement)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException("State ID cannot be empty.", nameof(id));
            }

            if (float.IsNaN(repathInterval) || float.IsInfinity(repathInterval) || repathInterval <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(repathInterval));
            }

            if (float.IsNaN(minimumTargetMovement) || float.IsInfinity(minimumTargetMovement) || minimumTargetMovement < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumTargetMovement));
            }

            Id = id;
            _repathInterval = repathInterval;
            _minimumTargetMovement = minimumTargetMovement;
        }

        public void Enter(AgentContext context)
        {
            _repathTimer = 0f;
            _hasDestination = false;
            _lastTarget = null;
            UpdateDestination(context);
        }

        public void Tick(AgentContext context, float deltaTime)
        {
            if (context.Perception.CurrentTarget == null)
            {
                context.Movement.Stop();
                _hasDestination = false;
                _lastTarget = null;
                return;
            }

            _repathTimer += deltaTime;

            if (_repathTimer < _repathInterval)
            {
                return;
            }

            _repathTimer = 0f;
            UpdateDestination(context);
        }

        public void Exit(AgentContext context)
        {
            context.Movement.Stop();
            _hasDestination = false;
            _lastTarget = null;
        }

        private void UpdateDestination(AgentContext context)
        {
            Transform target = context.Perception.CurrentTarget;

            if (target == null)
            {
                context.Movement.Stop();
                _hasDestination = false;
                _lastTarget = null;
                return;
            }

            Vector3 destination = target.position;
            float minimumDistanceSqr = _minimumTargetMovement * _minimumTargetMovement;

            if (_hasDestination && target == _lastTarget && (destination - _lastDestination).sqrMagnitude < minimumDistanceSqr)
            {
                return;
            }

            context.Movement.MoveTo(destination);
            _lastDestination = destination;
            _lastTarget = target;
            _hasDestination = true;
        }
    }
}