using System;
using UnityEngine;

namespace Patterns.Singleton.Session
{
    public sealed class RunSession : MonoBehaviour
    {
        public static RunSession Instance { get; private set; }

        public int Score { get; private set; }
        public int CollectedCrystals { get; private set; }
        public int CurrentLevel { get; private set; } = 1;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void AddScore(int amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Score amount must be greater than zero.");
            }

            Score += amount;
        }

        public void CollectCrystal()
        {
            CollectedCrystals++;
        }

        public void AdvanceLevel()
        {
            CurrentLevel++;
        }

        public void ResetRun()
        {
            Score = 0;
            CollectedCrystals = 0;
            CurrentLevel = 1;
        }
    }
}