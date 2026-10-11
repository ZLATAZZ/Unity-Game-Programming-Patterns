using System;
using Patterns.Singleton.Levels;
using Patterns.Singleton.Session;
using UnityEngine;
using UnityEngine.UI;

namespace Patterns.Singleton.Gameplay
{
    public sealed class RunDemoControls : MonoBehaviour
    {
        [SerializeField] private Button _defeatEnemyButton;
        [SerializeField] private Button _collectCrystalButton;
        [SerializeField] private Button _resetProgressButton;
        [SerializeField, Min(1)] private int _enemyDefeatScoreReward = 100;

        private RunSession _session;
        private LevelDefinition _level;
        private bool _isInitialized;
        private bool _isSubscribed;

        public void Initialize(LevelDefinition level)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(RunDemoControls)} is already initialized.");
            }

            ValidateConfiguration();
            _session = RunSession.Instance;
            _level = level ?? throw new ArgumentNullException(nameof(level));

            _defeatEnemyButton.onClick.AddListener(HandleDefeatEnemy);
            _collectCrystalButton.onClick.AddListener(HandleCollectCrystal);
            _resetProgressButton.onClick.AddListener(HandleResetProgress);
            _isInitialized = true;

            if (isActiveAndEnabled)
            {
                Subscribe();
                RefreshAvailability();
            }
        }

        private void OnEnable()
        {
            if (!_isInitialized)
            {
                return;
            }

            Subscribe();
            RefreshAvailability();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();

            if (!_isInitialized)
            {
                return;
            }

            _defeatEnemyButton.onClick.RemoveListener(HandleDefeatEnemy);
            _collectCrystalButton.onClick.RemoveListener(HandleCollectCrystal);
            _resetProgressButton.onClick.RemoveListener(HandleResetProgress);
        }

        private void HandleDefeatEnemy()
        {
            if (_session.Score >= _level.RequiredScore)
            {
                return;
            }

            _session.AddScore(_enemyDefeatScoreReward);
        }

        private void HandleCollectCrystal()
        {
            if (_session.CollectedCrystals >= _level.RequiredCrystals)
            {
                return;
            }

            _session.CollectCrystal();
        }

        private void HandleResetProgress()
        {
            _session.ResetProgress();
        }

        private void RefreshAvailability()
        {
            _defeatEnemyButton.interactable = _session.Score < _level.RequiredScore;
            _collectCrystalButton.interactable = _session.CollectedCrystals < _level.RequiredCrystals;
            _resetProgressButton.interactable = true;
        }

        private void Subscribe()
        {
            if (_isSubscribed)
            {
                return;
            }

            _session.StateChanged += RefreshAvailability;
            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed)
            {
                return;
            }

            if (_session != null)
            {
                _session.StateChanged -= RefreshAvailability;
            }

            _isSubscribed = false;
        }

        private void ValidateConfiguration()
        {
            if (_defeatEnemyButton == null || _collectCrystalButton == null || _resetProgressButton == null)
            {
                throw new InvalidOperationException($"{nameof(RunDemoControls)} requires all three button references.");
            }

            if (_defeatEnemyButton == _collectCrystalButton || _defeatEnemyButton == _resetProgressButton || _collectCrystalButton == _resetProgressButton)
            {
                throw new InvalidOperationException($"{nameof(RunDemoControls)} requires three distinct buttons.");
            }

            if (_enemyDefeatScoreReward <= 0)
            {
                throw new InvalidOperationException("Enemy score reward must be greater than zero.");
            }
        }
    }
}
