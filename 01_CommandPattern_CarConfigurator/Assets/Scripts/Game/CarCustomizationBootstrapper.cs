using System;
using Patterns.Command.Core;
using Patterns.Command.Customization.Core;
using UnityEngine;

namespace Patterns.Command.Game
{
    public sealed class CarCustomizationBootstrapper : MonoBehaviour
    {
        [SerializeField]
        private CustomizationFeature[] _customizationFeatures;

        public CustomizationController Controller { get; private set; }

        private void Awake()
        {
            var commandHistory = new CommandHistory();

            Controller = new CustomizationController(commandHistory);
        }

        private void Start()
        {
            InitializeFeatures();
        }

        private void InitializeFeatures()
        {
            if (_customizationFeatures == null || _customizationFeatures.Length == 0)
            {
                throw new InvalidOperationException("No customization features are configured.");
            }

            foreach (CustomizationFeature feature in _customizationFeatures)
            {
                if (feature == null)
                {
                    throw new InvalidOperationException("Customization features contain a null reference.");
                }

                feature.Initialize();
            }
        }
    }
}