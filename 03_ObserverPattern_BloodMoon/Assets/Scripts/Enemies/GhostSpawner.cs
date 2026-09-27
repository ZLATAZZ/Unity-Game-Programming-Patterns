using System;
using System.Collections.Generic;
using BloodMoon.Core;
using BloodMoon.Pooling;
using UnityEngine;

namespace BloodMoon.Enemies
{
    public sealed class GhostSpawner : MonoBehaviour, IBloodMoonObserver, IDisposable
    {
        [SerializeField] private Ghost[] _ghostPrefabs;
        [SerializeField] private Transform _poolContainer;
        [SerializeField] private Transform[] _spawnPoints;

        private readonly List<PrefabPool<Ghost>> _ghostPools = new();
        private readonly Dictionary<Ghost, PrefabPool<Ghost>> _activeGhosts = new();

        private Transform _target;

        private bool _isInitialized;
        private bool _isWaveActive;
        private bool _isDisposed;

        public event Action<int> WaveStarted;
        public event Action GhostDefeated;

        public void Initialize(Transform target)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(GhostSpawner)} has already been initialized.");
            }

            ValidateConfiguration();

            _target = target != null ? target : throw new ArgumentNullException(nameof(target));

            CreatePools();
            PrewarmPools();

            _isInitialized = true;
        }

        public void OnBloodMoonStarted()
        {
            EnsureReady();

            if (_isWaveActive)
            {
                return;
            }

            SpawnWave();
        }

        public void OnBloodMoonEnded()
        {
            EnsureReady();

            CleanupWave();
            _isWaveActive = false;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            CleanupWave();

            for (int i = 0; i < _ghostPools.Count; i++)
            {
                _ghostPools[i].Dispose();
            }

            _ghostPools.Clear();

            _isDisposed = true;
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void CreatePools()
        {
            for (int i = 0; i < _ghostPrefabs.Length; i++)
            {
                PrefabPool<Ghost> pool = new(_ghostPrefabs[i], _poolContainer);

                _ghostPools.Add(pool);
            }
        }

        private void PrewarmPools()
        {
            int baseCount = _spawnPoints.Length / _ghostPools.Count;
            int remainder = _spawnPoints.Length % _ghostPools.Count;

            for (int i = 0; i < _ghostPools.Count; i++)
            {
                int prewarmCount = baseCount;

                if (i < remainder)
                {
                    prewarmCount++;
                }

                _ghostPools[i].Prewarm(prewarmCount);
            }
        }

        private void SpawnWave()
        {
            _isWaveActive = true;

            try
            {
                for (int i = 0; i < _spawnPoints.Length; i++)
                {
                    Transform spawnPoint = _spawnPoints[i];

                    PrefabPool<Ghost> pool = GetPoolForSpawnPoint(i);

                    Ghost ghost = pool.Rent(
                        spawnPoint.position,
                        spawnPoint.rotation);

                    ghost.SetTarget(_target);
                    ghost.Died += HandleGhostDied;

                    _activeGhosts.Add(ghost, pool);
                }
            }
            catch
            {
                CleanupWave();
                _isWaveActive = false;

                throw;
            }

            WaveStarted?.Invoke(_activeGhosts.Count);
        }

        private PrefabPool<Ghost> GetPoolForSpawnPoint(int spawnPointIndex)
        {
            int poolIndex = spawnPointIndex % _ghostPools.Count;

            return _ghostPools[poolIndex];
        }

        private void HandleGhostDied(Ghost ghost)
        {
            if (!_activeGhosts.Remove(ghost, out PrefabPool<Ghost> pool))
            {
                throw new InvalidOperationException($"{nameof(GhostSpawner)} received a death event from an untracked Ghost.");
            }

            ghost.Died -= HandleGhostDied;

            pool.Return(ghost);

            GhostDefeated?.Invoke();
        }

        private void CleanupWave()
        {
            foreach (KeyValuePair<Ghost, PrefabPool<Ghost>> pair in _activeGhosts)
            {
                Ghost ghost = pair.Key;
                PrefabPool<Ghost> pool = pair.Value;

                if (ghost == null)
                {
                    continue;
                }

                ghost.Died -= HandleGhostDied;
                pool.Return(ghost);
            }

            _activeGhosts.Clear();
        }

        private void EnsureReady()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(GhostSpawner)} has not been initialized.");
            }

            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(GhostSpawner));
            }
        }

        private void ValidateConfiguration()
        {
            if (_ghostPrefabs == null || _ghostPrefabs.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(GhostSpawner)} requires at least one Ghost prefab.");
            }

            for (int i = 0; i < _ghostPrefabs.Length; i++)
            {
                if (_ghostPrefabs[i] == null)
                {
                    throw new InvalidOperationException($"{nameof(GhostSpawner)} contains a null Ghost prefab at index {i}.");
                }
            }

            if (_poolContainer == null)
            {
                throw new InvalidOperationException($"{nameof(GhostSpawner)} requires a Pool Container reference.");
            }

            if (_spawnPoints == null || _spawnPoints.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(GhostSpawner)} requires at least one spawn point.");
            }

            for (int i = 0; i < _spawnPoints.Length; i++)
            {
                if (_spawnPoints[i] == null)
                {
                    throw new InvalidOperationException($"{nameof(GhostSpawner)} contains a null spawn point at index {i}.");
                }
            }
        }
    }
}