using System;

namespace CharacterModifierBytecodeDemo.Bytecode
{
    public sealed class BytecodeVm
    {
        private const int DefaultStackCapacity = 32;
        private const int MaxInstructionCount = 256;

        private readonly BytecodeStack _stack = new BytecodeStack(DefaultStackCapacity);

        public void Execute(BytecodeProgram program, IBytecodeContext context)
        {
            if (program == null)
            {
                throw new ArgumentNullException(nameof(program));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _stack.Clear();

            byte[] code = program.Code;
            int instructionPointer = 0;
            int executedInstructions = 0;

            while (instructionPointer < code.Length)
            {
                if (executedInstructions >= MaxInstructionCount)
                {
                    throw new InvalidOperationException("Bytecode instruction limit exceeded.");
                }

                executedInstructions++;

                OpCode opCode = (OpCode)ReadByte(code, ref instructionPointer);

                switch (opCode)
                {
                    case OpCode.PushConstant:
                    {
                        _stack.Push(ReadFloat(code, ref instructionPointer));
                        break;
                    }

                    case OpCode.ReadProperty:
                    {
                        byte propertyId = ReadByte(code, ref instructionPointer);
                        _stack.Push(context.ReadProperty(propertyId));
                        break;
                    }

                    case OpCode.WriteProperty:
                    {
                        byte propertyId = ReadByte(code, ref instructionPointer);
                        context.WriteProperty(propertyId, _stack.Pop());
                        break;
                    }

                    case OpCode.Add:
                    {
                        float right = _stack.Pop();
                        float left = _stack.Pop();
                        _stack.Push(left + right);
                        break;
                    }

                    case OpCode.Subtract:
                    {
                        float right = _stack.Pop();
                        float left = _stack.Pop();
                        _stack.Push(left - right);
                        break;
                    }

                    case OpCode.Multiply:
                    {
                        float right = _stack.Pop();
                        float left = _stack.Pop();
                        _stack.Push(left * right);
                        break;
                    }

                    case OpCode.Divide:
                    {
                        float right = _stack.Pop();
                        float left = _stack.Pop();

                        if (Math.Abs(right) <= float.Epsilon)
                        {
                            throw new InvalidOperationException("Bytecode attempted division by zero.");
                        }

                        _stack.Push(left / right);
                        break;
                    }

                    case OpCode.End:
                    {
                        return;
                    }

                    default:
                    {
                        throw new InvalidOperationException($"Unknown bytecode opcode: {(byte)opCode}.");
                    }
                }
            }

            throw new InvalidOperationException("Bytecode program reached the end without END.");
        }

        private static byte ReadByte(byte[] code, ref int instructionPointer)
        {
            EnsureBytesAvailable(code, instructionPointer, sizeof(byte));

            byte value = code[instructionPointer];
            instructionPointer += sizeof(byte);

            return value;
        }

        private static float ReadFloat(byte[] code, ref int instructionPointer)
        {
            EnsureBytesAvailable(code, instructionPointer, sizeof(float));

            float value = BitConverter.ToSingle(code, instructionPointer);
            instructionPointer += sizeof(float);

            return value;
        }

        private static void EnsureBytesAvailable(byte[] code, int instructionPointer, int requiredBytes)
        {
            if (instructionPointer + requiredBytes > code.Length)
            {
                throw new InvalidOperationException("Bytecode ended in the middle of an instruction.");
            }
        }
    }
}
