using System;
using System.Collections.Generic;
using PrototypeForge.Core.Enemies;
using PrototypeForge.Core.Prototypes;
using PrototypeForge.Unity.Data;
using PrototypeForge.Unity.UI;
using PrototypeForge.Unity.Visuals;
using UnityEngine;

namespace PrototypeForge.Unity.Composition
{
    public sealed class PrototypeForgeController : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] private TextAsset _prototypeJson;
        [SerializeField] private EnemyVisualCatalog _visualCatalog;

        [Header("Scene")]
        [SerializeField] private EnemySpawner _enemySpawner;

        [Header("UI")]
        [SerializeField] private PrototypeSelectionView _selectionView;
        [SerializeField] private PrototypeComparisonView _comparisonView;

        [Header("Demo")]
        [SerializeField, Min(1)] private int _overchargeDamageBonus = 20;

        private PrototypeRegistry<EnemyPrototype> _registry;

        private PrototypeId _selectedPrototypeId;
        private EnemyPrototype _selectedBaseline;
        private EnemyActor _currentClone;

        private bool _hasSelection;

        private void Awake()
        {
            ValidateConfiguration();

            BuildApplication();
        }

        private void BuildApplication()
        {
            _visualCatalog.BuildLookup();

            EnemyPrototypeJsonLoader loader = new();

            IReadOnlyList<EnemyPrototype> loadedPrototypes = loader.Load(_prototypeJson.text);

            _registry = new PrototypeRegistry<EnemyPrototype>();

            for (int i = 0; i < loadedPrototypes.Count; i++)
            {
                EnemyPrototype prototype = loadedPrototypes[i];

                _registry.Register(prototype);

                _visualCatalog.Get(prototype.Id);
            }

            _comparisonView.Initialize();
            _enemySpawner.Initialize(_registry, _visualCatalog);

            _selectionView.PrototypeSelected += HandlePrototypeSelected;
            _comparisonView.CloneRequested += HandleCloneRequested;
            _comparisonView.OverchargeRequested += HandleOverchargeRequested;

            _selectionView.Initialize(
                _registry.CreateAll(),
                _visualCatalog);
        }

        private void HandlePrototypeSelected(PrototypeId id)
        {
            _selectedPrototypeId = id;
            _selectedBaseline = _registry.Create(id);

            _currentClone = null;
            _hasSelection = true;

            _comparisonView.SetSelectedPrototype(_selectedBaseline);
            _comparisonView.ClearClone();
        }

        private void HandleCloneRequested()
        {
            if (!_hasSelection)
            {
                throw new InvalidOperationException("Cannot create a clone before selecting a prototype.");
            }

            _currentClone = _enemySpawner.Spawn(_selectedPrototypeId);

            _comparisonView.SetActiveClone(_currentClone.Configuration);
        }

        private void HandleOverchargeRequested()
        {
            if (_currentClone == null)
            {
                throw new InvalidOperationException("Cannot overcharge because no clone exists.");
            }

            _currentClone.ApplyOvercharge(_overchargeDamageBonus);

            _comparisonView.SetActiveClone(_currentClone.Configuration);
        }

        private void OnDestroy()
        {
            if (_selectionView != null)
            {
                _selectionView.PrototypeSelected -= HandlePrototypeSelected;
            }

            if (_comparisonView != null)
            {
                _comparisonView.CloneRequested -= HandleCloneRequested;
                _comparisonView.OverchargeRequested -= HandleOverchargeRequested;
            }
        }

        private void ValidateConfiguration()
        {
            if (_prototypeJson == null)
            {
                throw new InvalidOperationException($"{nameof(PrototypeForgeController)} requires a Prototype JSON asset.");
            }

            if (_visualCatalog == null)
            {
                throw new InvalidOperationException($"{nameof(PrototypeForgeController)} requires an Enemy Visual Catalog.");
            }

            if (_enemySpawner == null)
            {
                throw new InvalidOperationException($"{nameof(PrototypeForgeController)} requires an Enemy Spawner.");
            }

            if (_selectionView == null)
            {
                throw new InvalidOperationException($"{nameof(PrototypeForgeController)} requires a Prototype Selection View.");
            }

            if (_comparisonView == null)
            {
                throw new InvalidOperationException($"{nameof(PrototypeForgeController)} requires a Prototype Comparison View.");
            }

            if (_overchargeDamageBonus <= 0)
            {
                throw new InvalidOperationException($"{nameof(PrototypeForgeController)} overcharge damage bonus must be positive.");
            }
        }
    }
}