using System;
using Patterns.Singleton.Levels;
using Patterns.Singleton.Session;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Patterns.Singleton.Gameplay
{
    public sealed class LevelProgressionController : MonoBehaviour
    {
        [SerializeField] private Button _nextLevelButton;
        [SerializeField] private GameObject _completionPanel;

        private RunSession _session;
        private LevelDefinition _currentLevel;
        private LevelDefinition _nextLevel;
        private int _nextSceneBuildIndex;
        private bool _isInitialized;
        private bool _isSubscribed;
        private bool _isLoading;

        public void Initialize(LevelDefinition currentLevel, LevelDefinition nextLevel)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(LevelProgressionController)} is already initialized.");
            }

            _session = RunSession.Instance;
            _currentLevel = currentLevel ?? throw new ArgumentNullException(nameof(currentLevel));
            _nextLevel = nextLevel;

            if (_nextLevel != null)
            {
                if (_nextLevelButton == null)
                {
                    throw new InvalidOperationException("A non-final level requires a next-level button.");
                }

                _nextSceneBuildIndex = _nextLevel.Scene.BuildIndex;
                _nextLevelButton.interactable = false;
                _nextLevelButton.onClick.AddListener(HandleNextLevel);
            }
            else
            {
                if (_completionPanel == null)
                {
                    throw new InvalidOperationException("The final level requires a completion panel.");
                }

                _completionPanel.SetActive(false);
            }

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

            if (_isInitialized && _nextLevel != null && _nextLevelButton != null)
            {
                _nextLevelButton.onClick.RemoveListener(HandleNextLevel);
            }
        }

        private void HandleNextLevel()
        {
            if (_isLoading || !MeetsRequirements())
            {
                return;
            }

            _isLoading = true;
            _nextLevelButton.interactable = false;
            SceneManager.LoadScene(_nextSceneBuildIndex);
        }

        private void RefreshAvailability()
        {
            bool goalsReached = MeetsRequirements();

            if (_nextLevel != null)
            {
                _nextLevelButton.interactable = !_isLoading && goalsReached;
            }
            else
            {
                _completionPanel.SetActive(goalsReached);
            }
        }

        private bool MeetsRequirements()
        {
            return _session.Score >= _currentLevel.RequiredScore && _session.CollectedCrystals >= _currentLevel.RequiredCrystals;
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
    }
}
