using System;
using CursedForest.Unity.Agents.Capabilities;
using UnityEngine;
using UnityEngine.AI;

namespace CursedForest.Unity.Navigation
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(NavMeshAgent))]
    public sealed class NavMeshEscapePlanner : MonoBehaviour, IEscapePlanner
    {
        private static readonly float[] SearchAngles = { 0f, -35f, 35f, -70f, 70f };

        [SerializeField, Min(0.1f)] private float _sampleRadius = 1.5f;
        [SerializeField, Min(0f)] private float _minimumSafetyGain = 0.25f;

        private NavMeshAgent _agent;
        private NavMeshPath _path;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _path = new NavMeshPath();
        }

        public bool TryPlanEscape(Vector3 position, Vector3 threatPosition, float distance, out Vector3 destination)
        {
            destination = default;

            if (float.IsNaN(distance) || float.IsInfinity(distance) || distance <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(distance), "Escape distance must be finite and positive.");
            }

            if (!_agent.isOnNavMesh)
            {
                return false;
            }

            Vector3 awayDirection = position - threatPosition;
            awayDirection.y = 0f;

            if (awayDirection.sqrMagnitude < 0.0001f)
            {
                awayDirection = transform.forward;
                awayDirection.y = 0f;
            }

            if (awayDirection.sqrMagnitude < 0.0001f)
            {
                awayDirection = Vector3.forward;
            }

            awayDirection.Normalize();

            float currentThreatDistance = HorizontalDistance(position, threatPosition);
            float bestThreatDistance = currentThreatDistance + _minimumSafetyGain;
            bool foundDestination = false;

            foreach (float angle in SearchAngles)
            {
                Vector3 direction = Quaternion.Euler(0f, angle, 0f) * awayDirection;
                Vector3 proposedPosition = position + direction * distance;

                if (!NavMesh.SamplePosition(proposedPosition, out NavMeshHit hit, _sampleRadius, _agent.areaMask))
                {
                    continue;
                }

                float threatDistance = HorizontalDistance(hit.position, threatPosition);

                if (threatDistance <= bestThreatDistance)
                {
                    continue;
                }

                if (!_agent.CalculatePath(hit.position, _path))
                {
                    continue;
                }

                if (_path.status != NavMeshPathStatus.PathComplete)
                {
                    continue;
                }

                bestThreatDistance = threatDistance;
                destination = hit.position;
                foundDestination = true;
            }

            return foundDestination;
        }

        private static float HorizontalDistance(Vector3 first, Vector3 second)
        {
            float differenceX = first.x - second.x;
            float differenceZ = first.z - second.z;

            return Mathf.Sqrt(differenceX * differenceX + differenceZ * differenceZ);
        }
    }
}