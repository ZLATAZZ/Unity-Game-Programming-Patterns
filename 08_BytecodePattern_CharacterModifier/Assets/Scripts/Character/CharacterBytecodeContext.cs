using System;
using CharacterModifierBytecodeDemo.Bytecode;

namespace CharacterModifierBytecodeDemo.Character
{
    public sealed class CharacterBytecodeContext : IBytecodeContext
    {
        private readonly CharacterModifierTarget _target;

        public CharacterBytecodeContext(CharacterModifierTarget target)
        {
            _target = target ?? throw new ArgumentNullException(nameof(target));
        }

        public float ReadProperty(byte propertyId)
        {
            CharacterPropertyId property = (CharacterPropertyId)propertyId;

            switch (property)
            {
                case CharacterPropertyId.Scale:
                {
                    return _target.Scale;
                }

                case CharacterPropertyId.MoveSpeed:
                {
                    return _target.MoveSpeed;
                }

                case CharacterPropertyId.Health:
                {
                    return _target.Health;
                }

                default:
                {
                    throw new InvalidOperationException($"Unknown character property: {propertyId}.");
                }
            }
        }

        public void WriteProperty(byte propertyId, float value)
        {
            CharacterPropertyId property = (CharacterPropertyId)propertyId;

            switch (property)
            {
                case CharacterPropertyId.Scale:
                {
                    _target.SetScale(value);
                    break;
                }

                case CharacterPropertyId.MoveSpeed:
                {
                    _target.SetMoveSpeed(value);
                    break;
                }

                case CharacterPropertyId.Health:
                {
                    _target.SetHealth(value);
                    break;
                }

                default:
                {
                    throw new InvalidOperationException($"Unknown character property: {propertyId}.");
                }
            }
        }
    }
}
