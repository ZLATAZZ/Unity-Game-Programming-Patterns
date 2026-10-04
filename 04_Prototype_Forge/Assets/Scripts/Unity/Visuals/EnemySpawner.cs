using System;
using PrototypeForge.Core.Enemies;
using PrototypeForge.Core.Prototypes;
using UnityEngine;

namespace PrototypeForge.Unity.Visuals
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;

        private PrototypeRegistry<EnemyPrototype> _registry;
        private EnemyVisualCatalog _visualCatalog;
        private EnemyActor _activeActor;

        private bool _isInitialized;

        public void Initialize(PrototypeRegistry<EnemyPrototype> registry, EnemyVisualCatalog visualCatalog)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(EnemySpawner)} has already been initialized.");
            }

            if (_spawnPoint == null)
            {
                throw new InvalidOperationException($"{nameof(EnemySpawner)} requires a spawn point.");
            }

            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _visualCatalog = visualCatalog != null ? visualCatalog : throw new ArgumentNullException(nameof(visualCatalog));

            _isInitialized = true;
        }

        public EnemyActor Spawn(PrototypeId id)
        {
            EnsureInitialized();

            ClearActive();

            EnemyPrototype clone = _registry.Create(id);
            EnemyVisualDefinition visual = _visualCatalog.Get(id);

            _activeActor = Instantiate(
                visual.Prefab,
                _spawnPoint.position,
                _spawnPoint.rotation);

            _activeActor.Initialize(clone);

            return _activeActor;
        }

        public void ClearActive()
        {
            if (_activeActor == null)
            {
                return;
            }

            Destroy(_activeActor.gameObject);
            _activeActor = null;
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(EnemySpawner)} has not been initialized.");
            }
        }
    }
}