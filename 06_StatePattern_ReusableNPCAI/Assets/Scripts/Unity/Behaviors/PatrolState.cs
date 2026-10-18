
using System;
using System.Collections.Generic;
using CursedForest.Core.FSM;
using CursedForest.Unity.Agents;
using UnityEngine;

namespace CursedForest.Unity.Behaviors
{
    public sealed class PatrolState : IState<AgentContext>
    {
        private readonly Vector3[] _pathPoints;
        private int _currentPointIndex;

        public StateId Id { get; }

        public PatrolState(StateId id, IReadOnlyList<Vector3> pathPoints)
        {
            if (id.IsEmpty)
            {
                throw new ArgumentException("State ID cannot be empty.", nameof(id));
            }

            if (pathPoints == null || pathPoints.Count == 0)
            {
                throw new ArgumentException("Path points cannot be null or empty.", nameof(pathPoints));
            }

            Id = id;
            _pathPoints = new Vector3[pathPoints.Count];

            for (int i = 0; i < pathPoints.Count; i++)
            {
                _pathPoints[i] = pathPoints[i];
            }
        }

        public void Enter(AgentContext context)
        {
            _currentPointIndex = 0;
            context.Movement.MoveTo(_pathPoints[_currentPointIndex]);
        }

        public void Tick(AgentContext context, float deltaTime)
        {
            if (!context.Movement.HasReachedDestination)
            {
                return;
            }

            if (_pathPoints.Length == 1)
            {
                context.Movement.Stop();
                return;
            }

            _currentPointIndex = (_currentPointIndex + 1) % _pathPoints.Length;
            context.Movement.MoveTo(_pathPoints[_currentPointIndex]);
        }

        public void Exit(AgentContext context)
        {
            context.Movement.Stop();
        }
    }
}
