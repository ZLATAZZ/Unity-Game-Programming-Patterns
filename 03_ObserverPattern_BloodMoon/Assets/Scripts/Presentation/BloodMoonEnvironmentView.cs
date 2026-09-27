using System;
using DG.Tweening;
using UnityEngine;

namespace BloodMoon.Presentation
{
    public sealed class BloodMoonEnvironmentView : MonoBehaviour
    {
        [SerializeField] private Light[] _lights;
        [SerializeField] private ParticleSystem[] _bloodMoonParticles;

        [Header("Blood Moon")]
        [SerializeField] private Color _bloodMoonColor = new(0.65f, 0.08f, 0.08f);
        [SerializeField, Min(0f)] private float _intensityMultiplier = 0.65f;
        [SerializeField, Min(0f)] private float _transitionDuration = 0.8f;

        private Color[] _defaultColors;
        private float[] _defaultIntensities;

        private Sequence _transitionSequence;
        private bool _isInitialized;

        private void Awake()
        {
            EnsureInitialized();
        }

        public void PlayBloodMoon()
        {
            EnsureInitialized();

            PlayParticles();
            AnimateLights(true);
        }

        public void PlayCalm()
        {
            EnsureInitialized();

            StopParticles();
            AnimateLights(false);
        }

        public void SetBloodMoonImmediate(bool isActive)
        {
            EnsureInitialized();
            KillTween();

            for (int i = 0; i < _lights.Length; i++)
            {
                _lights[i].color = isActive ? _bloodMoonColor : _defaultColors[i];
                _lights[i].intensity = isActive ? _defaultIntensities[i] * _intensityMultiplier : _defaultIntensities[i];
            }

            if (isActive)
            {
                PlayParticles();
            }
            else
            {
                StopParticles();
            }
        }

        private void AnimateLights(bool isBloodMoonActive)
        {
            KillTween();

            _transitionSequence = DOTween.Sequence();

            for (int i = 0; i < _lights.Length; i++)
            {
                Light light = _lights[i];
                Color targetColor = isBloodMoonActive ? _bloodMoonColor : _defaultColors[i];
                float targetIntensity = isBloodMoonActive ? _defaultIntensities[i] * _intensityMultiplier : _defaultIntensities[i];

                _transitionSequence.Join(DOTween.To(() => light.color, value => light.color = value, targetColor, _transitionDuration));
                _transitionSequence.Join(DOTween.To(() => light.intensity, value => light.intensity = value, targetIntensity, _transitionDuration));
            }
        }

        private void PlayParticles()
        {
            for (int i = 0; i < _bloodMoonParticles.Length; i++)
            {
                _bloodMoonParticles[i].Play();
            }
        }

        private void StopParticles()
        {
            for (int i = 0; i < _bloodMoonParticles.Length; i++)
            {
                _bloodMoonParticles[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        private void EnsureInitialized()
        {
            if (_isInitialized)
            {
                return;
            }

            ValidateConfiguration();

            _defaultColors = new Color[_lights.Length];
            _defaultIntensities = new float[_lights.Length];

            for (int i = 0; i < _lights.Length; i++)
            {
                _defaultColors[i] = _lights[i].color;
                _defaultIntensities[i] = _lights[i].intensity;
            }

            _isInitialized = true;
        }

        private void KillTween()
        {
            _transitionSequence?.Kill();
            _transitionSequence = null;
        }

        private void OnDestroy()
        {
            KillTween();
        }

        private void ValidateConfiguration()
        {
            if (_lights == null || _lights.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(BloodMoonEnvironmentView)} requires at least one Light reference.");
            }

            for (int i = 0; i < _lights.Length; i++)
            {
                if (_lights[i] == null)
                {
                    throw new InvalidOperationException($"{nameof(BloodMoonEnvironmentView)} contains a null Light reference at index {i}.");
                }
            }

            if (_bloodMoonParticles == null)
            {
                throw new InvalidOperationException($"{nameof(BloodMoonEnvironmentView)} requires a Particle System array.");
            }

            for (int i = 0; i < _bloodMoonParticles.Length; i++)
            {
                if (_bloodMoonParticles[i] == null)
                {
                    throw new InvalidOperationException($"{nameof(BloodMoonEnvironmentView)} contains a null Particle System reference at index {i}.");
                }
            }
        }
    }
}