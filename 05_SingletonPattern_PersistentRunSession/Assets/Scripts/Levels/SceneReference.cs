using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Patterns.Singleton.Levels
{
    [Serializable]
    public sealed class SceneReference
    {
#if UNITY_EDITOR
        [SerializeField] private UnityEditor.SceneAsset _sceneAsset;
#endif
        [SerializeField, HideInInspector] private string _scenePath;

        public string ScenePath => _scenePath;

        public int BuildIndex
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_scenePath))
                {
                    throw new InvalidOperationException("Scene reference has no configured scene path.");
                }

                int index = SceneUtility.GetBuildIndexByScenePath(_scenePath);

                if (index < 0)
                {
                    throw new InvalidOperationException($"Scene '{_scenePath}' is missing or disabled in Build Profiles / Scene List.");
                }

                return index;
            }
        }
    }
}
