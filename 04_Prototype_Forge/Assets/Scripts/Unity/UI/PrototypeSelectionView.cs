using System;
using System.Collections.Generic;
using PrototypeForge.Core.Enemies;
using PrototypeForge.Core.Prototypes;
using PrototypeForge.Unity.Visuals;
using UnityEngine;

namespace PrototypeForge.Unity.UI
{
    public sealed class PrototypeSelectionView : MonoBehaviour
    {
        [SerializeField] private PrototypeCardView _cardPrefab;
        [SerializeField] private Transform _cardContainer;

        private readonly List<PrototypeCardView> _cards = new();

        private bool _isInitialized;

        public event Action<PrototypeId> PrototypeSelected;

        public void Initialize(IReadOnlyList<EnemyPrototype> prototypes, EnemyVisualCatalog visualCatalog)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(PrototypeSelectionView)} has already been initialized.");
            }

            ValidateConfiguration();

            if (prototypes == null)
            {
                throw new ArgumentNullException(nameof(prototypes));
            }

            if (visualCatalog == null)
            {
                throw new ArgumentNullException(nameof(visualCatalog));
            }

            for (int i = 0; i < prototypes.Count; i++)
            {
                EnemyPrototype prototype = prototypes[i];
                EnemyVisualDefinition visual = visualCatalog.Get(prototype.Id);

                PrototypeCardView card = Instantiate(_cardPrefab, _cardContainer);

                card.Selected += HandleCardSelected;
                card.Initialize(prototype, visual.Icon);

                _cards.Add(card);
            }

            _isInitialized = true;
        }

        private void HandleCardSelected(PrototypeId id)
        {
            PrototypeSelected?.Invoke(id);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _cards.Count; i++)
            {
                if (_cards[i] != null)
                {
                    _cards[i].Selected -= HandleCardSelected;
                }
            }
        }

        private void ValidateConfiguration()
        {
            if (_cardPrefab == null)
            {
                throw new InvalidOperationException($"{nameof(PrototypeSelectionView)} requires a Card Prefab.");
            }

            if (_cardContainer == null)
            {
                throw new InvalidOperationException($"{nameof(PrototypeSelectionView)} requires a Card Container.");
            }
        }
    }
}