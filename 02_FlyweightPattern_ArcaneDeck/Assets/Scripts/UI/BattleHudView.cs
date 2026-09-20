using System;
using DG.Tweening;
using Patterns.Flyweight.Battle;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Patterns.Flyweight.UI
{
    public sealed class BattleHudView : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private Image _playerImage;
        [SerializeField] private TMP_Text _playerNameText;
        [SerializeField] private TMP_Text _playerHealthText;
        [SerializeField] private TMP_Text _energyText;
        [SerializeField] private TMP_Text _blockText;
        [SerializeField] private RectTransform _playerPanel;

        [Header("Enemy")]
        [SerializeField] private Image _enemyImage;
        [SerializeField] private TMP_Text _enemyNameText;
        [SerializeField] private TMP_Text _enemyHealthText;
        [SerializeField] private TMP_Text _enemyIntentText;
        [SerializeField] private RectTransform _enemyPanel;

        [Header("Actions")]
        [SerializeField] private Button _endTurnButton;

        private int _previousPlayerHealth = -1;
        private int _previousEnemyHealth = -1;
        private int _previousEnergy = -1;
        private int _previousBlock = -1;

        public event Action EndTurnRequested;

        private void Awake()
        {
            _endTurnButton.onClick.AddListener(HandleEndTurnRequested);
        }

        public void Refresh(BattleContext context, BattlePhase phase)
        {
            _playerImage.sprite = context.Player.Definition.ProfileImage;
            _playerNameText.text = context.Player.Definition.PlayerName;

            _enemyImage.sprite = context.Enemy.Definition.Artwork;
            _enemyNameText.text = context.Enemy.Definition.EnemyName;

            _playerHealthText.text = $"HP {context.Player.CurrentHealth}/{context.Player.Definition.MaxHealth}";

            _energyText.text = $"Energy {context.Player.CurrentEnergy}/{context.Player.Definition.MaxEnergy}";

            _blockText.text = $"Block {context.Player.CurrentBlockAmount}";

            _enemyHealthText.text = $"HP {context.Enemy.CurrentHealth}/{context.Enemy.Definition.MaxHealth}";

            _enemyIntentText.text = $"Next Attack: {context.Enemy.NextAttackDamage}";

            PunchIfChanged(_playerHealthText.transform, ref _previousPlayerHealth, context.Player.CurrentHealth);

            PunchIfChanged(_enemyHealthText.transform, ref _previousEnemyHealth, context.Enemy.CurrentHealth);

            PunchIfChanged(_energyText.transform, ref _previousEnergy, context.Player.CurrentEnergy);

            PunchIfChanged(_blockText.transform, ref _previousBlock, context.Player.CurrentBlockAmount);

            _endTurnButton.interactable = phase == BattlePhase.PlayerTurn;
        }

        public void PlayEnemyAttack(int damage)
        {
            _playerPanel.DOKill();

            _playerPanel.DOPunchPosition(new Vector2(-14f, 0f), 0.25f, 8, 0.5f);
        }

        private void PunchIfChanged(Transform target, ref int previousValue, int currentValue)
        {
            if (previousValue >= 0 && previousValue != currentValue)
            {
                target.DOKill();

                target.DOPunchScale(Vector3.one * 0.08f, 0.2f, 6, 0.5f);
            }

            previousValue = currentValue;
        }

        private void HandleEndTurnRequested()
        {
            EndTurnRequested?.Invoke();
        }

        private void OnDestroy()
        {
            _endTurnButton.onClick.RemoveListener(HandleEndTurnRequested);
        }
    }
}