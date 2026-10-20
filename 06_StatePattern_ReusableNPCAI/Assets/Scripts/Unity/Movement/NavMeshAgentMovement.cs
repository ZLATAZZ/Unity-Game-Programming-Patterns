
using System;
using CursedForest.Unity.Agents.Capabilities;
using UnityEngine;
using UnityEngine.AI;

namespace CursedForest.Unity.Movement
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class NavMeshAgentMovement : MonoBehaviour, IAgentMovement
    {
        private NavMeshAgent _agent;
        private bool _hasDestination;

        public Vector3 Position => transform.position;

        public bool HasReachedDestination =>
            _hasDestination &&
            _agent.isOnNavMesh &&
            !_agent.pathPending &&
            _agent.pathStatus == NavMeshPathStatus.PathComplete &&
            _agent.remainingDistance <= _agent.stoppingDistance &&
            _agent.velocity.sqrMagnitude <= 0.01f;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public void MoveTo(Vector3 destination)
        {
            if (!_agent.isOnNavMesh)
            {
                throw new InvalidOperationException("NavMeshAgent is not placed on a NavMesh.");
            }

            _agent.isStopped = false;
            _hasDestination = _agent.SetDestination(destination);

            if (!_hasDestination)
            {
                _agent.ResetPath();
            }
        }

        public void Stop()
        {
            _hasDestination = false;

            if (!_agent.isOnNavMesh)
            {
                return;
            }

            _agent.isStopped = true;
            _agent.ResetPath();
        }
    }
}
