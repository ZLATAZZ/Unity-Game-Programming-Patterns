using System;
using Patterns.Singleton.Session;
using UnityEngine;

namespace Patterns.Singleton.Gameplay
{
    public sealed class RunDemoActions : MonoBehaviour
    {
        [SerializeField] private int _enemyDefeatScoreReward;

        private void Awake()
        {
            ValidateConfiguration();
        }

        public void DefeatEnemy()
        {
            RunSession.Instance.AddScore(_enemyDefeatScoreReward);
        }

        public void CollectCrystal()
        {
            RunSession.Instance.CollectCrystal();
        }

        public void ResetRun()
        {
            RunSession.Instance.ResetRun();
        }

        private void ValidateConfiguration()
        {
            if (_enemyDefeatScoreReward <= 0)
            {
                throw new InvalidOperationException("Enemy defeat score reward must be greater than zero.");
            }
        }
    }
}