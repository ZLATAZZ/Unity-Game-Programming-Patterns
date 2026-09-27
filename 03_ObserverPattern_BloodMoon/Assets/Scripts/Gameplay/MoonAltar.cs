using System;
using BloodMoon.Core;
using BloodMoon.Input;
using BloodMoon.Player;
using UnityEngine;

namespace BloodMoon.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public sealed class MoonAltar : MonoBehaviour
    {
        private BloodMoonSystem _bloodMoonSystem;
        private PlayerInputReader _playerInputReader;

        private bool _canInteract;
        private bool _isConsumed;
        private bool _isInitialized;
        private bool _isSubscribed;

        public event Action<bool> InteractionAvailabilityChanged;

        public void Initialize(BloodMoonSystem bloodMoonSystem, PlayerInputReader playerInputReader)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(MoonAltar)} has already been initialized.");
            }

            _bloodMoonSystem = bloodMoonSystem ?? throw new ArgumentNullException(nameof(bloodMoonSystem));
            _playerInputReader = playerInputReader ?? throw new ArgumentNullException(nameof(playerInputReader));

            _isInitialized = true;

            if (isActiveAndEnabled)
            {
                Subscribe();
            }
        }

        private void Awake()
        {
            Collider trigger = GetComponent<Collider>();

            if (!trigger.isTrigger)
            {
                throw new InvalidOperationException($"{nameof(MoonAltar)} requires its Collider to be configured as a trigger.");
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

        private void OnTriggerEnter(Collider other)
        {
            if (_isConsumed)
            {
                return;
            }

            if (other.GetComponentInParent<PlayerMarker>() == null)
            {
                return;
            }

            SetInteractionAvailable(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<PlayerMarker>() == null)
            {
                return;
            }

            SetInteractionAvailable(false);
        }

        private void HandleInteractPressed()
        {
            if (!_canInteract || _isConsumed)
            {
                return;
            }

            if (!_bloodMoonSystem.TryStart())
            {
                return;
            }

            _isConsumed = true;

            SetInteractionAvailable(false);
        }

        private void Subscribe()
        {
            if (_isSubscribed)
            {
                return;
            }

            _playerInputReader.InteractPressed += HandleInteractPressed;
            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed)
            {
                return;
            }

            _playerInputReader.InteractPressed -= HandleInteractPressed;
            _isSubscribed = false;
        }

        private void SetInteractionAvailable(bool isAvailable)
        {
            if (_canInteract == isAvailable)
            {
                return;
            }

            _canInteract = isAvailable;
            InteractionAvailabilityChanged?.Invoke(isAvailable);
        }
    }
}