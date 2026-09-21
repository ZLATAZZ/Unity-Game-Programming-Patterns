using DG.Tweening;
using Patterns.Flyweight.Battle;
using TMPro;
using UnityEngine;

namespace Patterns.Flyweight.UI
{
    public sealed class BattleResultView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _panel;
        [SerializeField] private TMP_Text _resultText;

        public void Hide()
        {
            _root.SetActive(false);
        }

        public void Show(BattlePhase phase)
        {
            _root.SetActive(true);

            _resultText.text = phase == BattlePhase.Victory ? "VICTORY" : "DEFEAT";

            _canvasGroup.alpha = 0f;
            _panel.localScale = Vector3.one * 0.85f;

            Sequence sequence = DOTween.Sequence();

            sequence.Join(_canvasGroup.DOFade(1f, 0.25f));
            sequence.Join(_panel.DOScale(1f, 0.3f).SetEase(Ease.OutBack));
        }
    }
}