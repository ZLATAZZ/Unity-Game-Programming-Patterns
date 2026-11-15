using UnityEngine;

namespace CharacterModifierBytecodeDemo.Character
{
    public sealed class CharacterMover : MonoBehaviour
    {
        [SerializeField] private CharacterModifierTarget _target;
        [Min(0.1f)]
        [SerializeField] private float _travelDistance = 2.5f;

        private Vector3 _origin;

        private void Awake()
        {
            _origin = transform.position;
        }

        private void Update()
        {
            if (_target == null)
            {
                return;
            }

            float movementPhase = Time.time * _target.MoveSpeed * 0.35f;
            float normalizedOffset = Mathf.PingPong(movementPhase, 2.0f) - 1.0f;

            Vector3 position = _origin;
            position.x += normalizedOffset * _travelDistance;
            transform.position = position;
        }

        public void Configure(CharacterModifierTarget target)
        {
            _target = target;
        }
    }
}
