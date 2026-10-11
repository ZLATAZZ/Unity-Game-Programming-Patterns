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
                throw new ArgumentOutOfRangeException(nameof(amount), "Score reward must be greater than zero.");
            }

            Score = checked(Score + amount);
            StateChanged?.Invoke();
        }

        public void CollectCrystal()
        {
            CollectedCrystals = checked(CollectedCrystals + 1);
            StateChanged?.Invoke();
        }

        public void ResetProgress()
        {
            Score = 0;
            CollectedCrystals = 0;
            StateChanged?.Invoke();
        }
    }
}
