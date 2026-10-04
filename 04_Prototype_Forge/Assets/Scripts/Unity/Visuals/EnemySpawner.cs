using System;
using System.Collections.Generic;
using PrototypeForge.Core.Enemies;
using PrototypeForge.Core.Prototypes;
using UnityEngine;

namespace PrototypeForge.Unity.Visuals
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Transform _spawnOrigin;
        [SerializeField] private Vector3 _spawnSpacing = new(2.5f, 0f, 2.5f);
        [SerializeField, Min(1)] private int _columns = 4;

        private readonly List<EnemyActor> _spawnedActors = new();

        private PrototypeRegistry<EnemyPrototype> _registry;
        private EnemyVisualCatalog _visualCatalog;

        private bool _isInitialized;

        public void Initialize(PrototypeRegistry<EnemyPrototype> registry, EnemyVisualCatalog visualCatalog)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(EnemySpawner)} has already been initialized.");
            }

            if (_spawnOrigin == null)
            {
                throw new InvalidOperationException($"{nameof(EnemySpawner)} requires a Spawn Origin.");
            }

            if (_columns <= 0)
            {
                throw new InvalidOperationException($"{nameof(EnemySpawner)} columns must be greater than zero.");
            }

            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _visualCatalog = visualCatalog != null ? visualCatalog : throw new ArgumentNullException(nameof(visualCatalog));

            _isInitialized = true;
        }

        public EnemyActor Spawn(PrototypeId id)
        {
            EnsureInitialized();

            EnemyPrototype clone = _registry.Create(id);
            EnemyVisualDefinition visual = _visualCatalog.Get(id);

            Vector3 position = GetNextSpawnPosition();

            EnemyActor actor = Instantiate(
                visual.Prefab,
                position,
                _spawnOrigin.rotation);

            actor.Initialize(clone);

            _spawnedActors.Add(actor);

            return actor;
        }

        public void ClearAll()
        {
            for (int i = 0; i < _spawnedActors.Count; i++)
            {
                EnemyActor actor = _spawnedActors[i];

                if (actor != null)
                {
                    Destroy(actor.gameObject);
                }
            }

            _spawnedActors.Clear();
        }

        private Vector3 GetNextSpawnPosition()
        {
            int index = _spawnedActors.Count;

            int column = index % _columns;
            int row = index / _columns;

            return _spawnOrigin.position +
                   Vector3.right * (_spawnSpacing.x * column) +
                   Vector3.forward * (_spawnSpacing.z * row);
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(EnemySpawner)} has not been initialized.");
            }
        }

        private void OnDestroy()
        {
            ClearAll();
        }
    }
}