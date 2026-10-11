using System;
using UnityEngine;

namespace Patterns.Singleton.Levels
{
    [Serializable]
    public sealed class LevelDefinition
    {
        [SerializeField] private SceneReference _scene = new();
        [SerializeField, Min(1)] private int _requiredScore = 1;
        [SerializeField, Min(1)] private int _requiredCrystals = 1;

        public SceneReference Scene => _scene;
        public int RequiredScore => _requiredScore;
        public int RequiredCrystals => _requiredCrystals;
    }
}
