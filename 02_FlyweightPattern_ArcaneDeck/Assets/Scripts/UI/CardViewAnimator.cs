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

        private bool _isSelected;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void PlayAppear(int index)
        {
            KillTweens();

            _canvasGroup.alpha = 0f;
            _rectTransform.localScale = Vector3.one * 0.9f;

            Sequence sequence = DOTween.Sequence();

            sequence.SetDelay(index * _staggerDelay);
            sequence.Join(_canvasGroup.DOFade(1f, _appearDuration));
            sequence.Join(_rectTransform.DOScale(1f, _appearDuration).SetEase(Ease.OutBack));
        }

        public void SetSelected(bool isSelected)
        {
            _isSelected = isSelected;

            _rectTransform.DOKill();

            float targetScale = _isSelected ? 1.07f : 1f;

            _rectTransform.DOScale(targetScale, _selectionDuration).SetEase(Ease.OutQuad);
        }

        public void SetHovered(bool isHovered)
        {
            if (_isSelected)
            {
                return;
            }

            _rectTransform.DOKill();

            float targetScale = isHovered ? 1.035f : 1f;

            _rectTransform.DOScale(targetScale, _selectionDuration).SetEase(Ease.OutQuad);
        }

        public void PlayRejected()
        {
            _rectTransform.DOKill();

            _rectTransform.DOPunchPosition(new Vector2(12f, 0f), 0.25f, 8, 0.5f);
        }

        private void KillTweens()
        {
            _rectTransform.DOKill();
            _canvasGroup.DOKill();
        }

        private void OnDestroy()
        {
            KillTweens();
        }
    }
}