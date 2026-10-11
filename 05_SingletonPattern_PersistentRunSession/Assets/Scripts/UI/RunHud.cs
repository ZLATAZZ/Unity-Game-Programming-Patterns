using System;
using Patterns.Singleton.Levels;
using Patterns.Singleton.Session;
using TMPro;
using UnityEngine;

namespace Patterns.Singleton.UI
{
    public sealed class RunHud : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _crystalsText;

        private RunSession _session;
        private LevelDefinition _level;
        private bool _isInitialized;
        private bool _isSubscribed;

        public void Initialize(LevelDefinition level)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(RunHud)} is already initialized.");
            }

            ValidateConfiguration();
            _session = RunSession.Instance;
            _level = level ?? throw new ArgumentNullException(nameof(level));
            _isInitialized = true;

            if (isActiveAndEnabled)
            {
                Subscribe();
                Refresh();
            }
        }

        private void OnEnable()
        {
            if (!_isInitialized)
            {
                return;
            }

            Subscribe();
            Refresh();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Refresh()
        {
            _scoreText.text = $"Score: {_session.Score} / {_level.RequiredScore}";
            _crystalsText.text = $"Crystals: {_session.CollectedCrystals} / {_level.RequiredCrystals}";
        }

        private void Subscribe()
        {
            if (_isSubscribed)
            {
                return;
            }

            _session.StateChanged += Refresh;
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
                _session.StateChanged -= Refresh;
            }

            _isSubscribed = false;
        }

        private void ValidateConfiguration()
        {
            if (_scoreText == null || _crystalsText == null)
            {
                throw new InvalidOperationException($"{nameof(RunHud)} requires score and crystal text references.");
            }
        }
    }
}
