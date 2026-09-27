using System;
using DG.Tweening;
using UnityEngine;

namespace BloodMoon.Presentation
{
    public sealed class GhostView : MonoBehaviour
    {
        [SerializeField] private Transform _visualRoot;
        [SerializeField, Min(0f)] private float _hitDuration = 0.16f;
        [SerializeField, Range(0.1f, 1f)] private float _squashYMultiplier = 0.7f;
        [SerializeField, Min(1f)] private float _stretchXZMultiplier = 1.15f;

        private Vector3 _defaultScale;
        private Sequence _hitSequence;

        private void Awake()
        {
            ValidateConfiguration();
            _defaultScale = _visualRoot.localScale;
        }

        public void PlayHit()
        {
            KillTween();

            Vector3 squashScale = new(
                _defaultScale.x * _stretchXZMultiplier,
                _defaultScale.y * _squashYMultiplier,
                _defaultScale.z * _stretchXZMultiplier);

            _hitSequence = DOTween.Sequence()
                .Append(_visualRoot.DOScale(squashScale, _hitDuration * 0.5f).SetEase(Ease.OutQuad))
                .Append(_visualRoot.DOScale(_defaultScale, _hitDuration * 0.5f).SetEase(Ease.OutBack));
        }

        public void ResetImmediate()
        {
            KillTween();
            _visualRoot.localScale = _defaultScale;
        }

        private void KillTween()
        {
            _hitSequence?.Kill();
            _hitSequence = null;
        }

        private void OnDestroy()
        {
            KillTween();
        }

        private void ValidateConfiguration()
        {
            if (_visualRoot == null)
            {
                throw new InvalidOperationException($"{nameof(GhostView)} requires a Visual Root reference.");
            }

            if (_hitDuration <= 0f)
            {
                throw new InvalidOperationException($"{nameof(GhostView)} hit duration must be greater than zero.");
            }
        }
    }
}