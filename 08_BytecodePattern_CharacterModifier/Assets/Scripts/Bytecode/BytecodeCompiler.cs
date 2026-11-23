using System;
using System.Collections.Generic;
using System.Globalization;

namespace CharacterModifierBytecodeDemo.Bytecode
{
    public sealed class BytecodeCompiler
    {
        public BytecodeProgram Compile(string sourceName, string source, IBytecodeSymbolTable symbolTable)
        {
            if (string.IsNullOrWhiteSpace(sourceName))
            {
                throw new ArgumentException("Source name cannot be empty.", nameof(sourceName));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (symbolTable == null)
            {
                throw new ArgumentNullException(nameof(symbolTable));
            }

            var code = new List<byte>();
            string[] lines = source.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

            int stackDepth = 0;
            bool reachedEnd = false;

            for (int i = 0; i < lines.Length; i++)
            {
                int lineNumber = i + 1;
                string line = StripComment(lines[i]).Trim();

                if (line.Length == 0)
                {
                    continue;
                }

                if (reachedEnd)
                {
                    throw CreateCompileError(sourceName, lineNumber, "No instructions are allowed after END.");
                }

                string[] tokens = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                string instruction = tokens[0].ToUpperInvariant();

                switch (instruction)
                {
                    case "PUSH":
                    {
                        RequireTokenCount(sourceName, lineNumber, tokens, 2);

                        if (!float.TryParse(tokens[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float value))
                        {
                            throw CreateCompileError(sourceName, lineNumber, $"'{tokens[1]}' is not a valid float.");
                        }

                        Emit(code, OpCode.PushConstant);
                        code.AddRange(BitConverter.GetBytes(value));
                        stackDepth++;
                        break;
                    }

                    case "READ":
                    {
                        RequireTokenCount(sourceName, lineNumber, tokens, 2);
                        byte propertyId = ResolveProperty(sourceName, lineNumber, tokens[1], symbolTable);

                        Emit(code, OpCode.ReadProperty);
                        code.Add(propertyId);
                        stackDepth++;
                        break;
                    }

                    case "WRITE":
                    {
                        RequireTokenCount(sourceName, lineNumber, tokens, 2);
                        RequireStackDepth(sourceName, lineNumber, stackDepth, 1);

                        byte propertyId = ResolveProperty(sourceName, lineNumber, tokens[1], symbolTable);

                        Emit(code, OpCode.WriteProperty);
                        code.Add(propertyId);
                        stackDepth--;
                        break;
                    }

                    case "ADD":
                    {
                        RequireTokenCount(sourceName, lineNumber, tokens, 1);
                        stackDepth = EmitBinaryOperation(sourceName, lineNumber, code, stackDepth, OpCode.Add);
                        break;
                    }

                    case "SUB":
                    {
                        RequireTokenCount(sourceName, lineNumber, tokens, 1);
                        stackDepth = EmitBinaryOperation(sourceName, lineNumber, code, stackDepth, OpCode.Subtract);
                        break;
                    }

                    case "MUL":
                    {
                        RequireTokenCount(sourceName, lineNumber, tokens, 1);
                        stackDepth = EmitBinaryOperation(sourceName, lineNumber, code, stackDepth, OpCode.Multiply);
                        break;
                    }

                    case "DIV":
                    {
                        RequireTokenCount(sourceName, lineNumber, tokens, 1);
                        stackDepth = EmitBinaryOperation(sourceName, lineNumber, code, stackDepth, OpCode.Divide);
                        break;
                    }

                    case "END":
                    {
                        RequireTokenCount(sourceName, lineNumber, tokens, 1);

                        if (stackDepth != 0)
                        {
                            throw CreateCompileError(
                                sourceName,
                                lineNumber,
                                $"END requires an empty stack, but the compile-time stack depth is {stackDepth}.");
                        }

                        Emit(code, OpCode.End);
                        reachedEnd = true;
                        break;
                    }

                    default:
                    {
                        throw CreateCompileError(sourceName, lineNumber, $"Unknown instruction '{tokens[0]}'.");
                    }
                }
            }

            if (!reachedEnd)
            {
                throw new FormatException($"{sourceName}: program must contain END.");
            }

            return new BytecodeProgram(sourceName, code.ToArray());
        }

        private static int EmitBinaryOperation(
            string sourceName,
            int lineNumber,
            List<byte> code,
            int stackDepth,
            OpCode opCode)
        {
            RequireStackDepth(sourceName, lineNumber, stackDepth, 2);
            Emit(code, opCode);

            return stackDepth - 1;
        }

        private static byte ResolveProperty(
            string sourceName,
            int lineNumber,
            string symbol,
            IBytecodeSymbolTable symbolTable)
        {
            if (!symbolTable.TryResolveProperty(symbol, out byte propertyId))
            {
                throw CreateCompileError(sourceName, lineNumber, $"Unknown property '{symbol}'.");
            }

            return propertyId;
        }

        private static void RequireTokenCount(
            string sourceName,
            int lineNumber,
            string[] tokens,
            int expectedCount)
        {
            if (tokens.Length != expectedCount)
            {
                throw CreateCompileError(
                    sourceName,
                    lineNumber,
                    $"Expected {expectedCount} token(s), but found {tokens.Length}.");
            }
        }

        private static void RequireStackDepth(
            string sourceName,
            int lineNumber,
            int currentDepth,
            int requiredDepth)
        {
            if (currentDepth < requiredDepth)
            {
                throw CreateCompileError(
                    sourceName,
                    lineNumber,
                    $"Stack underflow: instruction requires {requiredDepth} value(s), but only {currentDepth} are available.");
            }
        }

        private static string StripComment(string line)
        {
            int commentIndex = line.IndexOf('#');

            return commentIndex >= 0
                ? line.Substring(0, commentIndex)
                : line;
        }

        private static void Emit(List<byte> code, OpCode opCode)
        {
            code.Add((byte)opCode);
        }

        private static FormatException CreateCompileError(string sourceName, int lineNumber, string message)
        {
            return new FormatException($"{sourceName}, line {lineNumber}: {message}");
        }
    }
}
