using System;
using System.Collections.Generic;
using Patterns.Singleton.Gameplay;
using Patterns.Singleton.Levels;
using Patterns.Singleton.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Patterns.Singleton.Composition
{
    public sealed class LevelSceneBootstrapper : MonoBehaviour
    {
        [SerializeField] private LevelSequenceConfig _levelSequence;
        [SerializeField] private int _levelIndex;
        [SerializeField] private RunDemoControls _controls;
        [SerializeField] private RunHud _hud;
        [SerializeField] private LevelProgressionController _progressionController;

        private void Start()
        {
            ValidateConfiguration();

            IReadOnlyList<LevelDefinition> levels = _levelSequence.Levels;
            LevelDefinition currentLevel = levels[_levelIndex];
            LevelDefinition nextLevel = _levelIndex + 1 < levels.Count ? levels[_levelIndex + 1] : null;

            _controls.Initialize(currentLevel);
            _hud.Initialize(currentLevel);
            _progressionController.Initialize(currentLevel, nextLevel);
        }

        private void ValidateConfiguration()
        {
            if (_levelSequence == null || _controls == null || _hud == null || _progressionController == null)
            {
                throw new InvalidOperationException($"{nameof(LevelSceneBootstrapper)} requires a level sequence and all three consumer components.");
            }

            IReadOnlyList<LevelDefinition> levels = _levelSequence.Levels;

            if (levels == null || levels.Count == 0)
            {
                throw new InvalidOperationException("Level sequence must contain at least one level.");
            }

            if (_levelIndex < 0 || _levelIndex >= levels.Count)
            {
                throw new InvalidOperationException($"Level index {_levelIndex} is outside the configured sequence.");
            }

            HashSet<string> scenePaths = new(StringComparer.Ordinal);

            for (int i = 0; i < levels.Count; i++)
            {
                LevelDefinition level = levels[i];

                if (level == null || level.Scene == null)
                {
                    throw new InvalidOperationException($"Level at index {i} has no valid scene definition.");
                }

                string path = level.Scene.ScenePath;
                _ = level.Scene.BuildIndex;

                if (!scenePaths.Add(path))
                {
                    throw new InvalidOperationException($"Scene '{path}' is configured more than once.");
                }

                if (level.RequiredScore <= 0 || level.RequiredCrystals <= 0)
                {
                    throw new InvalidOperationException($"Level at index {i} requires positive score and crystal targets.");
                }
            }

            string activeScenePath = SceneManager.GetActiveScene().path;
            string expectedScenePath = levels[_levelIndex].Scene.ScenePath;

            if (!string.Equals(activeScenePath, expectedScenePath, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Configured level index {_levelIndex} points to '{expectedScenePath}', but active scene is '{activeScenePath}'.");
            }
        }
    }
}
