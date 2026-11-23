using UnityEngine;

namespace CharacterModifierBytecodeDemo.Character
{
    public sealed class CharacterModifierTarget : MonoBehaviour
    {
        private const float MinScale = 0.25f;
        private const float MaxScale = 3.0f;
        private const float MinMoveSpeed = 0.0f;
        private const float MaxMoveSpeed = 20.0f;
        private const float MinHealth = 0.0f;
        private const float MaxHealth = 100.0f;

        [SerializeField] private float _initialScale = 1.0f;
        [SerializeField] private float _initialMoveSpeed = 4.0f;
        [SerializeField] private float _initialHealth = 100.0f;

        public float Scale => transform.localScale.x;
        public float MoveSpeed { get; private set; }
        public float Health { get; private set; }

        private void Awake()
        {
            ResetState();
        }

        public void SetScale(float value)
        {
            float clampedValue = Mathf.Clamp(value, MinScale, MaxScale);
            transform.localScale = Vector3.one * clampedValue;
        }

        public void SetMoveSpeed(float value)
        {
            MoveSpeed = Mathf.Clamp(value, MinMoveSpeed, MaxMoveSpeed);
        }

        public void SetHealth(float value)
        {
            Health = Mathf.Clamp(value, MinHealth, MaxHealth);
        }

        public void ResetState()
        {
            SetScale(_initialScale);
            SetMoveSpeed(_initialMoveSpeed);
            SetHealth(_initialHealth);
        }
    }
}
