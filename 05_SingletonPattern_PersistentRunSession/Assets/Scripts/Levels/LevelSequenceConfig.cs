using System;
using System.Collections.Generic;
using UnityEngine;

namespace Patterns.Singleton.Levels
{
    [CreateAssetMenu(fileName = "LevelSequence", menuName = "Patterns/Singleton/Level Sequence")]
    public sealed class LevelSequenceConfig : ScriptableObject
    {
        [SerializeField] private LevelDefinition[] _levels = Array.Empty<LevelDefinition>();

        public IReadOnlyList<LevelDefinition> Levels => _levels;
    }
}
