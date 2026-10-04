using System;
using System.Collections.Generic;
using PrototypeForge.Core.Enemies;
using PrototypeForge.Core.Prototypes;
using UnityEngine;

namespace PrototypeForge.Unity.Visuals
{
    public sealed class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private Transform[] _spawnSlots;

        private readonly List<EnemyActor> _spawnedActors = new();

        private PrototypeRegistry<EnemyPrototype> _registry;
        private EnemyVisualCatalog _visualCatalog;

        private bool _isInitialized;

        public int SpawnedCount => _spawnedActors.Count;
        public int Capacity => _spawnSlots.Length;
        public bool HasCapacity => _isInitialized && _spawnedActors.Count < _spawnSlots.Length;

        public void Initialize(PrototypeRegistry<EnemyPrototype> registry, EnemyVisualCatalog visualCatalog)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(EnemySpawner)} has already been initialized.");
            }

            ValidateConfiguration();

            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _visualCatalog = visualCatalog != null ? visualCatalog : throw new ArgumentNullException(nameof(visualCatalog));

            _isInitialized = true;
        }

        public bool TrySpawn(PrototypeId id, out EnemyActor actor)
        {
            EnsureInitialized();

            if (!HasCapacity)
            {
                actor = null;
                return false;
            }

            EnemyPrototype clone = _registry.Create(id);
            EnemyVisualDefinition visual = _visualCatalog.Get(id);

            Transform slot = _spawnSlots[_spawnedActors.Count];

            actor = Instantiate(
                visual.Prefab,
                slot.position,
                slot.rotation);

            actor.Initialize(clone);

            _spawnedActors.Add(actor);

            return true;
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

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(EnemySpawner)} has not been initialized.");
            }
        }

        private void ValidateConfiguration()
        {
            if (_spawnSlots == null || _spawnSlots.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(EnemySpawner)} requires at least one spawn slot.");
            }

            for (int i = 0; i < _spawnSlots.Length; i++)
            {
                if (_spawnSlots[i] == null)
                {
                    throw new InvalidOperationException($"{nameof(EnemySpawner)} contains a null spawn slot at index {i}.");
                }
            }
        }

        private void OnDestroy()
        {
            ClearAll();
        }
    }
}