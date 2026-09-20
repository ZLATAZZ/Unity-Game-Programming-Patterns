using System;
using Patterns.Flyweight.Cards;
using Patterns.Flyweight.Deck;

namespace Patterns.Flyweight.Battle
{
    public sealed class BattleController
    {
        private readonly DeckState _deckState;
        private readonly BattleContext _battleContext;
        private readonly Random _random;

        private BattlePhase _currentPhase;

        public BattlePhase CurrentPhase => _currentPhase;

        public event Action<CardInstance> CardDrawn;
        public event Action<CardInstance> CardPlayed;
        public event Action<int> EnemyAttacked;
        public event Action<BattlePhase> PhaseChanged;
        public event Action StateChanged;

        public BattleController(DeckState deckState, BattleContext battleContext, Random random)
        {
            _deckState = deckState ?? throw new ArgumentNullException(nameof(deckState));
            _battleContext = battleContext ?? throw new ArgumentNullException(nameof(battleContext));
            _random = random ?? throw new ArgumentNullException(nameof(random));
            _currentPhase = BattlePhase.NotStarted;
        }

        public void StartBattle()
        {
            if (_currentPhase != BattlePhase.NotStarted)
            {
                throw new InvalidOperationException("Battle has already started.");
            }

            GenerateEnemyIntent();
            DrawUntilHandIsFull();

            SetPhase(BattlePhase.PlayerTurn);
            StateChanged?.Invoke();
        }

        public bool TryPlayCard(CardInstance card)
        {
            if (card == null)
            {
                throw new ArgumentNullException(nameof(card));
            }

            if (_currentPhase != BattlePhase.PlayerTurn)
            {
                throw new InvalidOperationException("Cards can only be played during the player's turn.");
            }

            if (!_deckState.IsInHand(card))
            {
                throw new InvalidOperationException("The specified card is not in the player's hand.");
            }

            if (!_battleContext.Player.TrySpendEnergy(card.Definition.EnergyCost))
            {
                return false;
            }

            int effectValue = card.EffectiveValue;

            card.Definition.CardEffect.Apply(_battleContext, effectValue);

            card.TryUpgrade();

            _deckState.Discard(card);

            CardPlayed?.Invoke(card);

            if (_battleContext.Enemy.IsDead)
            {
                SetPhase(BattlePhase.Victory);
            }

            StateChanged?.Invoke();

            return true;
        }

        public void EndTurn()
        {
            if (_currentPhase != BattlePhase.PlayerTurn)
            {
                throw new InvalidOperationException("The turn can only be ended during the player's turn.");
            }

            _deckState.DiscardHand();

            SetPhase(BattlePhase.EnemyTurn);

            int attackDamage = _battleContext.Enemy.NextAttackDamage;

            _battleContext.Player.TakeDamage(attackDamage);

            EnemyAttacked?.Invoke(attackDamage);

            if (_battleContext.Player.IsDead)
            {
                SetPhase(BattlePhase.Defeat);
                StateChanged?.Invoke();
                return;
            }

            _battleContext.Player.ClearBlock();
            _battleContext.Player.RestoreEnergy();

            GenerateEnemyIntent();
            DrawUntilHandIsFull();

            SetPhase(BattlePhase.PlayerTurn);

            StateChanged?.Invoke();
        }

        private void DrawUntilHandIsFull()
        {
            while (_deckState.Hand.Count < _battleContext.Player.HandCapacity)
            {
                CardInstance drawnCard = _deckState.DrawCard();
                CardDrawn?.Invoke(drawnCard);
            }
        }

        private void GenerateEnemyIntent()
        {
            int minDamage = _battleContext.Enemy.Definition.MinAttackDamage;
            int maxDamage = _battleContext.Enemy.Definition.MaxAttackDamage;

            int nextAttackDamage = _random.Next(minDamage, maxDamage + 1);

            _battleContext.Enemy.SetNextAttackDamage(nextAttackDamage);
        }

        private void SetPhase(BattlePhase phase)
        {
            if (_currentPhase == phase)
            {
                return;
            }

            _currentPhase = phase;
            PhaseChanged?.Invoke(_currentPhase);
        }
    }
}