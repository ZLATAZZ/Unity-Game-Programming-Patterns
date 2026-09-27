using System;
using DG.Tweening;
using UnityEngine;

namespace BloodMoon.Presentation
{
    public sealed class ArenaBarrierView : MonoBehaviour
    {
        [SerializeField] private Transform _barrier;
        [SerializeField] private Transform _openPoint;
        [SerializeField] private Transform _closedPoint;
        [SerializeField, Min(0f)] private float _moveDuration = 0.6f;

        private Tween _movementTween;
        private bool _isValidated;

        public void Open()
        {
            EnsureConfigured();
            MoveTo(_openPoint.position);
        }

        public void Close()
        {
            EnsureConfigured();
            MoveTo(_closedPoint.position);
        }

        public void OpenImmediate()
        {
            EnsureConfigured();

            KillTween();
            _barrier.position = _openPoint.position;
        }

        public void CloseImmediate()
        {
            EnsureConfigured();

            KillTween();
            _barrier.position = _closedPoint.position;
        }

        private void MoveTo(Vector3 targetPosition)
        {
            KillTween();

            _movementTween = _barrier
                .DOMove(targetPosition, _moveDuration)
                .SetEase(Ease.InOutQuad);
        }

        private void EnsureConfigured()
        {
            if (_isValidated)
            {
                return;
            }

            if (_barrier == null)
            {
                throw new InvalidOperationException($"{nameof(ArenaBarrierView)} requires a Barrier reference.");
            }

            if (_openPoint == null)
            {
                throw new InvalidOperationException($"{nameof(ArenaBarrierView)} requires an Open Point reference.");
            }

            if (_closedPoint == null)
            {
                throw new InvalidOperationException($"{nameof(ArenaBarrierView)} requires a Closed Point reference.");
            }

            _isValidated = true;
        }

        private void KillTween()
        {
            _movementTween?.Kill();
            _movementTween = null;
        }

        private void OnDestroy()
        {
            KillTween();
        }
    }
}