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
            GetSession().AddScore(_enemyDefeatScoreReward);
        }

        public void CollectCrystal()
        {
            GetSession().CollectCrystal();
        }

        public void ResetRun()
        {
            GetSession().ResetRun();
        }

        private RunSession GetSession()
        {
            if (RunSession.Instance == null)
            {
                throw new InvalidOperationException("RunSession instance is not initialized.");
            }

            return RunSession.Instance;
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