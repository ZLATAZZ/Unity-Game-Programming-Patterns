using System;
using System.Collections.Generic;
using PrototypeForge.Core.Prototypes;
using UnityEngine;

namespace PrototypeForge.Unity.Visuals
{
    [Serializable]
    public sealed class EnemyVisualEntry
    {
        [SerializeField] private string _prototypeId;
        [SerializeField] private Sprite _icon;
        [SerializeField] private EnemyActor _prefab;

        public string PrototypeId => _prototypeId;
        public Sprite Icon => _icon;
        public EnemyActor Prefab => _prefab;
    }

    public readonly struct EnemyVisualDefinition
    {
        public Sprite Icon { get; }
        public EnemyActor Prefab { get; }

        public EnemyVisualDefinition(Sprite icon, EnemyActor prefab)
        {
            Icon = icon != null ? icon : throw new ArgumentNullException(nameof(icon));
            Prefab = prefab != null ? prefab : throw new ArgumentNullException(nameof(prefab));
        }
    }

    [CreateAssetMenu(fileName = "EnemyVisualCatalog", menuName = "Prototype Forge/Enemy Visual Catalog")]
    public sealed class EnemyVisualCatalog : ScriptableObject
    {
        [SerializeField] private EnemyVisualEntry[] _entries;

        private Dictionary<PrototypeId, EnemyVisualDefinition> _lookup;

        public void BuildLookup()
        {
            if (_entries == null || _entries.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(EnemyVisualCatalog)} requires at least one entry.");
            }

            Dictionary<PrototypeId, EnemyVisualDefinition> lookup = new();

            for (int i = 0; i < _entries.Length; i++)
            {
                EnemyVisualEntry entry = _entries[i];

                if (entry == null)
                {
                    throw new InvalidOperationException($"{nameof(EnemyVisualCatalog)} contains a null entry at index {i}.");
                }

                PrototypeId id = new(entry.PrototypeId);

                EnemyVisualDefinition definition = new(
                    entry.Icon,
                    entry.Prefab);

                if (!lookup.TryAdd(id, definition))
                {
                    throw new InvalidOperationException($"{nameof(EnemyVisualCatalog)} contains duplicate ID '{id}'.");
                }
            }

            _lookup = lookup;
        }

        public EnemyVisualDefinition Get(PrototypeId id)
        {
            EnsureInitialized();

            if (!_lookup.TryGetValue(id, out EnemyVisualDefinition definition))
            {
                throw new KeyNotFoundException($"No visual configuration exists for prototype '{id}'.");
            }

            return definition;
        }

        private void EnsureInitialized()
        {
            if (_lookup == null)
            {
                throw new InvalidOperationException($"{nameof(EnemyVisualCatalog)} has not been initialized.");
            }
        }
    }
}