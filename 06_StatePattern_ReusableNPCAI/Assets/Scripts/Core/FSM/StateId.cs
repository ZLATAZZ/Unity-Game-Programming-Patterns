using System;

namespace CursedForest.Core.FSM
{
    public readonly struct StateId : IEquatable<StateId>
    {
        private readonly string _value;

        public string Value => _value ?? string.Empty;
        public bool IsEmpty => string.IsNullOrEmpty(_value);

        public StateId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("State ID cannot be null or whitespace.", nameof(value));
            }

            _value = value.Trim();
        }

        public bool Equals(StateId other)
        {
            return StringComparer.Ordinal.Equals(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            return obj is StateId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value == null ? 0 : StringComparer.Ordinal.GetHashCode(_value);
        }

        public override string ToString()
        {
            return Value;
        }

        public static bool operator ==(StateId left, StateId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StateId left, StateId right)
        {
            return !left.Equals(right);
        }
    }
}