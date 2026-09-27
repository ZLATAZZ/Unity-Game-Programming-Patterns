using System;
using DG.Tweening;
using UnityEngine;

namespace BloodMoon.Presentation
{
    public sealed class PlayerMagicView : MonoBehaviour
    {
        [SerializeField] private GameObject _weaponRoot;
        [SerializeField, Min(0f)] private float _transitionDuration = 0.2f;

        private Vector3 _defaultScale;
        private Tween _transitionTween;

        private void Awake()
        {
            ValidateConfiguration();
            _defaultScale = _weaponRoot.transform.localScale;
        }

        public void Show()
        {
            KillTween();

            Transform weaponTransform = _weaponRoot.transform;

            weaponTransform.localScale = Vector3.zero;
            _weaponRoot.SetActive(true);

            _transitionTween = weaponTransform.DOScale(_defaultScale, _transitionDuration).SetEase(Ease.OutBack);
        }

        public void Hide()
        {
            KillTween();

            if (!_weaponRoot.activeSelf)
            {
                return;
            }

            Transform weaponTransform = _weaponRoot.transform;

            _transitionTween = weaponTransform
                .DOScale(Vector3.zero, _transitionDuration)
                .SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    _weaponRoot.SetActive(false);
                    weaponTransform.localScale = _defaultScale;
                });
        }

        public void SetVisibleImmediate(bool isVisible)
        {
            KillTween();

            _weaponRoot.transform.localScale = _defaultScale;
            _weaponRoot.SetActive(isVisible);
        }

        private void KillTween()
        {
            _transitionTween?.Kill();
            _transitionTween = null;
        }

        private void OnDestroy()
        {
            KillTween();
        }

        private void ValidateConfiguration()
        {
            if (_weaponRoot == null)
            {
                throw new InvalidOperationException($"{nameof(PlayerMagicView)} requires a Weapon Root reference.");
            }
        }
    }
}