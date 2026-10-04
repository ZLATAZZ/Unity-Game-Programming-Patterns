using System;
using PrototypeForge.Core.Enemies;
using TMPro;
using UnityEngine;

namespace PrototypeForge.Unity.Visuals
{
    public sealed class EnemyActor : MonoBehaviour
    {
        [SerializeField] private TMP_Text _worldLabel;

        private EnemyPrototype _configuration;
        private bool _isInitialized;

        public EnemyPrototype Configuration
        {
            get
            {
                if (!_isInitialized)
                {
                    throw new InvalidOperationException($"{nameof(EnemyActor)} has not been initialized.");
                }

                return _configuration;
            }
        }

        public void Initialize(EnemyPrototype configuration)
        {
            if (_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(EnemyActor)} has already been initialized.");
            }

            if (_worldLabel == null)
            {
                throw new InvalidOperationException($"{nameof(EnemyActor)} requires a world label.");
            }

            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _isInitialized = true;

            RefreshView();
        }

        public void ApplyOvercharge(int damageIncrease)
        {
            if (!_isInitialized)
            {
                throw new InvalidOperationException($"{nameof(EnemyActor)} has not been initialized.");
            }

            _configuration.Weapon.IncreaseDamage(damageIncrease);

            RefreshView();
        }

        private void RefreshView()
        {
            _worldLabel.text =
                $"{_configuration.DisplayName}\n" +
                $"HP {_configuration.MaxHealth}\n" +
                $"DMG {_configuration.Weapon.Damage}";
        }
    }
}