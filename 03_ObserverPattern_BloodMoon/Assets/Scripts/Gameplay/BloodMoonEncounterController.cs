using System;
using BloodMoon.Core;
using BloodMoon.Enemies;

namespace BloodMoon.Gameplay
{
    public sealed class BloodMoonEncounterController : IDisposable
    {
        private readonly BloodMoonSystem _bloodMoonSystem;
        private readonly GhostSpawner _ghostSpawner;

        private int _remainingGhosts;
        private bool _isDisposed;

        public BloodMoonEncounterController(BloodMoonSystem bloodMoonSystem, GhostSpawner ghostSpawner)
        {
            _bloodMoonSystem = bloodMoonSystem ?? throw new ArgumentNullException(nameof(bloodMoonSystem));
            _ghostSpawner = ghostSpawner != null ? ghostSpawner : throw new ArgumentNullException(nameof(ghostSpawner));

            _ghostSpawner.WaveStarted += HandleWaveStarted;
            _ghostSpawner.GhostDefeated += HandleGhostDefeated;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _ghostSpawner.WaveStarted -= HandleWaveStarted;
            _ghostSpawner.GhostDefeated -= HandleGhostDefeated;

            _isDisposed = true;
        }

        private void HandleWaveStarted(int ghostCount)
        {
            if (ghostCount <= 0)
            {
                throw new InvalidOperationException("A Blood Moon wave must contain at least one Ghost.");
            }

            _remainingGhosts = ghostCount;
        }

        private void HandleGhostDefeated()
        {
            if (!_bloodMoonSystem.IsActive)
            {
                return;
            }

            if (_remainingGhosts <= 0)
            {
                throw new InvalidOperationException($"{nameof(BloodMoonEncounterController)} received more Ghost defeats than expected.");
            }

            _remainingGhosts--;

            if (_remainingGhosts == 0)
            {
                _bloodMoonSystem.TryEnd();
            }
        }
    }
}