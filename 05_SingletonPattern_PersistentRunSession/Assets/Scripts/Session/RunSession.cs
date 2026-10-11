using System;
using UnityEngine;

namespace Patterns.Singleton.Session
{
    public sealed class RunSession : MonoBehaviour
    {
        private static RunSession _instance;

        public static RunSession Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException($"{nameof(RunSession)} instance is not initialized.");
                }

                return _instance;
            }
        }

        public int Score { get; private set; }
        public int CollectedCrystals { get; private set; }
        public int CurrentLevel { get; private set; } = 1;

        public event Action StateChanged;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public void AddScore(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(amount),
                    "Score amount must be greater than zero.");
            }

            Score += amount;

            StateChanged?.Invoke();
        }

        public void CollectCrystal()
        {
            CollectedCrystals++;

            StateChanged?.Invoke();
        }

        public void AdvanceLevel()
        {
            CurrentLevel++;

            StateChanged?.Invoke();
        }

        public void ResetRun()
        {
            Score = 0;
            CollectedCrystals = 0;
            CurrentLevel = 1;

            StateChanged?.Invoke();
        }
    }
}