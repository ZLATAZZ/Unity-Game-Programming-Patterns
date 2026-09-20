using System;
using Patterns.Flyweight.Battle;
using Patterns.Flyweight.Cards;
using Patterns.Flyweight.Deck;
using UnityEngine;

namespace Patterns.Flyweight.UI
{
    public sealed class BattleScreenPresenter : MonoBehaviour
    {
        [SerializeField] private HandView _handView;
        [SerializeField] private CardDetailsView _cardDetailsView;
        [SerializeField] private BattleHudView _hudView;
        [SerializeField] private BattleResultView _resultView;

        private BattleController _battleController;
        private BattleContext _battleContext;
        private DeckState _deckState;

        private CardInstance _selectedCard;

        public void Initialize(BattleController battleController, BattleContext battleContext, DeckState deckState)
        {
            _battleController = battleController ?? throw new ArgumentNullException(nameof(battleController));
            _battleContext = battleContext ?? throw new ArgumentNullException(nameof(battleContext));
            _deckState = deckState ?? throw new ArgumentNullException(nameof(deckState));

            _handView.CardSelected += HandleCardSelected;
            _cardDetailsView.PlayRequested += HandlePlayRequested;
            _hudView.EndTurnRequested += HandleEndTurnRequested;

            _battleController.StateChanged += HandleStateChanged;
            _battleController.PhaseChanged += HandlePhaseChanged;
            _battleController.EnemyAttacked += HandleEnemyAttacked;

            _cardDetailsView.Clear();
            _resultView.Hide();

            Refresh();
        }

        private void HandleCardSelected(CardInstance card)
        {
            _selectedCard = card;

            _handView.SetSelected(_selectedCard);

            RefreshSelectedCard();
        }

        private void HandlePlayRequested()
        {
            if (_selectedCard == null)
            {
                return;
            }

            bool played = _battleController.TryPlayCard(_selectedCard);

            if (!played)
            {
                _cardDetailsView.PlayRejectedFeedback();
                return;
            }

            _selectedCard = null;
        }

        private void HandleEndTurnRequested()
        {
            _selectedCard = null;

            _battleController.EndTurn();
        }

        private void HandleStateChanged()
        {
            Refresh();
        }

        private void HandlePhaseChanged(BattlePhase phase)
        {
            if (phase == BattlePhase.Victory || phase == BattlePhase.Defeat)
            {
                _resultView.Show(phase);
            }
        }

        private void HandleEnemyAttacked(int damage)
        {
            _hudView.PlayEnemyAttack(damage);
        }

        private void Refresh()
        {
            if (_selectedCard != null && !_deckState.IsInHand(_selectedCard))
            {
                _selectedCard = null;
            }

            _handView.Render(_deckState.Hand, _selectedCard);

            _handView.SetInteractable(_battleController.CurrentPhase == BattlePhase.PlayerTurn);

            _hudView.Refresh(_battleContext, _battleController.CurrentPhase);

            RefreshSelectedCard();
        }

        private void RefreshSelectedCard()
        {
            if (_selectedCard == null)
            {
                _cardDetailsView.Clear();
                return;
            }

            bool canPlay = _battleController.CurrentPhase == BattlePhase.PlayerTurn && _battleContext.Player.CurrentEnergy >= _selectedCard.Definition.EnergyCost;

            _cardDetailsView.Show(_selectedCard, canPlay);
        }

        private void OnDestroy()
        {
            if (_battleController != null)
            {
                _battleController.StateChanged -= HandleStateChanged;
                _battleController.PhaseChanged -= HandlePhaseChanged;
                _battleController.EnemyAttacked -= HandleEnemyAttacked;
            }

            _handView.CardSelected -= HandleCardSelected;
            _cardDetailsView.PlayRequested -= HandlePlayRequested;
            _hudView.EndTurnRequested -= HandleEndTurnRequested;
        }
    }
}