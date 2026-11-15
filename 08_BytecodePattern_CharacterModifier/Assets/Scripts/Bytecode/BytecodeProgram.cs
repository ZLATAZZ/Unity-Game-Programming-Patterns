using System;

namespace CharacterModifierBytecodeDemo.Bytecode
{
    public sealed class BytecodeProgram
    {
        public string SourceName { get; }
        public byte[] Code { get; }

        public BytecodeProgram(string sourceName, byte[] code)
        {
            if (string.IsNullOrWhiteSpace(sourceName))
            {
                throw new ArgumentException("Source name cannot be empty.", nameof(sourceName));
            }

            SourceName = sourceName;
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }
    }
}
