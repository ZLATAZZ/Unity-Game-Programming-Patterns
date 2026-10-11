using System;
using Patterns.Singleton.Session;
using TMPro;
using UnityEngine;

namespace Patterns.Singleton.UI
{
    public sealed class RunHud : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private TMP_Text _crystalsText;
        [SerializeField] private TMP_Text _levelText;

        private RunSession _session;
        private bool _isSubscribed;

        private void Awake()
        {
            ValidateConfiguration();
        }

        private void Start()
        {
            _session = RunSession.Instance;

            Subscribe();
            Refresh();
        }

        private void OnEnable()
        {
            if (_session == null)
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
            if (!_isSubscribed || _session == null)
            {
                return;
            }

            _session.StateChanged -= Refresh;
            _isSubscribed = false;
        }

        private void Refresh()
        {
            _scoreText.text = $"Score: {_session.Score}";
            _crystalsText.text = $"Crystals: {_session.CollectedCrystals}";
            _levelText.text = $"Level: {_session.CurrentLevel}";
        }

        private void ValidateConfiguration()
        {
            if (_scoreText == null)
            {
                throw new InvalidOperationException($"{nameof(RunHud)} requires a score text reference.");
            }

            if (_crystalsText == null)
            {
                throw new InvalidOperationException($"{nameof(RunHud)} requires a crystals text reference.");
            }

            if (_levelText == null)
            {
                throw new InvalidOperationException($"{nameof(RunHud)} requires a level text reference.");
            }
        }
    }
}