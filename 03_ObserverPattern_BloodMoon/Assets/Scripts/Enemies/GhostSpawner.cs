using System;
using System.Collections.Generic;
using BloodMoon.Core;
using BloodMoon.Pooling;
using UnityEngine;

namespace BloodMoon.Enemies
{
    public sealed class GhostSpawner : MonoBehaviour, IBloodMoonObserver, IDisposable
    {
        [SerializeField] private Ghost _ghostPrefab;
        [SerializeField] private Transform _poolContainer;
        [SerializeField] private Transform[] _spawnPoints;

        private readonly List<Ghost> _activeGhosts = new();

        private PrefabPool<Ghost> _ghostPool;
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

            _ghostPool = new PrefabPool<Ghost>(_ghostPrefab, _poolContainer);
            _ghostPool.Prewarm(_spawnPoints.Length);

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

            if (_ghostPool != null)
            {
                CleanupWave();
                _ghostPool.Dispose();
                _ghostPool = null;
            }

            _isDisposed = true;
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void SpawnWave()
        {
            _isWaveActive = true;

            try
            {
                for (int i = 0; i < _spawnPoints.Length; i++)
                {
                    Transform spawnPoint = _spawnPoints[i];

                    Ghost ghost = _ghostPool.Rent(spawnPoint.position, spawnPoint.rotation);

                    ghost.SetTarget(_target);
                    ghost.Died += HandleGhostDied;

                    _activeGhosts.Add(ghost);
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

        private void HandleGhostDied(Ghost ghost)
        {
            if (!_activeGhosts.Remove(ghost))
            {
                throw new InvalidOperationException($"{nameof(GhostSpawner)} received a death event from an untracked Ghost.");
            }

            ghost.Died -= HandleGhostDied;

            _ghostPool.Return(ghost);

            GhostDefeated?.Invoke();
        }

        private void CleanupWave()
        {
            for (int i = _activeGhosts.Count - 1; i >= 0; i--)
            {
                Ghost ghost = _activeGhosts[i];

                if (ghost == null)
                {
                    continue;
                }

                ghost.Died -= HandleGhostDied;
                _ghostPool.Return(ghost);
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
            if (_ghostPrefab == null)
            {
                throw new InvalidOperationException($"{nameof(GhostSpawner)} requires a Ghost prefab.");
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