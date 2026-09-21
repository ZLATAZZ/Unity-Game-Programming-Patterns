using Patterns.Flyweight.Battle;
using Patterns.Flyweight.Cards;
using Patterns.Flyweight.Deck;
using Patterns.Flyweight.Enemy;
using Patterns.Flyweight.Player;
using Patterns.Flyweight.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Patterns.Flyweight.Game
{
    public sealed class FlyweightBootstrapper : MonoBehaviour
    {
        [SerializeField] private CardDefinition[] _cardDefinitions;
        [SerializeField] private DeckCompositionEntry[] _playerDeckComposition;

        [SerializeField] private PlayerDefinition _playerDefinition;
        [SerializeField] private EnemyDefinition _enemyDefinition;

        [SerializeField] private BattleScreenPresenter _battleScreenPresenter;


        private BattleController _battleController;
        private BattleContext _battleContext;
        private PlayerState _playerState;
        private EnemyState _enemyState;
        private DeckState _deckState;

        private void Start()
        {
            ValidateConfiguration();

            System.Random random = new();

            CardDefinitionRegistry registry = new(_cardDefinitions);
            CardInstanceFactory factory = new(registry);

            CardInstance[] cards = CreateDeck(factory);

            _playerState = new PlayerState(_playerDefinition);
            _enemyState = new EnemyState(_enemyDefinition);

            _battleContext = new BattleContext(_playerState, _enemyState);
            _deckState = new DeckState(cards, random);

            _battleController = new BattleController(_deckState, _battleContext, random);

            _battleController.StartBattle();

            _battleController = new BattleController(_deckState, _battleContext, random);

            _battleScreenPresenter.Initialize(_battleController, _battleContext, _deckState);

            _battleController.StartBattle();
        }

        private CardInstance[] CreateDeck(CardInstanceFactory factory)
        {
            List<CardInstance> cards = new();

            foreach (DeckCompositionEntry entry in _playerDeckComposition)
            {
                for (int i = 0; i < entry.Count; i++)
                {
                    cards.Add(factory.Create(entry.CardId));
                }
            }

            return cards.ToArray();
        }

        private void ValidateConfiguration()
        {
            if (_cardDefinitions == null || _cardDefinitions.Length == 0)
            {
                throw new InvalidOperationException("Card definitions are not configured.");
            }

            if (_playerDeckComposition == null || _playerDeckComposition.Length == 0)
            {
                throw new InvalidOperationException("Player deck composition is not configured.");
            }

            if (_playerDefinition == null)
            {
                throw new InvalidOperationException("Player definition is not assigned.");
            }

            if (_enemyDefinition == null)
            {
                throw new InvalidOperationException("Enemy definition is not assigned.");
            }

            if (_playerDefinition.HandCapacity <= 0)
            {
                throw new InvalidOperationException("Hand size must be greater than zero.");
            }
        }
    }
}