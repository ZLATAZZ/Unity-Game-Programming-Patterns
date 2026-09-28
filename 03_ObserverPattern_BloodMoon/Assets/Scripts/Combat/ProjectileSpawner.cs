using System;
using BloodMoon.Pooling;
using UnityEngine;

namespace BloodMoon.Combat
{
    public sealed class ProjectileSpawner : MonoBehaviour, IDisposable
    {
        [SerializeField] private Projectile _projectilePrefab;
        [SerializeField] private Transform _poolContainer;
        [SerializeField, Min(0)] private int _prewarmCount = 12;

        private PrefabPool<Projectile> _projectilePool;

        private bool _isInitialized;
        private bool _isDisposed;

        public void Initialize()
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(ProjectileSpawner)} has already been initialized.");
            }

            ValidateConfiguration();

            _projectilePool = new PrefabPool<Projectile>(_projectilePrefab, _poolContainer);
            _projectilePool.Prewarm(_prewarmCount);

            _isInitialized = true;
        }

        public void Fire(Vector3 position, Quaternion rotation)
        {
            EnsureReady();

            Projectile projectile = _projectilePool.Rent(position, rotation);

            projectile.Finished += HandleProjectileFinished;

            try
            {
                projectile.Launch(rotation * Vector3.forward);
            }
            catch
            {
                projectile.Finished -= HandleProjectileFinished;
                _projectilePool.Return(projectile);
                throw;
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _projectilePool?.Dispose();
            _projectilePool = null;

            _isDisposed = true;
        }

        private void OnDestroy()
        {
            Dispose();
        }

        private void HandleProjectileFinished(Projectile projectile)
        {
            projectile.Finished -= HandleProjectileFinished;
            _projectilePool.Return(projectile);
        }

        private void EnsureReady()
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(ProjectileSpawner)} has not been initialized.");
            }

            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(ProjectileSpawner));
            }
        }

        private void ValidateConfiguration()
        {
            if (_projectilePrefab == null)
            {
                throw new InvalidOperationException($"{nameof(ProjectileSpawner)} requires a Projectile prefab.");
            }

            if (_poolContainer == null)
            {
                throw new InvalidOperationException($"{nameof(ProjectileSpawner)} requires a Pool Container reference.");
            }

            if (_prewarmCount < 0)
            {
                throw new InvalidOperationException($"{nameof(ProjectileSpawner)} prewarm count cannot be negative.");
            }
        }
    }
}