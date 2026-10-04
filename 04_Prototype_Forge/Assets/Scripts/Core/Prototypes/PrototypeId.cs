using System;

namespace PrototypeForge.Core.Prototypes
{
    public readonly struct PrototypeId : IEquatable<PrototypeId>
    {
        private readonly string _value;

        public string Value => _value ?? string.Empty;
        public bool IsEmpty => string.IsNullOrEmpty(_value);

        public PrototypeId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Prototype ID cannot be empty.", nameof(value));
            }

            _value = value.Trim();
        }

        public bool Equals(PrototypeId other)
        {
            return StringComparer.Ordinal.Equals(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            return obj is PrototypeId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value == null ? 0 : StringComparer.Ordinal.GetHashCode(_value);
        }

        public override string ToString()
        {
            return Value;
        }

        public static bool operator ==(PrototypeId left, PrototypeId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PrototypeId left, PrototypeId right)
        {
            return !left.Equals(right);
        }
    }
}