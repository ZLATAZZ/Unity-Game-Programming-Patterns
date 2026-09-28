using System;
using BloodMoon.Gameplay;
using BloodMoon.Input;
using BloodMoon.Presentation;
using UnityEngine;

namespace BloodMoon.UI
{
    public sealed class AltarInteractionPrompt : MonoBehaviour
    {
        [SerializeField] private InteractionPromptView _view;

        private MoonAltar _altar;
        private bool _isInitialized;
        private bool _isSubscribed;

        public void Initialize(MoonAltar altar, PlayerInputReader inputReader)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(AltarInteractionPrompt)} has already been initialized.");
            }

            if (_view == null)
            {
                throw new InvalidOperationException($"{nameof(AltarInteractionPrompt)} requires an InteractionPromptView reference.");
            }

            _altar = altar != null ? altar : throw new ArgumentNullException(nameof(altar));

            if (inputReader == null)
            {
                throw new ArgumentNullException(nameof(inputReader));
            }

            _view.SetBinding(inputReader.InteractBindingDisplayName);

            _isInitialized = true;

            if (isActiveAndEnabled)
            {
                Subscribe();
            }
        }

        private void OnEnable()
        {
            if (_isInitialized)
            {
                Subscribe();
            }
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void HandleInteractionAvailabilityChanged(bool isAvailable)
        {
            if (isAvailable)
            {
                _view.Show();
                return;
            }

            _view.Hide();
        }

        private void Subscribe()
        {
            if (_isSubscribed)
            {
                return;
            }

            _altar.InteractionAvailabilityChanged += HandleInteractionAvailabilityChanged;
            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed)
            {
                return;
            }

            _altar.InteractionAvailabilityChanged -= HandleInteractionAvailabilityChanged;
            _isSubscribed = false;
        }
    }
}