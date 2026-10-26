using System;
using Patterns.DoubleBuffer.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Patterns.DoubleBuffer.Unity.UI
{
    public sealed class SlapCanvasPresenter : MonoBehaviour
    {
        private const float AutoPlayTickIntervalSeconds = 1.25f;

        [SerializeField] private Button _evaluateButton;
        [SerializeField] private Button _publishButton;
        [SerializeField] private Button _tickButton;
        [SerializeField] private Button _autoButton;
        [SerializeField] private Button _resetButton;
        [SerializeField] private Text _tickLabel;
        [SerializeField] private Text _phaseLabel;
        [SerializeField] private Text _insightLabel;
        [SerializeField] private Text _autoButtonLabel;
        [SerializeField] private SlapStageCardView[] _cards;

        private SlapComparison _comparison;
        private bool _isAutoPlaying;
        private float _elapsedSeconds;

        public void Configure(Button evaluateButton, Button publishButton, Button tickButton, Button autoButton, Button resetButton, Text tickLabel, Text phaseLabel, Text insightLabel, Text autoButtonLabel, SlapStageCardView[] cards)
        {
            _evaluateButton = evaluateButton;
            _publishButton = publishButton;
            _tickButton = tickButton;
            _autoButton = autoButton;
            _resetButton = resetButton;
            _tickLabel = tickLabel;
            _phaseLabel = phaseLabel;
            _insightLabel = insightLabel;
            _autoButtonLabel = autoButtonLabel;
            _cards = cards;
        }

        private void Start()
        {
            if (_cards == null || _cards.Length != 4 || _evaluateButton == null || _publishButton == null)
            {
                throw new InvalidOperationException("SlapCanvasPresenter requires configured uGUI references.");
            }

            _evaluateButton.onClick.AddListener(HandleEvaluate);
            _publishButton.onClick.AddListener(HandlePublish);
            _tickButton.onClick.AddListener(HandleTick);
            _autoButton.onClick.AddListener(HandleAuto);
            _resetButton.onClick.AddListener(HandleReset);
            ResetSimulation();
        }

        private void Update()
        {
            if (!_isAutoPlaying)
            {
                return;
            }

            _elapsedSeconds += Time.unscaledDeltaTime;

            if (_elapsedSeconds < AutoPlayTickIntervalSeconds)
            {
                return;
            }

            _elapsedSeconds -= AutoPlayTickIntervalSeconds;
            _comparison.CompleteTick();
            Render();
        }

        private void OnDestroy()
        {
            _evaluateButton?.onClick.RemoveListener(HandleEvaluate);
            _publishButton?.onClick.RemoveListener(HandlePublish);
            _tickButton?.onClick.RemoveListener(HandleTick);
            _autoButton?.onClick.RemoveListener(HandleAuto);
            _resetButton?.onClick.RemoveListener(HandleReset);
        }

        private void HandleEvaluate()
        {
            StopAuto();
            _comparison.Evaluate();
            Render();
        }

        private void HandlePublish()
        {
            StopAuto();
            _comparison.Publish();
            Render();
        }

        private void HandleTick()
        {
            StopAuto();
            _comparison.CompleteTick();
            Render();
        }

        private void HandleAuto()
        {
            _isAutoPlaying = !_isAutoPlaying;
            _elapsedSeconds = 0f;
            Render();
        }

        private void HandleReset()
        {
            ResetSimulation();
        }

        private void StopAuto()
        {
            _isAutoPlaying = false;
            _elapsedSeconds = 0f;
        }

        private void ResetSimulation()
        {
            StopAuto();
            _comparison = new SlapComparison();
            Render();
        }

        private void Render()
        {
            _tickLabel.text = "TICK " + _comparison.CompletedTicks.ToString("00");
            _autoButtonLabel.text = _isAutoPlaying ? "PAUSE AUTO" : "AUTO PLAY";
            _phaseLabel.text = _comparison.IsAwaitingPublication
                ? "PHASE 2 / 2    EVALUATED - NEXT IS NOT YET VISIBLE"
                : "PHASE 1 / 2    READY TO EVALUATE";

            _evaluateButton.interactable = !_comparison.IsAwaitingPublication && !_isAutoPlaying;
            _publishButton.interactable = _comparison.IsAwaitingPublication && !_isAutoPlaying;
            _tickButton.interactable = !_isAutoPlaying;

            for (int index = 0; index < _cards.Length; index++)
            {
                _cards[index].Render(_comparison.Stages[index]);
            }

            if (_comparison.IsAwaitingPublication)
            {
                _insightLabel.text = "EVALUATE COMPLETE  /  Buffered actors only wrote to NEXT. Press PUBLISH to make those slaps visible on the following tick.";
            }
            else if (_comparison.CompletedTicks == 0)
            {
                _insightLabel.text = "INITIAL STATE  /  Only Harry is slapped. Compare both update orders, then press EVALUATE to inspect CURRENT vs NEXT.";
            }
            else
            {
                _insightLabel.text = "TICK COMPLETE  /  All actors will be evaluated again next tick. In buffered mode, each actor reads the same logical snapshot.";
            }
        }
    }
}
