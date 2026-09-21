using DG.Tweening;
using UnityEngine;

namespace Patterns.Flyweight.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class CardViewAnimator : MonoBehaviour
    {
        [SerializeField] private float _appearDuration = 0.25f;
        [SerializeField] private float _selectionDuration = 0.15f;
        [SerializeField] private float _staggerDelay = 0.05f;

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;

        private Tween _fadeTween;
        private Tween _scaleTween;
        private Tween _feedbackTween;

        private bool _isSelected;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void PlayAppear(int index)
        {
            KillTweens();

            _isSelected = false;

            _canvasGroup.alpha = 0f;
            _rectTransform.localScale = Vector3.one * 0.9f;
            _rectTransform.localRotation = Quaternion.identity;

            float delay = index * _staggerDelay;

            _fadeTween = _canvasGroup
                .DOFade(1f, _appearDuration)
                .SetDelay(delay);

            _scaleTween = _rectTransform
                .DOScale(1f, _appearDuration)
                .SetDelay(delay)
                .SetEase(Ease.OutBack);
        }

        public void SetSelected(bool isSelected)
        {
            _isSelected = isSelected;

            _scaleTween?.Kill();

            float targetScale = _isSelected ? 1.07f : 1f;

            _scaleTween = _rectTransform
                .DOScale(targetScale, _selectionDuration)
                .SetEase(Ease.OutQuad);
        }

        public void SetHovered(bool isHovered)
        {
            if (_isSelected)
            {
                return;
            }

            _scaleTween?.Kill();

            float targetScale = isHovered ? 1.035f : 1f;

            _scaleTween = _rectTransform
                .DOScale(targetScale, _selectionDuration)
                .SetEase(Ease.OutQuad);
        }

        public void PlayRejected()
        {
            _feedbackTween?.Kill();

            _feedbackTween = _rectTransform
                .DOShakeRotation(
                    0.25f,
                    new Vector3(0f, 0f, 8f),
                    10,
                    90f,
                    false);
        }

        public void ResetImmediate()
        {
            KillTweens();

            _isSelected = false;

            _canvasGroup.alpha = 1f;
            _rectTransform.localScale = Vector3.one;
            _rectTransform.localRotation = Quaternion.identity;
        }

        private void KillTweens()
        {
            _fadeTween?.Kill();
            _scaleTween?.Kill();
            _feedbackTween?.Kill();

            _fadeTween = null;
            _scaleTween = null;
            _feedbackTween = null;
        }

        private void OnDisable()
        {
            KillTweens();
        }

        private void OnDestroy()
        {
            KillTweens();
        }
    }
}