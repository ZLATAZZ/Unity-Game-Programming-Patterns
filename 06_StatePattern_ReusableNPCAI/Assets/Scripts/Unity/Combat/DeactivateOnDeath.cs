using UnityEngine;

namespace CursedForest.Unity.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(DamageableHealth))]
    public sealed class DeactivateOnDeath : MonoBehaviour
    {
        private DamageableHealth _health;

        private void Awake()
        {
            _health = GetComponent<DamageableHealth>();
        }

        private void OnEnable()
        {
            _health.Died += HandleDeath;
        }

        private void OnDisable()
        {
            _health.Died -= HandleDeath;
        }

        private void HandleDeath()
        {
            gameObject.SetActive(false);
        }
    }
}