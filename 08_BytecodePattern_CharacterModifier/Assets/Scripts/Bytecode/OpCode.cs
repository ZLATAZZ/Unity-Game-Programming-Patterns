namespace CharacterModifierBytecodeDemo.Bytecode
{
    public enum OpCode : byte
    {
        PushConstant = 0x01,
        ReadProperty = 0x02,
        WriteProperty = 0x03,

        Add = 0x10,
        Subtract = 0x11,
        Multiply = 0x12,
        Divide = 0x13,

        End = 0xFF
    }
}
