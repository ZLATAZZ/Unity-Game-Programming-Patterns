using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace BloodMoon.Presentation
{
    public sealed class InteractionPromptView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private string _interactionText = "Invoke the Blood Moon";
        [SerializeField, Min(0f)] private float _fadeDuration = 0.15f;

        private Tween _fadeTween;

        private void Awake()
        {
            ValidateConfiguration();

            _canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        public void SetBinding(string bindingDisplayName)
        {
            _label.text = $"[{bindingDisplayName}] {_interactionText}";
        }

        public void Show()
        {
            KillTween();

            gameObject.SetActive(true);

            _fadeTween = _canvasGroup.DOFade(1f, _fadeDuration).SetEase(Ease.OutQuad);
        }

        public void Hide()
        {
            KillTween();

            _fadeTween = _canvasGroup
                .DOFade(0f, _fadeDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(() => gameObject.SetActive(false));
        }

        private void KillTween()
        {
            _fadeTween?.Kill();
            _fadeTween = null;
        }

        private void ValidateConfiguration()
        {
            if (_canvasGroup == null)
            {
                throw new InvalidOperationException($"{nameof(InteractionPromptView)} requires a CanvasGroup reference.");
            }

            if (_label == null)
            {
                throw new InvalidOperationException($"{nameof(InteractionPromptView)} requires a TMP Text reference.");
            }

            if (string.IsNullOrWhiteSpace(_interactionText))
            {
                throw new InvalidOperationException($"{nameof(InteractionPromptView)} interaction text cannot be empty.");
            }
        }

        private void OnDestroy()
        {
            KillTween();
        }
    }
}