
using CursedForest.Unity.Agents.Capabilities;
using UnityEngine;

namespace CursedForest.Unity.Perception
{
    [DisallowMultipleComponent]
    public sealed class NearestTargetPerception : MonoBehaviour, IAgentPerception
    {
        private const int MaxDetectedColliders = 32;

        [SerializeField, Min(0.1f)] private float _detectionRadius = 12f;
        [SerializeField, Min(0.05f)] private float _scanInterval = 0.2f;
        [SerializeField] private LayerMask _targetLayers;
        [SerializeField] private LayerMask _obstacleLayers;

        private readonly Collider[] _results = new Collider[MaxDetectedColliders];
        private float _scanTimer;

        public Transform CurrentTarget { get; private set; }

        private void Update()
        {
            _scanTimer += Time.deltaTime;

            if (_scanTimer < _scanInterval)
            {
                return;
            }

            _scanTimer = 0f;
            Scan();
        }

        private void Scan()
        {
            CurrentTarget = null;

            Vector3 origin = transform.position + Vector3.up;

            int count = Physics.OverlapSphereNonAlloc(
                origin,
                _detectionRadius,
                _results,
                _targetLayers,
                QueryTriggerInteraction.Collide);

            float closestDistanceSqr = float.PositiveInfinity;

            for (int i = 0; i < count; i++)
            {
                Collider candidate = _results[i];

                if (candidate == null || candidate.transform.IsChildOf(transform))
                {
                    continue;
                }

                Vector3 targetPosition = candidate.bounds.center;
                float distanceSqr = (targetPosition - origin).sqrMagnitude;

                if (distanceSqr >= closestDistanceSqr)
                {
                    continue;
                }

                if (Physics.Linecast(origin, targetPosition, _obstacleLayers, QueryTriggerInteraction.Ignore))
                {
                    continue;
                }

                closestDistanceSqr = distanceSqr;
                CurrentTarget = candidate.transform;
            }
        }

        private void OnDisable()
        {
            CurrentTarget = null;
            _scanTimer = 0f;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + Vector3.up, _detectionRadius);
        }
    }
}
