using System;
using System.Collections.Generic;
using UnityEngine;

namespace BloodMoon.Pooling
{
    public sealed class PrefabPool<T> : IDisposable where T : Component, IPoolable
    {
        private readonly T _prefab;
        private readonly Transform _container;
        private readonly Stack<T> _inactiveInstances = new();
        private readonly HashSet<T> _activeInstances = new();

        private bool _isDisposed;

        public int ActiveCount => _activeInstances.Count;
        public int InactiveCount => _inactiveInstances.Count;

        public PrefabPool(T prefab, Transform container)
        {
            _prefab = prefab != null ? prefab : throw new ArgumentNullException(nameof(prefab));

            _container = container != null ? container : throw new ArgumentNullException(nameof(container));
        }

        public void Prewarm(int count)
        {
            ThrowIfDisposed();

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            for (int i = 0; i < count; i++)
            {
                T instance = CreateInstance();

                _inactiveInstances.Push(instance);
            }
        }

        public T Rent(Vector3 position, Quaternion rotation)
        {
            ThrowIfDisposed();

            T instance = GetInstance();

            Transform instanceTransform = instance.transform;

            instanceTransform.SetParent(_container);
            instanceTransform.SetPositionAndRotation(position, rotation);

            instance.OnRent();
            instance.gameObject.SetActive(true);

            if (!_activeInstances.Add(instance))
            {
                throw new InvalidOperationException($"{typeof(T).Name} instance is already active.");
            }

            return instance;
        }

        public void Return(T instance)
        {
            ThrowIfDisposed();

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (!_activeInstances.Remove(instance))
            {
                throw new InvalidOperationException($"{typeof(T).Name} instance does not belong to the active pool.");
            }

            instance.OnReturn();

            instance.gameObject.SetActive(false);
            instance.transform.SetParent(_container);

            _inactiveInstances.Push(instance);
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            foreach (T instance in _activeInstances)
            {
                if (instance != null)
                {
                    UnityEngine.Object.Destroy(instance.gameObject);
                }
            }

            _activeInstances.Clear();

            while (_inactiveInstances.Count > 0)
            {
                T instance = _inactiveInstances.Pop();

                if (instance != null)
                {
                    UnityEngine.Object.Destroy(instance.gameObject);
                }
            }

            _isDisposed = true;
        }

        private T GetInstance()
        {
            if (_inactiveInstances.Count > 0)
            {
                return _inactiveInstances.Pop();
            }

            return CreateInstance();
        }

        private T CreateInstance()
        {
            T instance = UnityEngine.Object.Instantiate(_prefab, _container);

            instance.gameObject.SetActive(false);

            return instance;
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(PrefabPool<T>));
            }
        }
    }
}