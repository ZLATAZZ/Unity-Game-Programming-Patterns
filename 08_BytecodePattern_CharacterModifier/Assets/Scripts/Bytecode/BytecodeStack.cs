using System;

namespace CharacterModifierBytecodeDemo.Bytecode
{
    internal sealed class BytecodeStack
    {
        private readonly float[] _values;
        private int _count;

        public BytecodeStack(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            _values = new float[capacity];
        }

        public void Clear()
        {
            _count = 0;
        }

        public void Push(float value)
        {
            if (_count >= _values.Length)
            {
                throw new InvalidOperationException("Bytecode stack overflow.");
            }

            _values[_count] = value;
            _count++;
        }

        public float Pop()
        {
            if (_count == 0)
            {
                throw new InvalidOperationException("Bytecode stack underflow.");
            }

            _count--;
            return _values[_count];
        }
    }
}
